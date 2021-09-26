using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    public int damage;
    public float knockback;
    public LayerMask enemyLayer;

    private void Start()
    {
        StartCoroutine(DeathTimer());
    }

    private IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
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
            if (enemy != null)
            {
                pSoundManager.PlaySound(pSoundManager.Sound.eHit);
                enemy.TakeDamage(dmg);
            }
        }
        Destroy(gameObject);
    }
}
