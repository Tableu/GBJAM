using System.Collections;
using UnityEngine;

namespace Attacks
{
    [CreateAssetMenu(fileName = "Attack", menuName = "Attacks/Melee", order = 0)]
    public class MeleeAttack : AttackScriptableObject
    {
        public int meleeDamage;
        public float windupTime;
        public float cooldownTime;
        public override AttackCommand MakeAttack()
        {
            return new Attack(meleeDamage, windupTime, cooldownTime);
        }

        private class Attack : AttackCommand
        {
            private int _damage;
            private float _windupTime;
            private float _cooldownTime;
            
            public Attack(int damage, float windupTime, float cooldownTime)
            {
                _damage = damage;
                _windupTime = windupTime;
                _cooldownTime = cooldownTime;
            }

            public bool IsRunning { get; private set; }
            public bool LockInput { get; private set; }

            public IEnumerator DoAttack(GameObject attacker)
            {
                IsRunning = true;
                yield return new WaitForSeconds(_windupTime);
                yield return new WaitForSeconds(_cooldownTime);
                IsRunning = false;
            }
        }
    }
}
