using Enemies;
using UnityEngine;


public class PlatformEnemy : EnemyBase
{
    [SerializeField] public float lookAheadDist = 0.5f;

    protected new void Awake()
    {
        base.Awake();
        MovementManager = new MovementManager(gameObject, collisionLayers);
        StateMachine = new FSM();
        var falling = new FallState(this);
        var patrol = new Patrol(this);

        StateMachine.AddTransition(falling, patrol, 
            () => MovementManager.MovementFlags.HasFlag(MovementManager.Flags.Grounded));
        StateMachine.AddAnyTransition(falling, 
            () => !MovementManager.MovementFlags.HasFlag(MovementManager.Flags.Grounded));
        StateMachine.SetState(falling);
    }

    private class Patrol: IState
    {
        private readonly PlatformEnemy _enemy;

        public Patrol(PlatformEnemy enemy)
        {
            _enemy = enemy;
        }

        public void Tick()
        {
            var t = _enemy.transform;
            var rayOrigin = new Vector2(t.position.x + _enemy.lookAheadDist * t.localScale.x, 
                                        _enemy.MovementManager.Bounds.min.y);
            var hit = Physics2D.Raycast(rayOrigin, Vector2.down, 0.5f, _enemy.collisionLayers);
            Debug.DrawRay(rayOrigin, Vector2.down);
            if (!hit)
            {
                _enemy.CurrentVelocity.x = -_enemy.CurrentVelocity.x;
            }
        }

        public void OnEnter()
        {
            _enemy.CurrentVelocity.x = _enemy.walkingSpeed;
        }

        public void OnExit()
        {
        }
    }
}
