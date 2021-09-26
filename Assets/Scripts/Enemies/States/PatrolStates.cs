using System;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPlatform : IState
{
    private readonly PlatformEnemy _enemy;
    private readonly MovementController _movement;
    private float _speed;

    public PatrolPlatform(PlatformEnemy enemy, MovementController movement)
    {
        _enemy = enemy;
        _movement = movement;
    }

    public void Tick()
    {
        if (_enemy.AtPlatformEdge()) _speed *= -1;
        _movement.MoveHorizontally(_speed);
    }

    public void OnEnter()
    {
        _speed = _movement.WalkingSpeed;
    }

    public void OnExit()
    {
    }
}

public class FloatingPatrol : IState
{
    private readonly PufferEnemy _enemy;
    private readonly MovementController _movement;
    private readonly List<Transform> _points;
    private int _activePointIdx;

    public FloatingPatrol(PufferEnemy enemy, List<Transform> points)
    {
        _enemy = enemy;
        _points = points;
        _movement = enemy.MovementController;
    }

    public void Tick()
    {
        Vector2 point = _points[_activePointIdx].position;
        var distToPoint = point - (Vector2) _enemy.transform.position;
        // If at point go to next point
        if (distToPoint.sqrMagnitude < 0.25)
        {
            _activePointIdx++;
            _activePointIdx %= _points.Count;
            return;
        }

        // Else move towards point
        var dir = distToPoint.normalized;
        _movement.Move(dir * _movement.WalkingSpeed);
    }

    public void OnEnter()
    {
    }

    public void OnExit()
    {
    }
}

public class FloatingAvoid : IState
{
    private readonly PufferEnemy _enemy;
    private readonly MovementController _movement;
    private readonly Transform _playerTransform;
    private readonly float _targetDistance = 10f;

    public FloatingAvoid(PufferEnemy enemy)
    {
        _enemy = enemy;
        _movement = enemy.MovementController;
        _playerTransform = enemy.PlayerTransform;
    }

    public void Tick()
    {
        if (_playerTransform == null) return;
        Vector2 toPlayer = _playerTransform.position - _enemy.transform.position;

        var dir = toPlayer.normalized;
        var distError = toPlayer.magnitude - _targetDistance;
        var dirSign = Math.Sign(distError);

        if (Mathf.Abs(distError) < 0.75f)
        {
            _movement.SetDirection(dirSign);
            return;
        }
        _movement.Move(dirSign * dir * _movement.WalkingSpeed);
    }

    public void OnEnter()
    {
        Debug.Log("Evading");
        _enemy.StartCooldown();
        _enemy.CanAttack = false;
        _movement.Stop();
    }

    public void OnExit()
    {
    }
}