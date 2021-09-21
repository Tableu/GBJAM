
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
