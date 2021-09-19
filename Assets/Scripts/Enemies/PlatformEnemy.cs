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
            var rayOrigin = t.position + t.right * _enemy.lookAheadDist * t.localScale.x;
            Debug.DrawRay(rayOrigin, Vector2.down);
    
            var hit = Physics2D.Raycast(rayOrigin, Vector2.down, 10, _enemy.collisionLayers);
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
            throw new System.NotImplementedException();
        }
    }
}
