using System;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    [System.Serializable]
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
    [SerializeField] private bool hiding;

    // Start is called before the first frame update
    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
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
        
        _groundFilter2D = new ContactFilter2D
        {
            layerMask = LayerMask.GetMask("Ground"),
            useLayerMask = true
        };
    }

    // Update is called once per frame
    void Update()
    {
        Grounded();
        Move();
    }

    public void SetStats(PlayerStats playerStats)
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
            Debug.Log("Jump");
        }
    }
    private void Attack()
    {
        switch (powerUp)
        {
            case AttackCommands.SIMPLE_PROJECTILE_ATTACK:
                break;
            case AttackCommands.MELEE_ATTACK:
                break;
            case AttackCommands.DASH:
                break;
        }
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
    }

    private void Hide(InputAction.CallbackContext context)
    {
        //Call playerAnimatorController
        if (context.started)
        {
            _playerInputActions.Player.Move.Disable();
            _playerInputActions.Player.Jump.Disable();
            hiding = true;
            playerAnimatorController.SetIsHiding(true);
        }
        else if (context.canceled)
        {
            _playerInputActions.Player.Move.Enable();
            _playerInputActions.Player.Jump.Enable();
            hiding = false;
            playerAnimatorController.SetIsHiding(false);
        }
    }

    public void TakeDamage(AttackCommands.AttackStats attackStats, Transform otherPos)
    {
        var direction = Math.Sign(transform.position.x - otherPos.position.x);
        if (direction == Math.Sign(transform.localScale.x))
        {
            health -= attackStats.Damage;
            if(health <= 0){Death();}
            Debug.Log("Lose Health");
        }
        else
        {
            armor -= attackStats.Damage;
            if (armor <= 0) {BreakShell();}
            Debug.Log("Lose Armor");
        }
        rigidBody.AddRelativeForce(new Vector2(attackStats.Knockback.x*direction, attackStats.Knockback.y), ForceMode2D.Impulse);
        StartCoroutine(Invulnerable());
    }

    private void Death()
    {
        //Perform other death tasks
        Destroy(gameObject);
    }

    private void BreakShell()
    {
        //remove shell sprite
    }

    private IEnumerator Invulnerable()
    {
        gameObject.layer = LayerMask.NameToLayer("Invulnerable");
        yield return new WaitForSeconds(1);
        gameObject.layer = LayerMask.NameToLayer("Player");
    }
    private bool Grounded()
    {
        RaycastHit2D[] hit = new RaycastHit2D[1];
        if (col.Raycast(Vector2.down, hit, 1, LayerMask.GetMask("Ground")) > 0)
        {
            playerAnimatorController.SetIsGrounded(true);
            _playerInputActions.Player.Hide.Enable();
            grounded = true;
            return true;
        }
        playerAnimatorController.SetIsGrounded(false);
        _playerInputActions.Player.Hide.Disable();
        grounded = false;
        return false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        switch (LayerMask.LayerToName(other.collider.gameObject.layer))
        {
            case "Enemy":
                other.gameObject.GetComponent<AttackController>().Hit(gameObject);
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
