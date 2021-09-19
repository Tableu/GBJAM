using System;
using UnityEngine;

public class MovementManager
{
    private readonly Transform _transform;
    private BoxCollider2D _boxCollider;
    public Bounds Bounds
    {
        get
        {
            var bounds = _boxCollider.bounds;
            bounds.Expand(-2f*_skinWidth);
            return bounds;
        }
    }

    private LayerMask _collisionLayers;
    private readonly float _skinWidth = 0.2f;
    
    [Flags]
    public enum Flags
    {
        None = 0,
        Grounded = 1 << 0
    }

    public Flags MovementFlags { get; private set; }

    public MovementManager(GameObject go, LayerMask collisionLayers)
    {
        _transform = go.transform;
        _boxCollider = go.GetComponent<BoxCollider2D>();
        MovementFlags = Flags.None;
        _collisionLayers = collisionLayers;
    }

    public void Move(Vector2 displacement)
    {
        MovementFlags = Flags.None;
        if (displacement.x > 0 && _transform.localScale.x < 0 ||
            displacement.x < 0 && _transform.localScale.x > 0)
        {
            var localScale = _transform.localScale;
            localScale = new Vector3(-localScale.x, localScale.y);
            _transform.localScale = localScale;
        }
        MoveVertically(ref displacement);
        // todo: handle horizontal collisions
        _transform.Translate(displacement);
    }

    private void MoveVertically(ref Vector2 displacement)
    {
        // todo: handle jumping (upwards displacement)
        if (!(displacement.y < 0)) return;
        var rayOrigin = new Vector2(Bounds.center.x, Bounds.min.y);
        rayOrigin.x += displacement.x;
        var rayLen = Mathf.Abs(displacement.y) + _skinWidth;
        var hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLen, _collisionLayers);
        Debug.DrawRay(rayOrigin, Vector2.down);
        if (hit)
        {
            displacement.y = -(hit.distance - _skinWidth);
            MovementFlags |= Flags.Grounded;
        }
    }
}