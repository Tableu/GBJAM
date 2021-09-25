using System;
using System.Collections;
using Attacks;
using UnityEngine.InputSystem;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerController : MonoBehaviour, IDamageable
{
    private PlayerInputActions _playerInputActions;
    private ContactFilter2D _groundFilter2D; 
    private AttackCommand _attackCommand;
    private MovementController _movementController;

    [SerializeField] private PlayerStats meleeStats;
    [SerializeField] private PlayerStats currentStats;
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private Collider2D col;
    [SerializeField] private PlayerAnimatorController playerAnimatorController;
    [SerializeField] private SpriteRenderer playerShellSpriteRenderer;
    [SerializeField] private AttackScriptableObject _attack;
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private SpriteRenderer smearSprite;

    [SerializeField] private int health;
    [SerializeField] private int armor;
    [SerializeField] private Vector2 speed;
    [SerializeField] private Sprite shell;
    [SerializeField] private Sprite damagedShell;
    [Header("Bools")]
    [SerializeField] private bool grounded;
    [SerializeField] public bool frontClear;
    [SerializeField] public bool inputLocked;
    [SerializeField] private bool hiding;
    [SerializeField] private bool nearCeiling;
    [Header("Shell Prefabs")]
    [SerializeField] private GameObject snailShell;
    [SerializeField] private GameObject spikyShell;
    [SerializeField] private GameObject conchShell;

    private const int NO_SHELL = 0;
    private const int SNAIL_SHELL = 1;
    private const int SPIKY_SHELL = 2;
    private const int CONCH_SHELL = 3;
    public int Health
    {
        get { return health; }
    }
    public int Armor
    {
        get { return armor; }
    }
    // [SerializeField, Range(0, 1f)] private float knockBackDuration = 0.25f;
    // private bool isInKnockback = false;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _movementController = new MovementController(gameObject, speed.x);
    }

    private void OnEnable()
    {
        _playerInputActions.Enable();
    }

    private void OnDisable()
    {
        _playerInputActions.Disable();
    }
    void Start()
    {
        _playerInputActions.Player.Jump.started += (context =>
        {
            if (_movementController.Jump(speed.y))
            {
                pSoundManager.PlaySound(pSoundManager.Sound.pJump);
                if (hiding)
                {
                    _playerInputActions.Player.Hide.Disable();
                    _playerInputActions.Player.Hide.Enable();
                }
            }
        });
        _playerInputActions.Player.Move.canceled += Idle;
        _playerInputActions.Player.PickUpShell.started += SwitchShells;

        _playerInputActions.Player.Hide.started += Hide;
        _playerInputActions.Player.Hide.canceled += Hide;

        _playerInputActions.Player.Attack.started += Attack;
        _groundFilter2D = new ContactFilter2D
        {
            layerMask = LayerMask.GetMask("Ground"),
            useLayerMask = true
        };
        
        int savedShell = PlayerPrefs.GetInt("Shell", 0);
        GameObject shell;
        switch (savedShell)
        {
            case NO_SHELL:
                SetStats(gameObject.GetComponent<PlayerStats>());
                break;
            case SNAIL_SHELL: 
                shell = Instantiate(snailShell, transform.position, Quaternion.identity);
                EquipShell(shell);
                SetStats(shell.GetComponent<PlayerStats>());
                break;
            case SPIKY_SHELL: 
                shell = Instantiate(spikyShell, transform.position, Quaternion.identity);
                EquipShell(shell);
                SetStats(shell.GetComponent<PlayerStats>());
                break;
            case CONCH_SHELL: 
                shell = Instantiate(conchShell, transform.position, Quaternion.identity);
                EquipShell(shell);
                SetStats(shell.GetComponent<PlayerStats>());
                break;
        }
        CinemachineVirtualCamera virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCamera)
        {
            virtualCamera.Follow = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        grounded = _movementController.Grounded();
        nearCeiling = _movementController.NearCeiling();
        if (grounded)
        {
            playerAnimatorController.SetIsGrounded(true);
            //_playerInputActions.Player.Hide.Enable();
        }
        else
        {
            playerAnimatorController.SetIsGrounded(false);
            //_playerInputActions.Player.Hide.Disable();
        }
        frontClear = _movementController.FrontClear();
        Move();
        if (_attackCommand != null)
        {
            if (_attackCommand.LockInput)
            {
                inputLocked = true;
            }
            else
            {
                inputLocked = false;
            }
        }
    }

    public void SetStats(PlayerStats shellStats)
    {
        armor = shellStats.armor;
        speed = shellStats.speed;
        _movementController.WalkingSpeed = shellStats.speed.x;
        if (hiding)
        {
            _movementController.WalkingSpeed *= 0.4f;
        }
        _attack = shellStats.attackConfig;
        if (_attack != null)
        {
            _attackCommand = _attack.MakeAttack();
        }
        shell = shellStats.shellSprite;
        damagedShell = shellStats.damagedShell;
        currentStats = shellStats;
        PlayerPrefs.SetInt("Shell", currentStats.shell);
        _playerInputActions.Player.Attack.Enable();
        //Update UI each time stats are changed.
        HUDManager.Instance.UpdateHealth(health);
        HUDManager.Instance.UpdateArmor(armor);
    }
    private void Move()
    {
        var horizontal = _playerInputActions.Player.Move.ReadValue<float>();
        var horizontalVelocity = horizontal * speed.x;
        if (!inputLocked)
        {
            _movementController.MoveHorizontally(horizontalVelocity);
            if (horizontalVelocity != 0)
            {
                playerAnimatorController.SetIsMoving(true);
            }
        }
    }

    private void Idle(InputAction.CallbackContext context)
    {
        rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
        playerAnimatorController.SetIsMoving(false);
    }

    private void Attack(InputAction.CallbackContext context)
    {
        if (_attackCommand != null)
        {
            Debug.Log("Attack");
            if (!_attackCommand.IsRunning && !hiding)
            {
                Debug.Log(_attack.GetType());
                if (_attack.GetType() == typeof(Attacks.MeleeAttack))
                {
                    playerAnimatorController.TriggerAttack();
                }

                if (_attack.GetType() == typeof(Attacks.DashAttack))
                {
                    playerAnimatorController.TriggerAttack();
                    particleSystem.Emit(20);
                    StartCoroutine(DashCooldown(1f));
                }
                StartCoroutine(_attackCommand.DoAttack(gameObject));
                pSoundManager.PlaySound(pSoundManager.Sound.pAttack);
            }
        }
    }

    private IEnumerator DashCooldown(float coolDown)
    {
        _playerInputActions.Player.Attack.Disable();
        yield return new WaitForSeconds(coolDown);
        _playerInputActions.Player.Attack.Enable();
    }
    private void SwitchShells(InputAction.CallbackContext context)
    {
        ContactFilter2D contactFilter2D = new ContactFilter2D
        {
            layerMask = LayerMask.GetMask("Shells"),
            useLayerMask = true
        };
        Collider2D[] collider2D = new Collider2D[1];
        if (grounded)
        {
            if (Physics2D.OverlapCollider(col, contactFilter2D, collider2D) == 1)
            {
                var newShell = collider2D[0].gameObject;
                DropShell();
                SetStats(newShell.GetComponent<PlayerStats>());
                EquipShell(newShell);
            }
            else
            {
                DropShell();
                playerShellSpriteRenderer.sprite = null;
                SetStats(meleeStats);
            }
            pSoundManager.PlaySound(pSoundManager.Sound.pPickup);
        }
    }

    private void EquipShell(GameObject newShell)
    {
        shell = newShell.GetComponent<PlayerStats>().shellSprite;
        damagedShell = newShell.GetComponent<PlayerStats>().damagedShell;
        playerShellSpriteRenderer.sprite = newShell.GetComponent<SpriteRenderer>().sprite;
        newShell.transform.parent = gameObject.transform;
        newShell.SetActive(false);
    }
    private void DropShell()
    {
        if (transform.childCount > 1)
        {
            var oldShell = transform.GetChild(1);
            oldShell.localPosition = Vector3.zero;
            oldShell.gameObject.SetActive(true);
            oldShell.localScale = new Vector3(transform.localScale.x, oldShell.localScale.y, oldShell.localScale.z);
            oldShell.GetComponent<PlayerStats>().armor = armor;
            if (armor == 1)
            {
                oldShell.gameObject.GetComponent<ShellAnimator>().SetIsDamaged();
            }
            oldShell.SetParent(null);
        }
    }
    private void Hide(InputAction.CallbackContext context)
    {
        if ((context.started || context.performed) && !hiding && grounded)
        {
            hiding = true;
            _movementController.WalkingSpeed *= 0.4f;
            _movementController.Stop();
            playerAnimatorController.SetIsHiding(true);
            pSoundManager.PlaySound(pSoundManager.Sound.pHide);
            BoxCollider2D box = (BoxCollider2D)col;
            box.size = new Vector2(box.size.x, box.size.y*0.5f);
            box.offset = new Vector2(box.offset.x, box.offset.y*0.5f);
        }
        else if (context.canceled && hiding && !nearCeiling)
        {
            StopHiding();
        }
        else if(context.canceled && hiding && nearCeiling)
        {
            StartCoroutine(autoStopHide());
        }
        
    }

    private IEnumerator autoStopHide()
    {
        while (hiding)
        {
            if (!nearCeiling && hiding && grounded)
            {
                StopHiding();
            }
            yield return null;
        }
    }
    
    private void StopHiding()
    {
        hiding = false;
        _movementController.WalkingSpeed = currentStats.speed.x;
        playerAnimatorController.SetIsHiding(false);
        BoxCollider2D box = (BoxCollider2D)col;
        box.offset = new Vector2(box.offset.x, box.offset.y*2f);
        box.size = new Vector2(box.size.x, box.size.y*2f);
    }
    public void TakeDamage(Damage dmg)
    {
        if (_attackCommand.IsRunning && _attack.GetType() == typeof(Attacks.DashAttack))
        {
            return;
        }

        if (_attackCommand.IsRunning && _attack.GetType() == typeof(Attacks.MeleeAttack) && smearSprite.enabled)
        {
            return;
        }
        if (armor > 0)
        {
            LoseArmor(dmg);
        }
        else
        {
            LoseHealth(dmg);
        }
        pSoundManager.PlaySound(pSoundManager.Sound.pHit);
        StartCoroutine(Invulnerable());
        //Only do the Knockback coroutine if knockback on dmg isn't 0, so player doesn't come to a full stop for a moment if knockback is 0.
        if (dmg.Knockback != 0)
        {
            StartCoroutine(_movementController.Knockback(dmg));
        }
    }

    private void LoseArmor(Damage dmg)
    {
        armor -= dmg.RawDamage;
        HUDManager.Instance.UpdateArmor(Mathf.Max(0, armor));
        if (armor <= 0)
        {
            BreakShell();

        }
        else if (armor == 1)
        {
            playerShellSpriteRenderer.sprite = damagedShell;
        }
        if (armor <= 0) { BreakShell(); }
        Debug.Log("Lose Armor");
    }
    private void LoseHealth(Damage dmg)
    {
        health -= dmg.RawDamage;
        HUDManager.Instance.UpdateHealth(Mathf.Max(0, health));
        if (health <= 0)
        {
            Death();
        }
        Debug.Log("Lose Health");
    }

    private void Death()
    {
        pSoundManager.PlaySound(pSoundManager.Sound.pDie);
        //Tell MapManager the player died, it handles respawn and such.
        if (MapManager.Instance)
        {
            MapManager.Instance.PlayerDied();
        }
        PlayerPrefs.SetInt("Shell", NO_SHELL);
        //Perform other death tasks
        Destroy(gameObject);
    }

    private void BreakShell()
    {
        playerShellSpriteRenderer.sprite = null;
        shell = null;
        damagedShell = null;
        if (transform.childCount > 1)
        {
            var shell = transform.GetChild(1);
            shell.transform.position = transform.position;
            shell.transform.localScale = transform.localScale;
            shell.parent = null;
            shell.gameObject.SetActive(true);
            shell.gameObject.GetComponent<ShellAnimator>().SetIsDead();
            Destroy(shell.gameObject, 0.5f);
        }
        SetStats(meleeStats);
        //switch to melee
    }
    
    private IEnumerator Invulnerable()
    {
        gameObject.layer = LayerMask.NameToLayer("Invulnerable");
        playerAnimatorController.SetIsInvulnerable(true);
        yield return new WaitForSeconds(1);
        gameObject.layer = LayerMask.NameToLayer("Player");
        playerAnimatorController.SetIsInvulnerable(false);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (LayerMask.LayerToName(other.transform.gameObject.layer))
        {
            case "Secret":
                //Do something??
                break;
            case "Spikes":
                //TakeDamage(new Damage(new Vector2(transform.position.x - transform.localScale.x, transform.position.y), 20f, 1));
                Damage dmg = new Damage(transform.position, 0, 1);
                if (armor > 0)
                {
                    LoseArmor(dmg);
                }
                else
                {
                    LoseHealth(dmg);
                }
                pSoundManager.PlaySound(pSoundManager.Sound.pHit);
                StartCoroutine(Invulnerable());
                //Only do the Knockback coroutine if knockback on dmg isn't 0, so player doesn't come to a full stop for a moment if knockback is 0.
                if (dmg.Knockback != 0)
                {
                    StartCoroutine(_movementController.Knockback(dmg));
                }
                break;
        }
    }

    //private void OnCollisionExit(Collision other)
    //{
    //    if (other.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
    //    {
    //        Grounded();
    //    }
    //}
}



#if UNITY_EDITOR
[CustomEditor(typeof(PlayerController))]
class PlayerControllerEditor : Editor
{
    PlayerController player { get { return target as PlayerController; } }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (Application.isPlaying)
        {
            EditorExtensionMethods.DrawSeparator(Color.gray);
            if (GUILayout.Button("Damage left"))
            {
                Damage auxDamage = new Damage((Vector2)player.transform.position + new Vector2(0.5f, -0.5f), 20f, 0);
                player.TakeDamage(auxDamage);
            }
            if (GUILayout.Button("Damage right"))
            {
                Damage auxDamage = new Damage((Vector2)player.transform.position + new Vector2(-0.5f, -0.5f), 20f, 0);
                player.TakeDamage(auxDamage);
            }
        }
    }
}
#endif