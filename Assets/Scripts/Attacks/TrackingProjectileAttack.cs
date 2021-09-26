using System.Collections;
using Enemies;
using UnityEngine;


[CreateAssetMenu(fileName = "attack", menuName = "Attacks/TrackingProjectile", order = 0)]
public class TrackingProjectileAttack : AttackScriptableObject
{
    public float projectileSpeed;
    public GameObject projectilePrefab;
    public float windupTime;
    public Vector2 offset;
    public float cooldownTime;


    public override AttackCommand MakeAttack()
    {
        return new Attack(projectileSpeed, projectilePrefab, windupTime, cooldownTime, offset);
    }

    private class Attack : AttackCommand
    {
        private float _speed;
        private GameObject _projectilePrefab;
        private float _windupTime;
        private float _cooldownTime;
        private Vector2 _offset;

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

            pSoundManager.PlaySound(pSoundManager.Sound.eFireball);

            var playerTransform = attacker.GetComponent<EnemyBase>().PlayerTransform;
            var transform = attacker.transform;
            var distance = playerTransform.position - transform.position;
            Vector2 dir = distance.normalized;
            
            
            var pos = (Vector2)transform.position + _offset + 1.5f*dir;
            var rot = Vector2.SignedAngle(Vector2.left, dir);
            var projectile = Instantiate(_projectilePrefab, pos, Quaternion.Euler(0, 0, rot));

            projectile.GetComponent<Rigidbody2D>().velocity = _speed * dir;
            yield return new WaitForSeconds(_cooldownTime);
            IsRunning = false;
        }
    }
}
