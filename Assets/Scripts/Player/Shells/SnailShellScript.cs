using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailShellScript : ShellScript
{
    [SerializeField] private float projectileSpeed;
    [SerializeField] GameObject projectilePrefab;
    private ProjectileAttack projectileAttack;
    // Start is called before the first frame update
    void Start()
    {
        projectileAttack = new ProjectileAttack(projectilePrefab, projectileSpeed); 
        playerStats = new PlayerController.PlayerStats(speed, maxSpeed, health, armor, projectileAttack);
    }
}
