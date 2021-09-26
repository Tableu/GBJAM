using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Attacks
{
    [CreateAssetMenu(fileName = "Attack", menuName = "Attacks/Dash", order = 0)]
    public class DashAttack : AttackScriptableObject
    {
        // Start is called before the first frame update

        public float distance;
        public float speed;
        public float windupTime;
        public float cooldownTime;

        public override AttackCommand MakeAttack()
        {
            return new Attack(distance, speed, windupTime, cooldownTime);
        }

        private class Attack : AttackCommand
        {
            private readonly float _cooldownTime;
            private readonly float _distance;
            private readonly LayerMask _enemyLayer = LayerMask.GetMask("Enemy");
            private readonly float _speed;
            private readonly float _windupTime;

            public Attack(float distance, float speed, float windupTime, float cooldownTime)
            {
                _distance = distance;
                _speed = speed;
                _windupTime = windupTime;
                _cooldownTime = cooldownTime;
            }

            public bool IsRunning { get; private set; }
            public bool LockInput { get; private set; }

            public IEnumerator DoAttack(GameObject attacker)
            {
                IsRunning = true;
                LockInput = true;
                yield return new WaitForSeconds(_windupTime);
                var transform = attacker.GetComponent<Transform>();
                var rigidBody = attacker.GetComponent<Rigidbody2D>();
                // todo: use movement controller
                var controller = attacker.GetComponent<PlayerController>().MovementController;
                var start = transform.position.x;
                var collider = attacker.GetComponent<Collider2D>();


                rigidBody.velocity = new Vector2(_speed * -1 * transform.localScale.x, rigidBody.velocity.y);
                while (Mathf.Abs(transform.position.x - start) <= _distance &&
                       controller.FrontClear() && !collider.IsTouchingLayers(_enemyLayer))
                {
                    rigidBody.AddRelativeForce(new Vector2(-1 * _speed * transform.localScale.x, 0));
                    yield return null;
                }
                rigidBody.velocity = Vector2.zero;
                var meleeDmg = attacker.GetComponentInChildren<MeleeDamage>();
                meleeDmg.Collider2D.enabled = true;
                yield return new WaitForSeconds(0.25f);
                meleeDmg.Collider2D.enabled = false;
                LockInput = false;
                yield return new WaitForSeconds(_cooldownTime);
                IsRunning = false;
            }
        }
    }
}