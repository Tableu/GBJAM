using System.Collections;
using Enemies;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Attacks/Jump", order = 0)]
public class JumpAttack : AttackScriptableObject
{
    public float jumpAngle;
    public float attackRange;
    public float knockback;
    public int damage;
    public float windup;
    public float cooldown;

    public override AttackCommand MakeAttack()
    {
        return new Attack(jumpAngle, attackRange, knockback, damage, windup, cooldown);
    }

    private class Attack : AttackCommand
    {
        private const string TargetTag = "Player";
        private readonly float _cooldown;
        private readonly float _windup;
        private readonly int _damage;
        private readonly float _jumpAngle = 45f;
        private readonly float _jumpDistance = 5f;
        private readonly float _knockback;

        public Attack(float jumpAngle, float jumpDistance, float knockback, int damage, float windup, float cooldown)
        {
            _jumpAngle = jumpAngle;
            _jumpDistance = jumpDistance;
            _knockback = knockback;
            _damage = damage;
            _windup = windup;
            _cooldown = cooldown;
        }

        public IEnumerator DoAttack(GameObject attacker)
        {
            var targetGO = GameObject.FindWithTag(TargetTag);
            if (targetGO is null) yield break;
            var targetTransform = targetGO.transform;

            IsRunning = true;
            LockInput = true;
            var movement = attacker.GetComponent<EnemyBase>().MovementController;
            movement.SetMovementEnable(false);
            // Windup 
            yield return new WaitForSeconds(_windup);

            // Get Player location
            Vector2 target = targetTransform.position;


            // Calculate trajectory
            var rb = attacker.GetComponent<Rigidbody2D>();
            Vector2 attackerPos = attacker.transform.position;
            var theta = _jumpAngle * Mathf.Deg2Rad;
            var distance = target.x - attackerPos.x;
            var g = -Physics2D.gravity.y * rb.gravityScale;
            var dir = Mathf.Sign(distance);
            distance = Mathf.Abs(distance);
            distance = Mathf.Min(distance, _jumpDistance);
            var velMag = Mathf.Sqrt(distance * g / Mathf.Sin(2 * theta));
            var vel = velMag * new Vector2(dir * Mathf.Cos(theta), Mathf.Sin(theta));

            // Jump
            rb.velocity = vel;

            yield return new WaitForSeconds(0.1f);
            var collider = attacker.GetComponent<Collider2D>();
            while (!collider.IsTouchingLayers()) yield return null;

            var targetCollider = targetGO.GetComponent<Collider2D>();
            if (collider.IsTouching(targetCollider))
            {
                var damageable = targetGO.GetComponent<IDamageable>();
                damageable.TakeDamage(new Damage(attacker.transform.position, _knockback, _damage));
                var attackerDmg = attacker.GetComponent<IDamageable>();
                attackerDmg.TakeDamage(new Damage(targetTransform.position, _knockback, 0));
            }

            LockInput = false;
            movement.SetMovementEnable(true);
            yield return new WaitForSeconds(_cooldown);
            IsRunning = false;
        }

        public bool IsRunning { get; private set; }
        public bool LockInput { get; private set; }
    }
}