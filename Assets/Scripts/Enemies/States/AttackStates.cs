using System;
using Enemies;
using UnityEngine;

public class RangedAttack : IState
{
    private readonly MovementController _movement;
    private readonly Transform _playerTransform;
    private readonly PlatformEnemy _enemy;
    private float _targetDistance;
    private float _verticalAttackRange;

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
        // todo: stop enemy from running off edge of platform
        var playerPosition = _playerTransform.position;
        var position = _movement.Position;
        var distance = playerPosition.x - _movement.Position.x;
        var direction = Mathf.Sign(distance);
        var error = Mathf.Abs(distance) - _targetDistance;

        _enemy.CanAttack = Mathf.Abs(playerPosition.y - position.y) < _verticalAttackRange &&
                           Math.Sign(distance) == _movement.GetDirection();

        if (Mathf.Abs(error) < 0.5f || !_movement.FrontClear() || !_movement.BackClear())
        {
            _movement.SetDirection(Math.Sign(distance));
            return;
        }

        var speed = Mathf.Clamp(error * _movement.WalkingSpeed, -_movement.WalkingSpeed, _movement.WalkingSpeed);
        _movement.MoveHorizontally(direction * speed);
    }

    public void OnEnter()
    {
        _movement.Stop();
    }

    public void OnExit()
    {
        _enemy.CanAttack = false;
    }
}

public class FloatingAttack : IState
{
    private readonly MovementController _movement;
    private readonly Transform _playerTransform;
    private readonly EnemyBase _enemy;
    private float _puffDistance;
    
    public FloatingAttack(EnemyBase enemy, float puffDistance)
    {
        _enemy = enemy;
        _puffDistance = puffDistance;
        _movement = enemy.MovementController;
        _playerTransform = enemy.PlayerTransform;
        _playerTransform.gameObject.GetComponent<Collider2D>();
    }
    public void Tick()
    {
        if (_playerTransform != null)
        {
            Vector2 distToPoint = _playerTransform.position - _enemy.transform.position;
            _enemy.CanAttack = distToPoint.sqrMagnitude < _puffDistance * _puffDistance;
            var dir = distToPoint.normalized;
            _movement.Move(dir * _movement.WalkingSpeed);
        }
    }

    public void OnEnter()
    {
    }

    public void OnExit()
    {
        _enemy.CanAttack = false;
    }
}

// todo: Attack Pattern for Jump attack
// Try to charge the player and jump when out of range