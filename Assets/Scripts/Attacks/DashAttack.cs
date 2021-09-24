using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Attacks
{
    [CreateAssetMenu(fileName = "Attack", menuName = "Attacks/Dash", order = 0)]
    public class DashAttack : AttackScriptableObject
    {
        // Start is called before the first frame update

        public float distance;
        public float speed;
        public int damage;
        public float windupTime;
        public float cooldownTime;
        public override AttackCommand MakeAttack()
        {
            return new Attack(distance, speed, damage, windupTime, cooldownTime);
        }
        
        private class Attack : AttackCommand
        {
            private float _distance;
            private float _speed;
            private int _damage;
            private float _windupTime;
            private float _cooldownTime;
            private GameObject afterImage;
    
            public Attack(float distance, float speed, int damage, float windupTime, float cooldownTime)
            {
                _distance = distance;
                _speed = speed;
                _damage = damage;
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
                var controller = attacker.GetComponent<PlayerController>();
                var start = transform.position.x;
                
                while (Mathf.Abs(transform.position.x - start) <= _distance &&
                       controller.frontClear)
                {
                    rigidBody.AddRelativeForce(new Vector2((-1) * _speed * transform.localScale.x, 0));
                    yield return null;
                }
                
                rigidBody.velocity = Vector2.zero;
                yield return new WaitForSeconds(_cooldownTime);
                IsRunning = false;
                LockInput = false;
                
            }
        }
    }
}
