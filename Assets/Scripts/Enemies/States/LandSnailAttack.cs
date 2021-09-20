
using UnityEngine;

public class LandSnailAttack: IState
{
    private readonly MovementController _movement;
    private readonly Transform _target;
    private readonly SnailEnemy _enemy;

    public LandSnailAttack(SnailEnemy enemy, MovementController movement, Transform target)
    {
        _enemy = enemy;
        _movement = movement;
        _target = target;
    }

    public void Tick()
    {
        Vector2 targetPos = _target.position;
        var dir = Mathf.Sign( (targetPos-_movement.Position).x);
        if (!_enemy.AtPlatformEdge())
        {
            _movement.MoveHorizontally(dir*_movement.WalkingSpeed);
        }
    }

    public void OnEnter()
    {
        _movement.Stop();
    }

    public void OnExit()
    {
    }
}
