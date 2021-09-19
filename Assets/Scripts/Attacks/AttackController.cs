using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public AttackCommands.AttackStats attackStats;

    public void Hit(GameObject other)
    {
        
        // switch (LayerMask.LayerToName(other.layer))
        // {
        //     case "Player":
        //         other.GetComponent<PlayerController>().TakeDamage(attackStats, transform);
        //         Destroy(gameObject);
        //         break;
        //     case "Enemy":
        //         //call TakeDamage() equivalent in EnemyController
        //         Destroy(gameObject);
        //         break;
        // }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            Destroy(gameObject);
        }
    }
}
