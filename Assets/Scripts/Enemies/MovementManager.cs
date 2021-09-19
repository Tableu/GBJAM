using System;
using UnityEngine;

public class MovementManager
{
    private readonly Transform _transform;
    private BoxCollider2D _boxCollider;
    private Rigidbody2D _rigidbody;
    public Bounds Bounds
    {
        get
        {
            var bounds = _boxCollider.bounds;
            bounds.Expand(-2f*_skinWidth);
            return bounds;
        }
    }

    private ContactFilter2D _groundFilter;
    private readonly float _skinWidth = 0.2f;
    
    [Flags]
    public enum Flags
    {
        None = 0,
        Grounded = 1 << 0
    }

    public Flags MovementFlags { get; private set; }

    public MovementManager(GameObject go, ContactFilter2D groundFilter)
    {
        _transform = go.transform;
        _boxCollider = go.GetComponent<BoxCollider2D>();
        _rigidbody = go.GetComponent<Rigidbody2D>();
        MovementFlags = Flags.None;
        _groundFilter = groundFilter;
    }

    public void MoveVelocity(Vector2 velocity)
    {
        if (velocity.x > 0 && _transform.localScale.x < 0 ||
            velocity.x < 0 && _transform.localScale.x > 0)
        {
            var localScale = _transform.localScale;
            localScale = new Vector3(-localScale.x, localScale.y);
            _transform.localScale = localScale;
        }

        if (Mathf.Abs(_rigidbody.velocity.x) < Mathf.Abs(velocity.x) && MovementFlags.HasFlag(Flags.Grounded))
        {
            _rigidbody.AddForce(velocity, ForceMode2D.Impulse);
        }
        
        MovementFlags = Flags.None;
        if (_boxCollider.IsTouching(_groundFilter))
        {
            MovementFlags |= Flags.Grounded;
        }
    }

    public void Knockback(Vector2 source, float scale)
    {
        var dir = ((Vector2)_transform.position-source).normalized;
        var knockbackForce = dir * scale;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.AddForce(knockbackForce, ForceMode2D.Impulse);
    }
}