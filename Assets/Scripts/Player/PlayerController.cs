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
        public PlayerStats(Vector2 speed, Vector2 maxSpeed, int health, int armor, string powerUp)
        {
            _speed = speed;
            _maxSpeed = maxSpeed;
            _health = health;
            _armor = armor;
            _powerUp = powerUp;
        }

        [SerializeField] private Vector2 _speed;
        [SerializeField] private Vector2 _maxSpeed;
        [SerializeField] private int _health;
        [SerializeField] private int _armor;
        [SerializeField] private String _powerUp;
        public Vector2 Speed => _speed;
        public Vector2 MaxSpeed => _maxSpeed;
        public int Health => _health;
        public int Armor => _armor;
        public string PowerUp => _powerUp;
    }
    private PlayerInputActions _playerInputActions;
    private ContactFilter2D _groundFilter2D;
    private AttackCommands _attackCommands;

    [SerializeField] private GameObject projectile;
    [SerializeField] private AttackCommands.AttackStats _attackStats;
    private float dashStart;
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private Collider2D col;
    [SerializeField] private PlayerAnimatorController playerAnimatorController;
    [SerializeField] private SpriteRenderer playerShellSpriteRenderer;

    [SerializeField] private int health;
    [SerializeField] private int armor;
    [SerializeField] private Vector2 speed;
    [SerializeField] private Vector2 maxSpeed;
    [SerializeField] private string powerUp;

    [SerializeField] private bool grounded;
    [SerializeField] private bool frontClear;
    [SerializeField] private bool hiding;

    [SerializeField] private bool dash;

    [SerializeField, Range(0, 1f)] private float knockBackDuration = 0.25f;
    private bool isInKnockback = false;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _attackCommands = new AttackCommands();
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
        _playerInputActions.Player.Jump.started += Jump;
        _playerInputActions.Player.Move.started += Rotate;
        _playerInputActions.Player.Move.canceled += Idle;
        _playerInputActions.Player.PickUpShell.started += SwitchShells;

        _playerInputActions.Player.Hide.started += Hide;
        _playerInputActions.Player.Hide.canceled += Hide;

        //_playerInputActions.Player.ChargeAttack.started += ChargeAttack;
        //_playerInputActions.Player.ChargeAttack.performed += ChargeAttack;
        _playerInputActions.Player.ChargeAttack.canceled += ChargeAttack;

        _playerInputActions.Player.Attack.canceled += Attack;
        _groundFilter2D = new ContactFilter2D
        {
            layerMask = LayerMask.GetMask("Ground"),
            useLayerMask = true
        };
        dash = false;

        CinemachineVirtualCamera virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCamera)
        {
            virtualCamera.Follow = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Grounded();
        frontClear = FrontClear();
        if (!hiding)
        {
            Move();
        }

        if (dash)
        {
            if (!_attackCommands.Dash(gameObject, _attackStats, dashStart) || !frontClear)
            {
                dash = false;
                _playerInputActions.Player.Move.Enable();
            }
        }
    }

    public void SetStats(PlayerStats playerStats, AttackCommands.AttackStats attackStats)
    {
        health = playerStats.Health;
        armor = playerStats.Armor;
        speed = playerStats.Speed;
        maxSpeed = playerStats.MaxSpeed;
        powerUp = playerStats.PowerUp;
    }
    private void Move()
    {
        var horizontal = _playerInputActions.Player.Move.ReadValue<float>();
        var horizontalVelocity = horizontal * speed.x;

        if (!grounded)
        {
            if (col.IsTouching(_groundFilter2D))
            {
                return;
            }
        }
        if (horizontalVelocity != 0)
        {
            if (Mathf.Abs(rigidBody.velocity.x) < maxSpeed.x)
            {
                rigidBody.AddForce(new Vector2(horizontalVelocity, 0), ForceMode2D.Impulse);
                playerAnimatorController.SetIsMoving(true);
            }
        }
        else
        {
            if (!isInKnockback && grounded)
            {
                //rigidBody.velocity = new Vector2(Mathf.Lerp(rigidBody.velocity.x, 0, Something???), rigidBody.velocity.y);
                //To avoid the player from sliding after knockback
                rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
            }
        }
    }

    private void Rotate(InputAction.CallbackContext context)
    {
        var direction = context.ReadValue<float>();
        var scale = transform.localScale;
        if (direction > 0)
        {
            transform.localScale = new Vector3(-1, scale.y, scale.z);
        }
        else if (direction < 0)
        {
            transform.localScale = new Vector3(1, scale.y, scale.z);
        }
    }
    private void Idle(InputAction.CallbackContext context)
    {
        rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
        playerAnimatorController.SetIsMoving(false);
        Debug.Log("Idle");
    }
    private void Jump(InputAction.CallbackContext context)
    {
        if (grounded)
        {
            playerAnimatorController.TriggerJump();
            rigidBody.AddRelativeForce(new Vector2(0, speed.y), ForceMode2D.Impulse);
            pSoundManager.PlaySound(pSoundManager.Sound.pJump);
            Debug.Log("Jump");
        }
    }
    private void Attack(InputAction.CallbackContext context)
    {
        if (context.duration < 1)
        {
            Debug.Log("Attack");
            switch (powerUp)
            {
                case AttackCommands.SIMPLE_PROJECTILE_ATTACK:
                    _attackCommands.SimpleProjectileAttack(gameObject, projectile, _attackStats);
                    break;
                case AttackCommands.MELEE_ATTACK:
                    break;
                case AttackCommands.DASH:
                    dashStart = transform.position.x;
                    if (_attackCommands.Dash(gameObject, _attackStats, dashStart))
                    {
                        dash = true;
                        _playerInputActions.Player.Move.Disable();
                    }
                    break;
            }
        }
        pSoundManager.PlaySound(pSoundManager.Sound.pAttack);
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
        if (Physics2D.OverlapCollider(col, contactFilter2D, collider2D) == 1 && grounded)
        {
            collider2D[0].gameObject.GetComponent<ShellScript>().AttachedToPlayer(gameObject);
        }
        pSoundManager.PlaySound(pSoundManager.Sound.pPickup);
    }

    private void Hide(InputAction.CallbackContext context)
    {
        //Call playerAnimatorController
        if (context.started)
        {
            _playerInputActions.Player.Jump.Disable();
            rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
            hiding = true;
            playerAnimatorController.SetIsHiding(true);
            pSoundManager.PlaySound(pSoundManager.Sound.pHide);
        }
        else if (context.canceled)
        {
            _playerInputActions.Player.Jump.Enable();
            hiding = false;
            playerAnimatorController.SetIsHiding(false);
        }
    }

    public void TakeDamage(Damage dmg)
    {
        var dir = ((Vector2)transform.position - dmg.Source).normalized;
        var direction = Math.Sign(dir.x);
        if (direction == Math.Sign(transform.localScale.x))
        {
            health -= dmg.RawDamage;
            HUDManager.Instance.UpdateHealth(health);
            if (health <= 0) { Death(); }
            Debug.Log("Lose Health");
        }
        else
        {
            armor -= dmg.RawDamage;
            HUDManager.Instance.UpdateArmor(armor);
            if (armor <= 0) { BreakShell(); }
            Debug.Log("Lose Armor");
        }
        pSoundManager.PlaySound(pSoundManager.Sound.pHit);
        var knockbackForce = dir * dmg.Knockback;
        rigidBody.velocity = Vector2.zero;
        rigidBody.AddForce(knockbackForce, ForceMode2D.Impulse);
        StartCoroutine(KnockbackCoroutine());
        StartCoroutine(Invulnerable());
    }

    private void Death()
    {
        pSoundManager.PlaySound(pSoundManager.Sound.pDie);
        //Perform other death tasks
        Destroy(gameObject);
    }

    private void BreakShell()
    {
        //remove shell sprite
    }

    private IEnumerator KnockbackCoroutine()
    {
        isInKnockback = true;
        yield return new WaitForSeconds(knockBackDuration);
        isInKnockback = false;
    }
    private IEnumerator Invulnerable()
    {
        gameObject.layer = LayerMask.NameToLayer("Invulnerable");
        yield return new WaitForSeconds(1);
        gameObject.layer = LayerMask.NameToLayer("Player");
    }
    private bool Grounded()
    {
        RaycastHit2D hit;
        Vector2[] posArray = {new Vector2(col.bounds.max.x,col.bounds.min.y),
            new Vector2(col.bounds.center.x,col.bounds.min.y),
            new Vector2(col.bounds.min.x,col.bounds.min.y)};
        for (int x = 0; x < 3; x++)
        {
            hit = Physics2D.Raycast(posArray[x], Vector2.down, 0.3f, LayerMask.GetMask("Ground"));
            Debug.DrawRay(posArray[x], new Vector2(0, -0.3f), Color.red);
            if (hit.collider != null)
            {
                playerAnimatorController.SetIsGrounded(true);
                _playerInputActions.Player.Hide.Enable();
                return true;
            }
        }
        playerAnimatorController.SetIsGrounded(false);
        _playerInputActions.Player.Hide.Disable();
        return false;
    }

    private bool FrontClear()
    {
        RaycastHit2D[] hit = new RaycastHit2D[1];
        if (col.Raycast(new Vector2(transform.localScale.x * (-1), 0), hit, 1, LayerMask.GetMask("Ground")) > 0)
        {
            return false;
        }
        return true;
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        switch (LayerMask.LayerToName(other.collider.gameObject.layer))
        {
            case "Enemy":
                // todo: add variable for these properties
                var dmg = new Damage(transform.position, 20, 1);
                other.gameObject.GetComponent<IDamageable>().TakeDamage(dmg);
                break;
        }

        if (dash)
        {
            dash = false;
            _playerInputActions.Player.Move.Enable();
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