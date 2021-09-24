using Enemies;
using UnityEngine;

public class PlatformEnemy : EnemyBase
{
    [Header("Platform Enemy Config")] [SerializeField]
    public float lookAheadDist = 0.5f;

    private BoxCollider2D _collider;

    protected new void Start()
    {
        base.Awake();
        _collider = GetComponent<BoxCollider2D>();
        StateMachine = new FSM();
        var falling = new FallState(this);
        var patrol = new PatrolPlatform(this, MovementController);
        var attack = new RangedAttack(this, MovementController, PlayerTransform);

        StateMachine.AddTransition(falling, patrol,
            () => MovementController.Grounded());
        StateMachine.AddAnyTransition(falling,
            () => !MovementController.Grounded());
        StateMachine.AddTransition(patrol, attack, PlayerVisible);
        StateMachine.AddTransition(attack, patrol, () => timeSinceSawPlayer > attackTime);
        StateMachine.SetState(falling);
    }

    public bool AtPlatformEdge()
    {
        var bounds = _collider.bounds;
        var rayOrigin = new Vector2(bounds.center.x - lookAheadDist * transform.localScale.x, bounds.min.y);
        var hit = Physics2D.Raycast(rayOrigin, Vector2.down, 0.5f, groundLayer);
        return !hit;
    }
}