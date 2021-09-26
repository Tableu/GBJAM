using System;
using Enemies;
using UnityEngine;

public class RangedAttack : IState
{
    private readonly PlatformEnemy _enemy;
    private readonly MovementController _movement;
    private readonly Transform _playerTransform;
    private readonly float _targetDistance;
    private readonly float _verticalAttackRange;

    public RangedAttack(PlatformEnemy enemy, float targetDistance, float verticalAttackRange)
    {
        _enemy = enemy;
        _movement = enemy.MovementController;
        _playerTransform = enemy.PlayerTransform;
        _playerTransform.gameObject.GetComponent<Collider2D>();
        _targetDistance = targetDistance;
        _verticalAttackRange = verticalAttackRange;
    }

    public void Tick()
    {
        if (_playerTransform != null)
        {
            var playerPosition = _playerTransform.position;
            var position = _movement.Position;
            var distance = playerPosition.x - _movement.Position.x;
            var direction = Math.Sign(distance);
            var error = Mathf.Abs(distance) - _targetDistance;

            _enemy.CanAttack = Mathf.Abs(playerPosition.y - position.y) < _verticalAttackRange &&
                               Math.Sign(distance) == _movement.GetDirection();

            if (Mathf.Abs(error) < 0.5f || !_movement.FrontClear() ||
                !_movement.BackClear() && Math.Sign(error) == _movement.GetDirection())
            {
                _movement.SetDirection(direction);
                return;
            }

            var speed = Mathf.Clamp(error * _movement.WalkingSpeed, -_movement.WalkingSpeed, _movement.WalkingSpeed);
            _movement.MoveHorizontally(direction * speed);
        }
    }

    public void OnEnter()
    {
        Debug.Log("Attacking");
        _movement.Stop();
    }

    public void OnExit()
    {
        _enemy.CanAttack = false;
    }
}

public class FloatingAttack : IState
{
    private readonly PufferEnemy _enemy;
    private readonly MovementController _movement;
    private readonly Transform _playerTransform;
    private readonly float _puffDistance;

    public FloatingAttack(PufferEnemy enemy, float puffDistance)
    {
        _enemy = enemy;
        _puffDistance = puffDistance;
        _movement = enemy.MovementController;
        _playerTransform = enemy.PlayerTransform;
        _playerTransform.gameObject.GetComponent<Collider2D>();
    }

    public void Tick()
    {
        if (_playerTransform == null) return;
        Vector2 distToPoint = _playerTransform.position - _enemy.transform.position;

        if (distToPoint.sqrMagnitude < _puffDistance * _puffDistance && !_enemy.StartedAttack)
        {
            _enemy.CanAttack = true;
            _enemy.StartedAttack = true;
            _enemy.StartAttack();
        }
        else
        {
            _enemy.CanAttack = false;
        }

        var dir = distToPoint.normalized;
        _movement.Move(dir * _movement.WalkingSpeed);
    }

    public void OnEnter()
    {
        _enemy.StartedAttack = false;
    }

    public void OnExit()
    {
        _enemy.CanAttack = false;
    }
}

// todo: Attack Pattern for Jump attack
// Try to charge the player and jump when out of range