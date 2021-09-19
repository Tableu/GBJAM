using System;
using System.IO;
using UnityEngine;

namespace Enemies
{
    public abstract class EnemyBase : MonoBehaviour, IDamageable
    {
        
        [SerializeField] protected float gravity;

        [SerializeField] protected float walkingSpeed;

        [SerializeField] protected LayerMask collisionLayers;

        [SerializeField] private int maxHealth;

        [SerializeField] private float knockbackFactor;

        protected MovementManager MovementManager;
        
        protected Vector2 CurrentVelocity = Vector2.zero;

        protected FSM StateMachine;

        private int _currentHealth;

        protected void Awake()
        {
            MovementManager = new MovementManager(gameObject, collisionLayers);
            StateMachine = new FSM();
            _currentHealth = maxHealth;
        }

        protected void Update()
        {
            CurrentVelocity.y -= gravity;
            MovementManager.Move(CurrentVelocity*Time.deltaTime);
            
            if (MovementManager.MovementFlags.HasFlag(MovementManager.Flags.Grounded))
            {
                CurrentVelocity.y = 0;
            }
            StateMachine.Tick();
        }

        protected class FallState: IState
        {
            private readonly EnemyBase _enemy;

            public FallState(EnemyBase enemy)
            {
                _enemy = enemy;
            }

            public void Tick() {}

            public void OnEnter(){}

            public void OnExit() => _enemy.CurrentVelocity.y = 0;
        }

        public void TakeDamage(AttackCommands.AttackStats attackStats, Transform otherPos)
        {
            _currentHealth -= attackStats.Damage;
            // kill if zero health
            if (_currentHealth <= 0)
            {
                Destroy(this);
                return;
            }
            // apply knockback
            var knockback = attackStats.Knockback * attackStats.Distance * knockbackFactor;
            MovementManager.Move(knockback);
        }
    }
}