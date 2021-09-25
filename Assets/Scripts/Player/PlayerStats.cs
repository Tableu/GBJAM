using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] public Vector2 speed;
    [SerializeField] public int armor;
    [SerializeField] public AttackScriptableObject attackConfig;
    [SerializeField] public Sprite shellSprite;
    [SerializeField] public Sprite damagedShell;
    [SerializeField] public int shell;
}
