using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private Collider2D col;
    [SerializeField] private PlayerAnimatorController playerAnimatorController;

    public float speed;
    public float maxSpeed;

    public float jumpSpeed;
    // Start is called before the first frame update
    private void Awake() {
        playerInputActions = new PlayerInputActions();
    }

    private void OnEnable() {
        playerInputActions.Enable();
    }

    private void OnDisable() {
        playerInputActions.Disable();
    }
    void Start()
    {
        playerInputActions.Player.Jump.started += Jump;
        playerInputActions.Player.Move.started += Rotate;
        playerInputActions.Player.Move.canceled += Idle;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move(){
        var horizontal = playerInputActions.Player.Move.ReadValue<float>();
        var horizontalVelocity = horizontal * speed;
        if (Mathf.Abs(rigidBody.velocity.x) < maxSpeed)
        {
            rigidBody.AddForce(new Vector2(horizontalVelocity, 0), ForceMode2D.Impulse);
            playerAnimatorController.SetIsMoving(true);
        }
        
    }

    private void Rotate(InputAction.CallbackContext context)
    {
        var direction = context.ReadValue<float>();
        if (direction > 0 )
        {
            transform.localScale = new Vector3(-1, transform.localScale.y,
                transform.localScale.z);
        }else if (direction < 0)
        {
            transform.localScale = new Vector3(1, transform.localScale.y,
                transform.localScale.z);
        }
    }
    private void Idle(InputAction.CallbackContext context)
    {
        rigidBody.velocity = new Vector2(0,rigidBody.velocity.y);
        playerAnimatorController.SetIsMoving(false);
        //Play Idle animation
        Debug.Log("Idle");
    }
    private void Jump(InputAction.CallbackContext context)
    {
        if (Grounded())
        {
            rigidBody.AddRelativeForce(new Vector2(0,jumpSpeed), ForceMode2D.Impulse);
            Debug.Log("Jump");
        }
    }
    private void Attack()
    {
        
    }

    private bool Grounded()
    {
        RaycastHit2D[] hit = new RaycastHit2D[1];
        if (col.Raycast(Vector2.down,hit, 1, LayerMask.GetMask("Ground")) > 0)
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
    }
}
