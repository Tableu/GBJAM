using System.Collections;
using UnityEngine;

namespace Attacks
{
    [CreateAssetMenu(fileName = "Attack", menuName = "Attacks/Projectile", order = 0)]
    public class ProjectileAttack : AttackScriptableObject
    {
        public float projectileSpeed;
        public GameObject projectilePrefab;
        public float windupTime;
        public float cooldownTime;
        // todo: get from box collider
        public float horizontalOffset;

        public override AttackCommand MakeAttack()
        {
            return new Attack(projectileSpeed, projectilePrefab, windupTime, cooldownTime, horizontalOffset);
        }
        
        private class Attack : AttackCommand
        {
            private float _speed;
            private GameObject _projectilePrefab;
            private float _horizontalOffset;
            private float _windupTime;
            private float _cooldownTime;

            public Attack(float projectileSpeed, 
                GameObject projectilePrefab, 
                float windupTime, 
                float cooldownTime, 
                float horizontalOffset)
            {
                _speed = projectileSpeed;
                _windupTime = windupTime;
                _cooldownTime = cooldownTime;
                _horizontalOffset = horizontalOffset;
                _projectilePrefab = projectilePrefab;
            }

            public bool IsRunning { get; private set; }
            public bool LockInput { get; private set; }

            public IEnumerator DoAttack(GameObject attacker)
            {
                IsRunning = true;
                LockInput = true;


                yield return new WaitForSeconds(_windupTime);

                var transform = attacker.GetComponent<Transform>();
                var dir = -1 * Mathf.Sign(transform.localScale.x);
                var pos = transform.position + dir * _horizontalOffset * Vector3.right;
                var projectile = Instantiate(_projectilePrefab, pos, Quaternion.identity);
                projectile.transform.localScale = transform.localScale;
                projectile.GetComponent<Rigidbody2D>().velocity =
                    new Vector2(_speed * (-1) * transform.localScale.x, 0);
                yield return new WaitForSeconds(_cooldownTime);
                IsRunning = false;
                LockInput = false;
            }
        }
    }
}