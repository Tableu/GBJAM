using Enemies;
using UnityEngine;


public class SnailEnemy : EnemyBase
{
    [SerializeField] public float lookAheadDist = 0.5f;
    protected float timeSinceSawPlayer = 0;


    protected new void Awake()
    {
        base.Awake();
        MovementManager = new MovementManager(gameObject, collisionLayers);
        StateMachine = new FSM();
        var falling = new FallState(this);
        var patrol = new Patrol(this);
        var attack = new Attack(this);

        StateMachine.AddTransition(falling, patrol, 
            () => MovementManager.MovementFlags.HasFlag(MovementManager.Flags.Grounded));
        StateMachine.AddAnyTransition(falling, 
            () => !MovementManager.MovementFlags.HasFlag(MovementManager.Flags.Grounded));
        StateMachine.AddTransition(patrol, attack, PlayerVisible);
        StateMachine.AddTransition(attack, patrol, () => timeSinceSawPlayer > attackTime);
        StateMachine.SetState(falling);
    }

    private class Attack : IState
    {
        private readonly SnailEnemy _enemy;

        public Attack(SnailEnemy enemy)
        {
            _enemy = enemy;
        }

        public void Tick()
        {
            // todo: move this logic to the base class
            if (_enemy.PlayerVisible())
            {
                _enemy.timeSinceSawPlayer = 0;
            }
            else
            {
                _enemy.timeSinceSawPlayer += Time.deltaTime;
            }
        }

        public void OnEnter()
        {
            _enemy.CurrentVelocity = Vector2.zero;
            Debug.Log("Attack Mode");
        }

        public void OnExit()
        {
            Debug.Log("Leaving Attack Mode");
        }
    }
    
    private class Patrol: IState
    {
        private readonly SnailEnemy _enemy;

        public Patrol(SnailEnemy enemy)
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
