using System;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class FloatingEnemy : EnemyBase
{
    [SerializeField] private float puffDistance = 2f;
    [SerializeField] private List<Transform> patrolPoints;
    private void Start()
    {
        StateMachine = new FSM();
        var patrol = new FloatingPatrol(this, patrolPoints);
        var attack = new FloatingAttack(this, puffDistance);
        StateMachine.AddTransition(patrol, attack, PlayerVisible);
        StateMachine.AddTransition(attack, patrol, () => timeSinceSawPlayer > deaggroTime);
        StateMachine.SetState(patrol);
    }
}
