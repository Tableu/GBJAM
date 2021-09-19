using System;
using UnityEngine;

public class MovementManager
{
    private readonly Transform _transform;
    private BoxCollider2D _boxCollider;
    private Bounds Bounds => _boxCollider.bounds;
    private LayerMask _collisionLayers;
    
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
        // todo: handle jumping
        if (!(displacement.y < 0)) return;
        var rayOrigin = new Vector2(Bounds.center.x, Bounds.min.y);
        var rayLen = displacement.magnitude;
        var hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLen, _collisionLayers);
        
        if (hit)
        {
            displacement.y = hit.point.y - rayOrigin.y;
            MovementFlags |= Flags.Grounded;
        }
        else
        {
            MovementFlags &= ~Flags.Grounded;
        }
    }
}