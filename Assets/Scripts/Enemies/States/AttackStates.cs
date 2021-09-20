using UnityEngine;

/// <summary>
/// Enemy Will chase down the player and try to ram them
/// </summary>
public class MeleeAttack : IState
{
    private readonly MovementController _movement;
    private readonly Transform _target;
    private readonly SnailEnemy _enemy;
    private readonly Collider2D _collider;
    private readonly LayerMask _playerLayer = LayerMask.GetMask("Player");

    public MeleeAttack(SnailEnemy enemy, MovementController movement, Transform target)
    {
        _enemy = enemy;
        _movement = movement;
        _target = target;
        _collider = target.gameObject.GetComponent<Collider2D>();
    }

    public void Tick()
    {
        if (_collider.IsTouchingLayers(_playerLayer))
        {
            return;
        }

        Vector2 targetPos = _target.position;
        var dist = targetPos.x - _movement.Position.x;
        var dir = Mathf.Sign(dist);
        if (!_enemy.AtPlatformEdge())
        {
            _movement.MoveHorizontally(dir * _movement.WalkingSpeed);
        }
    }

    public void OnEnter()
    {
        _movement.Stop();
    }

    public void OnExit()
    {
        _movement.Stop();
    }
}