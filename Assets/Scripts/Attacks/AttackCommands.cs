using System.Collections;
using UnityEngine;

public interface AttackCommand
{
    public IEnumerator DoAttack(IDamageable target);
    public bool IsRunning { get; }
    public bool LockInput { get; }
}

public class DashAttack : AttackCommand
{
    private float _distance;
    private float _speed;
    private int _damage;
    private GameObject _go;

    public DashAttack(GameObject go, float distance, float speed, int damage)
    {
        _distance = distance;
        _speed = speed;
        _damage = damage;
        _go = go;
    }

    public bool IsRunning { get; private set; }
    public bool LockInput { get; private set; }

    public IEnumerator DoAttack(IDamageable target)
    {
        IsRunning = true;
        LockInput = true;
        var transform = _go.GetComponent<Transform>();
        var rigidBody = _go.GetComponent<Rigidbody2D>();
        var collider = _go.GetComponent<Collider2D>();
        // todo: use movement controller
        var controller = _go.GetComponent<PlayerController>();
        var start = transform.position.x;

        while (Mathf.Abs(transform.position.x - start) <= _distance &&
               !collider.IsTouchingLayers(LayerMask.NameToLayer("Ground")) &&
               controller.frontClear)
        {
            rigidBody.AddRelativeForce(new Vector2((-1) * _speed * transform.localScale.x, 0));
            yield return null;
        }

        rigidBody.velocity = Vector2.zero;
        IsRunning = false;
        LockInput = false;
    }
}