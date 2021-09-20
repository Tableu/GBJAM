using UnityEngine;

public class MovementController
{
    private Rigidbody2D _rigidbody;
    private BoxCollider2D _boxCollider;
    private Transform _transform;
    private float _maxWalkSpeed;

    public MovementController(GameObject go)
    {
        _rigidbody = go.GetComponent<Rigidbody2D>();
        _transform = go.transform;
        _boxCollider = go.GetComponent<BoxCollider2D>();
    }

    public void MoveHorizontally(float speed)
    {
    }

    public void Jump(float height)
    {
    }

    public void Stop()
    {

    }

    public void Knockback(Damage dmg)
    {
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
                return true;
            }
        }

        return false;
    }
}