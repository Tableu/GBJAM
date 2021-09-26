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
    public bool alwaysAttack = false;
    public AttackScriptableObject secondaryAttack;
    [NonSerialized] public bool StartedAttack;

    private bool _attackOnCooldown;
    private bool _startCooldown;
    private bool _startAttack;
    private bool _attackDone;
    
    private void Start()
    {
        StateMachine = new FSM();
        var patrol = new FloatingPatrol(this, patrolPoints);
        var attack = new FloatingAttack(this, puffDistance);
        var evade = new FloatingAvoid(this);
        StateMachine.AddTransition(patrol, attack, PlayerVisible);
        StateMachine.AddAnyTransition( patrol, () => timeSinceSawPlayer > deaggroTime);
        if (!alwaysAttack)
        {
            StateMachine.AddTransition(attack, evade, () => StartedAttack && _attackDone);
            StateMachine.AddTransition(evade, attack, () => !_attackOnCooldown);
        }
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

        if (_startAttack)
        {
            StartCoroutine(PuffAttackDuration());
            _startAttack = false;
        }
    }

    public void StartCooldown()
    {
        _startCooldown = true;
        _attackOnCooldown = true;
    }

    public void StartAttack()
    {
        _attackDone = false;
        _startAttack = true;
    }

    private IEnumerator PuffAttackCooldown()
    {
        var config = attackConfig as PuffAttack;
        yield return new WaitForSeconds(config.cooldown);
        _attackOnCooldown = false;
    }
    
    private IEnumerator PuffAttackDuration()
    {
        var config = attackConfig as PuffAttack;
        yield return new WaitForSeconds(config.windup + config.attackDuration);
        _attackDone = true;
    }
}
