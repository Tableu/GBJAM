using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellScript : MonoBehaviour
{
    [SerializeField] public Vector2 speed;
    [SerializeField] public Vector2 maxSpeed;
    [SerializeField] public int health;
    [SerializeField] public int armor;
    [HideInInspector] public PlayerController.PlayerStats playerStats;
    public AttackScriptableObject attackConfig;
}
