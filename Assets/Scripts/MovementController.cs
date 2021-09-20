using System;
using UnityEngine;

public class MovementController
{
    private Rigidbody2D _rigidbody;
    private BoxCollider2D _boxCollider;
    private Transform _transform;
    private float _maxWalkSpeed;
    private int _spriteForward;

    ContactFilter2D _groundFilter2D = new ContactFilter2D
    {
        layerMask = LayerMask.GetMask("Ground"),
        useLayerMask = true
    };

    /// <summary>
    /// Movement controller constructor
    /// </summary>
    /// <param name="go">The gameobject to control</param>
    /// <param name="maxWalkSpeed">The maximum speed the character can walk at</param>
    /// <param name="spriteForwardDir">The direction the sprite faces (-1 for left, 1 for right)</param>
    public MovementController(GameObject go, float maxWalkSpeed, int spriteForwardDir=-1)
    {
        _rigidbody = go.GetComponent<Rigidbody2D>();
        _transform = go.transform;
        _boxCollider = go.GetComponent<BoxCollider2D>();
        _maxWalkSpeed = maxWalkSpeed;
        _spriteForward = spriteForwardDir;
    }

    public void MoveHorizontally(float speed)
    {
        if (speed == 0)
        {
            return;
        }

        var dir = (int) Mathf.Sign(speed);
        var currentDir = (int) Mathf.Sign(_rigidbody.velocity.x);

        if (dir != _spriteForward*Math.Sign(_transform.localScale.x))
        {
            var localScale = _transform.localScale;
            localScale = new Vector3(-localScale.x, localScale.y, localScale.x);
            _transform.localScale = localScale;
        }

        if (currentDir != dir || Mathf.Abs(_rigidbody.velocity.x) < _maxWalkSpeed)
        {
            _rigidbody.AddForce(new Vector2(speed, 0), ForceMode2D.Impulse);
        }
    }

    public void Jump(float height)
    {
        if (!Grounded()) return;
        var a = -Physics2D.gravity.y * _rigidbody.gravityScale;
        var speed = Mathf.Sqrt(2 * a * height);
        _rigidbody.AddForce(new Vector2(0, speed * _rigidbody.mass), ForceMode2D.Impulse);
    }

    public void Stop()
    {
        _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
    }

    public void Knockback(Damage dmg)
    {
        // todo: improve knockback calculations and logic
        var dir = ((Vector2)_transform.position - dmg.Source).normalized;
        var knockbackForce = dir * dmg.Knockback;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.AddForce(knockbackForce, ForceMode2D.Impulse);
    }

    public bool FrontClear()
    {
        RaycastHit2D[] hit = new RaycastHit2D[1];
        if (_boxCollider.Raycast(new Vector2(_transform.localScale.x * (-1), 0), hit, 1, LayerMask.GetMask("Ground")) >
            0)
        {
            return false;
        }

        return true;
    }

    public bool Grounded()
    {
        RaycastHit2D hit;
        var bounds = _boxCollider.bounds;
        Vector2[] posArray =
        {
            new Vector2(bounds.max.x, bounds.min.y),
            new Vector2(bounds.center.x, bounds.min.y),
            new Vector2(bounds.min.x, bounds.min.y)
        };
        for (int x = 0; x < 3; x++)
        {
            hit = Physics2D.Raycast(posArray[x], Vector2.down, 0.3f, LayerMask.GetMask("Ground"));
            Debug.DrawRay(posArray[x], new Vector2(0, -0.3f), Color.red);
            if (hit.collider != null)
            {
                return _boxCollider.IsTouching(_groundFilter2D);
            }
        }

        return false;
    }
}