using System;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class FloatingEnemy : EnemyBase
{
    [SerializeField] private List<Transform> patrolPoints;
    private void Start()
    {
        StateMachine = new FSM();
        var patrol = new FloatingPatrol(this, patrolPoints);
        var attack = new FloatingAttack(this);
        StateMachine.AddTransition(patrol, attack, () => false);
        // StateMachine.AddTransition(attack, patrol, () => !PlayerVisible());
        StateMachine.SetState(patrol);
    }
}
