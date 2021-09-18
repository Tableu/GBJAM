using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Attacks
{
    void MeleeAttack(GameObject character);
    void SimpleProjectileAttack(GameObject character, GameObject projectile);
    void Dash(GameObject character, float speed);
}

public class AttackCommands : Attacks
{
    public const String DASH = "Dash";
    public const String MELEE_ATTACK = "Melee Attack";
    public const String SIMPLE_PROJECTILE_ATTACK = "Simple Projectile Attack";
    public void MeleeAttack(GameObject character)
    {
        
    }

    public void SimpleProjectileAttack(GameObject character, GameObject projectile)
    {
        
    }

    public void Dash(GameObject character, float speed)
    {
        character.GetComponent<Rigidbody2D>().AddForce(new Vector2(speed,0));
    }
}
