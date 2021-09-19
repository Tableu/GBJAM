using System;
using System.IO;
using UnityEngine;

namespace Enemies
{
    public abstract class EnemyBase : MonoBehaviour, IDamageable
    {
        // todo: add getters and setters for public fields
        [SerializeField] protected float gravity;

        [SerializeField] public float walkingSpeed;

        [SerializeField] public LayerMask collisionLayers;

        [SerializeField] private int maxHealth;

        [SerializeField] private float knockbackFactor;

        [Range(10, 360)]
        [SerializeField] private float fieldOfView;

        [SerializeField] private float visionRange;
        [SerializeField] private float detectionRange;
        [SerializeField] protected float attackTime = 5;

        [SerializeField] private LayerMask sightBlockingLayers;

        [NonSerialized] public MovementManager MovementManager;
        
        [NonSerialized] public Vector2 CurrentVelocity = Vector2.zero;
        
        protected Vector2 Forward => Vector2.right*transform.localScale.x;
        protected float timeSinceSawPlayer = 0;

        protected FSM StateMachine;

        protected Transform PlayerTransform;
        protected PlayerController Player;
        
        private int _currentHealth;
        private LayerMask _playerLayer;

        protected void Awake()
        {
            MovementManager = new MovementManager(gameObject, collisionLayers);
            StateMachine = new FSM();
            _currentHealth = maxHealth;
            _playerLayer = LayerMask.GetMask("Player");
            var playerGO = GameObject.FindWithTag("Player");
            PlayerTransform = playerGO.transform;
            Player = playerGO.GetComponent<PlayerController>();
        }

        protected void Update()
        {
            CurrentVelocity.y -= gravity;
            MovementManager.Move(CurrentVelocity*Time.deltaTime);
            
            if (MovementManager.MovementFlags.HasFlag(MovementManager.Flags.Grounded))
            {
                CurrentVelocity.y = 0;
            }
            LookForPlayer();
            StateMachine.Tick();
        }
        
        protected bool PlayerVisible()
        {
            // Check if player is in fov
            Vector2 playerPos = PlayerTransform.position - transform.position;
            var angle = Vector2.Angle(Forward, playerPos);
            var distance = playerPos.magnitude;
            if (angle <= fieldOfView/2 && distance <= visionRange || distance < detectionRange)
            {
                // Check for line of sight
                var hit = Physics2D.Raycast(transform.position, playerPos, distance + 1, sightBlockingLayers);
                if (hit.transform == PlayerTransform)
                {
                    return true;
                }
            }
            return false;
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

        private void LookForPlayer()
        {
            if (PlayerVisible())
            {
                timeSinceSawPlayer = 0;
            }
            else
            {
                timeSinceSawPlayer += Time.deltaTime;
            }
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
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject != Player.gameObject) return;
            Vector2 direction = PlayerTransform.position - transform.position;
            // todo: add variable for these properties
            var dmg = new Damage(direction, 20, 1);
            Player.TakeDamage(dmg);
        }

        public void TakeDamage(Damage dmg)
        {
            // todo: damage cooldown
            Debug.Log(_currentHealth);
            _currentHealth -= dmg.RawDamage;
            // kill if zero health
            if (_currentHealth <= 0)
            {
                // todo: add death state to play animation
                Destroy(gameObject);
                return;
            }
            // apply knockback
            var knockback = dmg.Direction*dmg.Knockback*knockbackFactor;
            MovementManager.Move(knockback);
        }
    }
}