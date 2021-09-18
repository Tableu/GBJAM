using UnityEngine.InputSystem;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    public readonly struct PlayerStats
    {
        public PlayerStats(Vector2 speed, Vector2 maxSpeed, int health, int armor, string powerup)
        {
            Speed = speed;
            MaxSpeed = maxSpeed;
            Health = health;
            Armor = armor;
            Powerup = powerup;
        }
        public Vector2 Speed { get; }
        public Vector2 MaxSpeed { get; }
        public int Health { get; }
        public int Armor { get; }
        public string Powerup { get; }
    }
    private PlayerInputActions playerInputActions;
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private Collider2D col;
    [SerializeField] private PlayerAnimatorController playerAnimatorController;
    [SerializeField] private SpriteRenderer playerShellSpriteRenderer;

    [SerializeField] private int health;
    [SerializeField] private int armor;
    [SerializeField] private Vector2 speed;
    [SerializeField] private Vector2 maxSpeed;
    [SerializeField] private string powerup;

    // Start is called before the first frame update
    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }
    void Start()
    {
        playerInputActions.Player.Jump.started += Jump;
        playerInputActions.Player.Move.started += Rotate;
        playerInputActions.Player.Move.canceled += Idle;
        playerInputActions.Player.PickUpShell.started += SwitchShells;
        playerInputActions.Player.Hide.started += Hide;
        playerInputActions.Player.Hide.canceled += Hide;
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
        powerup = playerStats.Powerup;
    }
    private void Move()
    {
        var horizontal = playerInputActions.Player.Move.ReadValue<float>();
        var horizontalVelocity = horizontal * speed.x;
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
            Idle(new InputAction.CallbackContext());
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
        if (Grounded())
        {
            playerAnimatorController.TriggerJump();
            rigidBody.AddRelativeForce(new Vector2(0, speed.y), ForceMode2D.Impulse);
            Debug.Log("Jump");
        }
    }
    private void Attack()
    {
        switch (powerup)
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
        if (Physics2D.OverlapCollider(col, contactFilter2D, collider2D) == 1 && Grounded())
        {
            collider2D[0].gameObject.GetComponent<ShellScript>().AttachedToPlayer(gameObject);
        }
    }

    private void Hide(InputAction.CallbackContext context)
    {
        //Call playerAnimatorController
        if (context.started)
        {
            playerInputActions.Player.Move.Disable();
            playerInputActions.Player.Jump.Disable();
            playerAnimatorController.SetIsHiding(true);
        }
        else if (context.canceled)
        {
            playerInputActions.Player.Move.Enable();
            playerInputActions.Player.Jump.Enable();
            playerAnimatorController.SetIsHiding(false);
        }
    }
    private bool Grounded()
    {
        RaycastHit2D[] hit = new RaycastHit2D[1];
        if (col.Raycast(Vector2.down, hit, 1, LayerMask.GetMask("Ground")) > 0)
        {
            playerAnimatorController.SetIsGrounded(true);
            return true;
        }
        playerAnimatorController.SetIsGrounded(false);
        return false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Grounded();
        }
        //if other.collider is in the Enemy layer lose armor or health based on damage script
    }

    //private void OnCollisionExit(Collision other)
    //{
    //    if (other.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
    //    {
    //        Grounded();
    //    }
    //}
}
