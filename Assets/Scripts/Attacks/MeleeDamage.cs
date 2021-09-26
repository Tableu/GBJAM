using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDamage : MonoBehaviour
{
    public int damage;
    public float knockback;
    public LayerMask enemyLayer;

    public Collider2D Collider2D;
    
    public void Awake()
    {
        Collider2D = GetComponent<Collider2D>();
    }

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
            enemy?.TakeDamage(dmg);
        }
    }
}
