using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class PufferEnemy : EnemyBase
{
    [SerializeField] private float puffDistance = 2f;
    [SerializeField] private List<Transform> patrolPoints;
    public float puffWalkSpeed;
    [NonSerialized] public bool StartedAttack;

    private bool _attackOnCooldown;
    private bool _startCooldown;
    
    private void Start()
    {
        StateMachine = new FSM();
        var patrol = new FloatingPatrol(this, patrolPoints);
        var attack = new FloatingAttack(this, puffDistance);
        var evade = new FloatingAvoid(this);
        StateMachine.AddTransition(patrol, attack, PlayerVisible);
        StateMachine.AddTransition(attack, patrol, () => timeSinceSawPlayer > deaggroTime);
        StateMachine.AddTransition(attack, evade, () => StartedAttack && !Attack.IsRunning);
        StateMachine.AddTransition(evade, attack, () => !_attackOnCooldown);
        StateMachine.SetState(patrol);
    }

    private new void Update()
    {
        base.Update();
        if (_startCooldown)
        {
            StartCoroutine(PuffAttackCooldown());
            _startCooldown = false;
        }
    }

    public void StartCooldown()
    {
        _startCooldown = true;
        _attackOnCooldown = true;
    }
    private IEnumerator PuffAttackCooldown()
    {
        var config = attackConfig as PuffAttack;
        yield return new WaitForSeconds(config.cooldown);
        _attackOnCooldown = false;
    }
}
