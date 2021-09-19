
using UnityEngine;

public class PatrolPlatform: IState
{
    private readonly SnailEnemy _enemy;

    public PatrolPlatform(SnailEnemy enemy)
    {
        _enemy = enemy;
    }

    public void Tick()
    {
        var t = _enemy.transform;
        var rayOrigin = new Vector2(t.position.x + _enemy.lookAheadDist * t.localScale.x, 
            _enemy.MovementManager.Bounds.min.y);
        var hit = Physics2D.Raycast(rayOrigin, Vector2.down, 0.5f, _enemy.collisionLayers);
        Debug.DrawRay(rayOrigin, Vector2.down);
        if (!hit)
        {
            _enemy.CurrentVelocity.x = -_enemy.CurrentVelocity.x;
        }
    }

    public void OnEnter()
    {
        _enemy.CurrentVelocity.x = _enemy.walkingSpeed;
    }

    public void OnExit()
    {
    }
}
