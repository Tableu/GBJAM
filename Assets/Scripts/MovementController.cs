using System;
using System.Collections;
using UnityEngine;

public class MovementController
{
    private Rigidbody2D _rigidbody;
    private BoxCollider2D _boxCollider;
    private Transform _transform;
    private float _maxWalkSpeed;
    private int _spriteForward;

    private ContactFilter2D _groundFilter2D = new ContactFilter2D
    {
        layerMask = LayerMask.GetMask("Ground"),
        useLayerMask = true
    };

    public Vector2 Position => _transform.position;
    public float WalkingSpeed => _maxWalkSpeed;

    /// <summary>
    /// Movement controller constructor
    /// </summary>
    /// <param name="go">The gameobject to control</param>
    /// <param name="maxWalkSpeed">The maximum speed the character can walk at</param>
    /// <param name="spriteForwardDir">The direction the sprite faces (-1 for left, 1 for right)</param>
    public MovementController(GameObject go, float maxWalkSpeed, int spriteForwardDir = -1)
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

        if (Mathf.Abs(_rigidbody.velocity.x) > 9)
        {
            Debug.Log("9");
        }
        var dir = (int) Mathf.Sign(speed);
        var currentDir = (int) Mathf.Sign(_rigidbody.velocity.x);

        if (dir != _spriteForward * Math.Sign(_transform.localScale.x))
        {
            var localScale = _transform.localScale;
            localScale = new Vector3(-localScale.x, localScale.y, localScale.x);
            _transform.localScale = localScale;
        }

        if (currentDir != dir || Mathf.Abs(_rigidbody.velocity.x) < _maxWalkSpeed)
        {
            var diff = Mathf.Abs(_maxWalkSpeed)-Mathf.Abs(_rigidbody.velocity.x);
            speed = Mathf.Min(diff, Mathf.Abs(speed));
            _rigidbody.AddForce(new Vector2(speed*dir, 0), ForceMode2D.Impulse);
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

    public IEnumerator Knockback(Damage dmg)
    {
        // todo: try to improve knockback formula
        // todo: fix knockback inconsistencies
        var dir = Mathf.Sign(((Vector2) _transform.position - dmg.Source).x);
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.AddForce(new Vector2(dir * dmg.Knockback, dmg.Knockback), ForceMode2D.Impulse);
        
        // Wait for the actor to leave the ground, or 5 frames
        for (int i = 0; i < 10; i++)
        {
            if (!Grounded())
            {
                break;
            }
            yield return null;
        }
        // Stop the actor once they land
        while (!Grounded())
        {
            yield return null;
        }

        Stop();
    }

    public bool FrontClear()
    {
        // todo: look for enemies/player as well. Add a layer mask parameter to the constructor?
        // (Needed for dash to work correctly)
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
        // todo: try to only update once per frame (use Time.frameCount)
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