using System;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine;
using Cinemachine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerController : MonoBehaviour, IDamageable
{
    [Serializable]
    public struct PlayerStats
    {
        public PlayerStats(Vector2 speed, Vector2 maxSpeed, int armor, AttackScriptableObject attack)
        {
            _speed = speed;
            _maxSpeed = maxSpeed;
            _armor = armor;
            _attack = attack;
        }

        [SerializeField] private Vector2 _speed;
        [SerializeField] private Vector2 _maxSpeed;
        [SerializeField] private int _armor;
        [SerializeField] private AttackScriptableObject _attack;
        public Vector2 Speed => _speed;
        public Vector2 MaxSpeed => _maxSpeed;
        public int Armor => _armor;
        public AttackScriptableObject Attack => _attack;
    }
    private PlayerInputActions _playerInputActions;
    private ContactFilter2D _groundFilter2D;
    private AttackCommand _attackCommand;
    private MovementController _movementController;

    [SerializeField] private PlayerStats meleeStats;
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private Collider2D col;
    [SerializeField] private PlayerAnimatorController playerAnimatorController;
    [SerializeField] private SpriteRenderer playerShellSpriteRenderer;

    [SerializeField] private int health;
    [SerializeField] private int armor;
    [SerializeField] private Vector2 speed;
    [SerializeField] private Sprite shell;
    [SerializeField] private Sprite damagedShell;

    [SerializeField] private bool grounded;
    [SerializeField] public bool frontClear;
    [SerializeField] private bool hiding;


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
            }
        });
        _playerInputActions.Player.Move.canceled += Idle;
        _playerInputActions.Player.PickUpShell.started += SwitchShells;

        _playerInputActions.Player.Hide.started += Hide;
        _playerInputActions.Player.Hide.canceled += Hide;
        
        _playerInputActions.Player.ChargeAttack.canceled += ChargeAttack;

        _playerInputActions.Player.Attack.canceled += Attack;
        _groundFilter2D = new ContactFilter2D
        {
            layerMask = LayerMask.GetMask("Ground"),
            useLayerMask = true
        };

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
        if (grounded)
        {
            playerAnimatorController.SetIsGrounded(true);
            _playerInputActions.Player.Hide.Enable();
        }
        else
        {
            playerAnimatorController.SetIsGrounded(false);
            _playerInputActions.Player.Hide.Disable();
        }
        frontClear = _movementController.FrontClear();
        Move();
        if (_attackCommand != null)
        {
            if (_attackCommand.LockInput)
            {
                _playerInputActions.Player.Disable();
            }
            else
            {
                _playerInputActions.Player.Enable();
            }
        }
    }

    public void SetStats(PlayerStats playerStats)
    {
        armor = playerStats.Armor;
        speed = playerStats.Speed;
        _movementController.WalkingSpeed = playerStats.MaxSpeed.x;
        _attackCommand = playerStats.Attack.MakeAttack();

        //Update UI each time stats are changed.
        HUDManager.Instance.UpdateHealth(health);
        HUDManager.Instance.UpdateArmor(armor);
    }
    private void Move()
    {
        var horizontal = _playerInputActions.Player.Move.ReadValue<float>();
        var horizontalVelocity = horizontal * speed.x;
        _movementController.MoveHorizontally(horizontalVelocity);
        if (horizontalVelocity != 0)
        {
            playerAnimatorController.SetIsMoving(true);
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
            if (!_attackCommand.IsRunning)
            {
                StartCoroutine(_attackCommand.DoAttack(gameObject));
                pSoundManager.PlaySound(pSoundManager.Sound.pAttack);
            }
        }
    }

    private void ChargeAttack(InputAction.CallbackContext context)
    {

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
                SetStats(newShell.GetComponent<ShellScript>().playerStats);
                shell = newShell.GetComponent<ShellScript>().shell;
                damagedShell = newShell.GetComponent<ShellScript>().damagedShell;
                playerShellSpriteRenderer.sprite = collider2D[0].GetComponent<SpriteRenderer>().sprite;
                newShell.transform.parent = gameObject.transform;
                newShell.SetActive(false);
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

    private void DropShell()
    {
        if (transform.childCount > 1)
        {
            var oldShell = transform.GetChild(1);
            oldShell.gameObject.SetActive(true);
            oldShell.SetParent(null);
            oldShell.GetComponent<SpriteRenderer>().sprite = playerShellSpriteRenderer.sprite;
            oldShell.localScale = new Vector3(transform.localScale.x, oldShell.localScale.y, oldShell.localScale.z);
            oldShell.GetComponent<ShellScript>().armor = armor;
        }
    }
    private void Hide(InputAction.CallbackContext context)
    {
        //Call playerAnimatorController
        if (context.started)
        {
            _playerInputActions.Player.Jump.Disable();
            hiding = true;
            _movementController.WalkingSpeed *= 0.5f;
            speed *= 0.5f;
            playerAnimatorController.SetIsHiding(true);
            pSoundManager.PlaySound(pSoundManager.Sound.pHide);
        }
        else if (context.canceled)
        {
            _playerInputActions.Player.Jump.Enable();
            hiding = false;
            _movementController.WalkingSpeed *= 2f;
            speed *= 2f;
            playerAnimatorController.SetIsHiding(false);
        }
    }

    public void TakeDamage(Damage dmg)
    {
        var direction = Math.Sign(transform.position.x - dmg.Source.x);
        //Only do the 'Got hit from behind?' thing if you actually have armor left
        //I may just not have understood how shell armor was supposed to work, given that player also gets more health from the shell but when the shell breaks, player's health returns to pre-shell.
        //TODO: Check this behaviour later  ^^^
        if (armor > 0)
        {
            if (direction == Math.Sign(transform.localScale.x))
            {
                health -= dmg.RawDamage;
                HUDManager.Instance.UpdateHealth(Mathf.Max(0, health));
                if (health <= 0)
                {
                    Death();
                }
                Debug.Log("Lose Health");
            }
            else
            {
                // todo: make sure the player takes damage if their armour breaks
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
        }
        else
        {
            health -= dmg.RawDamage;
            HUDManager.Instance.UpdateHealth(Mathf.Max(0, health));
            if (health <= 0)
            {
                Death();
            }
            Debug.Log("Lose Health");
        }
        pSoundManager.PlaySound(pSoundManager.Sound.pHit);
        StartCoroutine(Invulnerable());
        //Only do the Knockback coroutine if knockback on dmg isn't 0, so player doesn't come to a full stop for a moment if knockback is 0.
        if (dmg.Knockback != 0)
        {
            StartCoroutine(_movementController.Knockback(dmg));
        }
    }

    private void Death()
    {
        pSoundManager.PlaySound(pSoundManager.Sound.pDie);
        //Tell MapManager the player died, it handles respawn and such.
        if (MapManager.Instance)
        {
            MapManager.Instance.PlayerDied();
        }
        //Perform other death tasks
        Destroy(gameObject);
    }

    private void BreakShell()
    {
        playerShellSpriteRenderer.sprite = null;
        shell = null;
        damagedShell = null;
        //TODO: Check out this thing cus it's pumping out errors sometimes, this if is just so that it doesn't do that and it actually Sets Stats.
        if (transform.childCount > 1)
        {
            Destroy(transform.GetChild(1).gameObject);
        }
        SetStats(meleeStats);
        //switch to melee
    }
    
    private IEnumerator Invulnerable()
    {
        gameObject.layer = LayerMask.NameToLayer("Invulnerable");
        yield return new WaitForSeconds(1);
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        switch (LayerMask.LayerToName(other.collider.gameObject.layer))
        {
            case "Enemy":
                // todo: add variable for these properties
                //var dmg = new Damage(transform.position, 20, 1);
                //other.gameObject.GetComponent<IDamageable>().TakeDamage(dmg);
                break;
        }
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
                TakeDamage(new Damage(transform.position, 0, 1));
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