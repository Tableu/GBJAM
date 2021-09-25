using System;
using System.Collections;
using UnityEngine;

public class MovementController
{
    private readonly BoxCollider2D _boxCollider;

    private readonly ContactFilter2D _groundFilter2D = new ContactFilter2D
    {
        layerMask = LayerMask.GetMask("Ground"),
        useLayerMask = true
    };

    private bool _inputLocked;
    private readonly Rigidbody2D _rigidbody;
    private readonly int _spriteForward;
    private readonly Transform _transform;

    /// <summary>
    ///     Movement controller constructor
    /// </summary>
    /// <param name="go">The gameobject to control</param>
    /// <param name="maxWalkSpeed">The maximum speed the character can walk at</param>
    /// <param name="spriteForwardDir">The direction the sprite faces (-1 for left, 1 for right)</param>
    public MovementController(GameObject go, float maxWalkSpeed, int spriteForwardDir = -1)
    {
        _rigidbody = go.GetComponent<Rigidbody2D>();
        _transform = go.transform;
        _boxCollider = go.GetComponent<BoxCollider2D>();
        WalkingSpeed = maxWalkSpeed;
        _spriteForward = spriteForwardDir;
    }

    public Vector2 Position => _transform.position;
    public float WalkingSpeed { get; set; }

    public void MoveHorizontally(float speed)
    {
        if (speed == 0 || _inputLocked) return;

        var dir = (int) Mathf.Sign(speed);
        var currentDir = (int) Mathf.Sign(_rigidbody.velocity.x);

        if (dir != _spriteForward * Math.Sign(_transform.localScale.x))
        {
            var localScale = _transform.localScale;
            localScale = new Vector3(-localScale.x, localScale.y, localScale.x);
            _transform.localScale = localScale;
        }

        if (currentDir != dir || Mathf.Abs(_rigidbody.velocity.x) < WalkingSpeed)
        {
            var diff = Mathf.Abs(WalkingSpeed) - Mathf.Abs(_rigidbody.velocity.x);
            speed = Mathf.Min(diff, Mathf.Abs(speed));
            _rigidbody.AddForce(new Vector2(speed * dir, 0), ForceMode2D.Impulse);
        }
    }

    public int GetDirection()
    {
        return (int) Mathf.Sign(_transform.localScale.x * _spriteForward);
    }

    public void SetDirection(int dir)
    {
        var localScale = _transform.localScale;
        localScale = new Vector3(Mathf.Abs(localScale.x) * dir * _spriteForward, localScale.y, localScale.x);
        _transform.localScale = localScale;
    }

    public bool Jump(float height)
    {
        if (!Grounded()) return false;
        var a = -Physics2D.gravity.y * _rigidbody.gravityScale;
        var speed = Mathf.Sqrt(2 * a * height);
        _rigidbody.AddForce(new Vector2(0, speed * _rigidbody.mass), ForceMode2D.Impulse);
        return true;
    }

    public void Stop()
    {
        _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
    }

    public IEnumerator Knockback(Damage dmg)
    {
        // todo: try to improve knockback formula
        // todo: fix knockback inconsistencies
        _inputLocked = true;
        var dir = Mathf.Sign(_transform.position.x - dmg.Source.x);
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.AddForce(new Vector2(dir * dmg.Knockback, dmg.Knockback), ForceMode2D.Impulse);
        float timoutTimer = 0;

        // Wait at least 0.5s
        yield return new WaitForSeconds(0.5f);

        // Stop the actor once they land
        while (!_boxCollider.IsTouching(_groundFilter2D))
        {
            // Stop the player from getting suck on enemies
            timoutTimer += Time.deltaTime;
            if (timoutTimer > 1.5f) break;

            yield return null;
        }

        Stop();
        _inputLocked = false;
    }

    public bool FrontClear()
    {
        // todo: look for enemies/player as well. Add a layer mask parameter to the constructor?
        // (Needed for dash to work correctly)
        var hit = new RaycastHit2D[1];
        if (_boxCollider.Raycast(new Vector2(_transform.localScale.x * -1, 0), hit, 1, LayerMask.GetMask("Ground")) >
            0)
            return false;

        return true;
    }

    public bool NearCeiling()
    {
        RaycastHit2D hit;
        var bounds = _boxCollider.bounds;
        Vector2[] posArray =
        {
            new Vector2(bounds.max.x, bounds.max.y),
            new Vector2(bounds.center.x, bounds.max.y),
            new Vector2(bounds.min.x, bounds.max.y)
        };
        for (var x = 0; x < 3; x++)
        {
            hit = Physics2D.Raycast(posArray[x], Vector2.up, 0.3f, LayerMask.GetMask("Ground"));
            Debug.DrawRay(posArray[x], new Vector2(0, -0.3f), Color.red);
            if (hit.collider != null) return _boxCollider.IsTouching(_groundFilter2D);
        }

        return false;
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
        for (var x = 0; x < 3; x++)
        {
            hit = Physics2D.Raycast(posArray[x], Vector2.down, 0.3f, LayerMask.GetMask("Ground"));
            Debug.DrawRay(posArray[x], new Vector2(0, -0.3f), Color.red);
            if (hit.collider != null) return _boxCollider.IsTouching(_groundFilter2D);
        }

        return false;
    }

    public void SetMovementEnable(bool status)
    {
        _inputLocked = !status;
    }
}