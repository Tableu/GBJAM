using Enemies;
using UnityEngine;


public class SnailEnemy : EnemyBase
{
    [SerializeField] public float lookAheadDist = 0.5f;

    protected new void Awake()
    {
        base.Awake();
        StateMachine = new FSM();
        var falling = new FallState(this);
        var patrol = new PatrolPlatform(this);
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
            var targetPos = _enemy.PlayerTransform.position;
            var dir = Mathf.Sign( (_enemy.transform.position - targetPos).x);
            _enemy.CurrentVelocity = Vector2.right*dir*_enemy.walkingSpeed;
        }

        public void OnEnter()
        {
            _enemy.CurrentVelocity = Vector2.zero;
        }

        public void OnExit()
        {
        }
    }
}
