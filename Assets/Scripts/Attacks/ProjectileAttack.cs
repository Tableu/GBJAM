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
        public Vector2 offset;

        public override AttackCommand MakeAttack()
        {
            return new Attack(projectileSpeed, projectilePrefab, windupTime, cooldownTime, offset);
        }
        
        private class Attack : AttackCommand
        {
            private float _speed;
            private GameObject _projectilePrefab;
            private Vector2 _offset;
            private float _windupTime;
            private float _cooldownTime;

            public Attack(float projectileSpeed, 
                GameObject projectilePrefab, 
                float windupTime, 
                float cooldownTime, 
                Vector2 offset)
            {
                _speed = projectileSpeed;
                _windupTime = windupTime;
                _cooldownTime = cooldownTime;
                _offset = offset;
                _projectilePrefab = projectilePrefab;
            }

            public bool IsRunning { get; private set; }
            public bool LockInput { get; private set; }

            public IEnumerator DoAttack(GameObject attacker)
            {
                IsRunning = true;

                yield return new WaitForSeconds(_windupTime);

                var transform = attacker.GetComponent<Transform>();
                var dir = -1 * Mathf.Sign(transform.localScale.x);
                var pos = transform.position + dir * _offset.x * Vector3.right + Vector3.up * _offset.y;
                var projectile = Instantiate(_projectilePrefab, pos, Quaternion.identity);
                projectile.transform.localScale = transform.localScale;
                projectile.GetComponent<Rigidbody2D>().velocity =
                    new Vector2(_speed * (-1) * transform.localScale.x, 0);
                yield return new WaitForSeconds(_cooldownTime);
                IsRunning = false;
            }
        }
    }
}