using UnityEngine;

/// <summary>
/// Enemy Will chase down the player and try to ram them
/// </summary>
public class MeleeAttack : IState
{
    private readonly MovementController _movement;
    private readonly Transform _target;
    private readonly PlatformEnemy _enemy;
    private readonly Collider2D _collider;
    private readonly LayerMask _playerLayer = LayerMask.GetMask("Player");

    public MeleeAttack(PlatformEnemy enemy, MovementController movement, Transform target)
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

public class RangedAttack : IState
{
    private readonly MovementController _movement;
    private readonly Transform _playerTransform;
    private readonly PlatformEnemy _enemy;
    private readonly Collider2D _collider;
    private readonly LayerMask _playerLayer = LayerMask.GetMask("Player");
    private float _targetDistance;

    public RangedAttack(PlatformEnemy enemy, MovementController movement, Transform playerTransform)
    {
        _enemy = enemy;
        _movement = movement;
        _playerTransform = playerTransform;
        _collider = playerTransform.gameObject.GetComponent<Collider2D>();
        _targetDistance = 5;
    }

    public void Tick()
    {
        var playerPosition = _playerTransform.position;
        var position = _movement.Position;
        var distance = playerPosition.x - _movement.Position.x;
        var direction = Mathf.Sign(distance);
        distance = Mathf.Abs(distance) - _targetDistance;

        _enemy.CanAttack = Mathf.Abs(playerPosition.y - position.y) < 1f;

        if (Mathf.Abs(distance) < 0.5)
        {
            return;
        }
        
        var speed = Mathf.Clamp(distance * _movement.WalkingSpeed, -_movement.WalkingSpeed, _movement.WalkingSpeed);
        _movement.MoveHorizontally(direction * speed);
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