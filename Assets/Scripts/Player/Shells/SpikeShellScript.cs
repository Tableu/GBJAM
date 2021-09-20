using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeShellScript : ShellScript
{
    [SerializeField] private float distance;
    [SerializeField] private float dashSpeed;
    [SerializeField] private int damage;
    private DashAttack dashAttack;
    // Start is called before the first frame update
    void Start()
    {
        dashAttack = new DashAttack(GameObject.FindWithTag("Player"), distance, dashSpeed, damage);
        playerStats = new PlayerController.PlayerStats(speed, maxSpeed, health, armor, dashAttack);
    }
}
