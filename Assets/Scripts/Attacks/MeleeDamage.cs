using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDamage : MonoBehaviour
{
    public int damage;
    public float knockback;
    public LayerMask enemyLayer;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            var enemy = other.gameObject.GetComponent<IDamageable>();
            var dmg = new Damage
            {
                Knockback = knockback,
                RawDamage = damage,
                Source = transform.position
            };
            enemy.TakeDamage(dmg);
        }
    }
}
