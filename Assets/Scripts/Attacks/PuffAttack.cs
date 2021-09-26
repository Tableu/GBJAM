using System;
using System.Collections;
using Enemies;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Attacks/Puff", order = 0)]
public class PuffAttack : AttackScriptableObject
{
    public float attackDuration;
    public float puffRange;
    public float cooldown;
    public int damage;
    public int knockback;
    public float windup;
    
    public override AttackCommand MakeAttack()
    {
        return new Attack(attackDuration, cooldown, damage, knockback, windup, puffRange);
    }

    private class Attack : AttackCommand
    {
        private readonly float _attackDuration;
        private readonly float _cooldown;
        private readonly int _damage;
        private readonly int _knockback;
        private readonly float _windup;
        private readonly float _puffRange;

        public Attack(float attackDuration, float cooldown, int damage, int knockback, float windup, float puffRange)
        {
            _attackDuration = attackDuration;
            _cooldown = cooldown;
            _damage = damage;
            _knockback = knockback;
            _windup = windup;
            _puffRange = puffRange;
        }

        public IEnumerator DoAttack(GameObject attacker)
        {
            // Wind up
            IsRunning = true;
            var animator = attacker.GetComponentInChildren<EnemyAnimatorController>();
            var enemy = attacker.GetComponent<EnemyBase>();
            animator.IsAngry(true);
            yield return new WaitForSeconds(_windup);
            // Puff
            pSoundManager.PlaySound(pSoundManager.Sound.puffUp);
            var oldKnock = enemy.collisionKnockback;
            var oldDmg = enemy.collisionDamage;
            enemy.collisionDamage = _damage;
            enemy.collisionKnockback = _knockback;

            var player = GameObject.FindWithTag("Player");
            Vector2 playerPos = player.transform.position;
            Vector2 enemyPos = enemy.transform.position;
            var dist = (playerPos-enemyPos).magnitude;
            if (dist <= _puffRange)
            {
                player.GetComponent<IDamageable>().TakeDamage(new Damage(enemyPos, _knockback, _damage));
            }
            
            yield return new WaitForSeconds(_attackDuration);
            animator.IsAngry(false);
            enemy.collisionDamage = oldDmg;
            enemy.collisionKnockback = oldKnock;
            pSoundManager.PlaySound(pSoundManager.Sound.puffEnd);
            // De-puff
            yield return new WaitForSeconds(_cooldown);
            IsRunning = false;
        }

        public bool IsRunning { get; private set; }
        public bool LockInput { get; }
    }
}