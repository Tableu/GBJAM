
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class PatrolPlatform: IState
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
        if (_enemy.AtPlatformEdge())
        {
            _speed *= -1;
        }
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
    private readonly EnemyBase _enemy;
    private readonly MovementController _movement;
    private List<Transform> _points;
    private int _activePointIdx = 0;

    public FloatingPatrol(EnemyBase enemy, List<Transform> points)
    {
        _enemy = enemy;
        _points = points;
        _movement = enemy.MovementController;
    }

    public void Tick()
    {
        Vector2 point = _points[_activePointIdx].position;
        var distToPoint = point - (Vector2)_enemy.transform.position;
        // If at point go to next point
        if (distToPoint.sqrMagnitude < 0.25)
        {
            _activePointIdx++;
            _activePointIdx %= _points.Count;
            return;
        }
        // Else move towards point
        var dir = distToPoint.normalized;
        _movement.Move(dir*_movement.WalkingSpeed);
    }

    public void OnEnter()
    {
    }

    public void OnExit()
    {
    }
}
