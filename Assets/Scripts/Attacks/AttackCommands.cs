using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public interface AttackCommand
{
    public IEnumerator DoAttack(IDamageable target);
    public bool IsRunning { get; }
}

public class DashAttack: AttackCommand
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

    public IEnumerator DoAttack(IDamageable target)
    {
        IsRunning = true;
        var transform = _go.GetComponent<Transform>();
        var rigidBody = _go.GetComponent<Rigidbody2D>();
        var start = transform.position.x;
          
        while(Mathf.Abs(transform.position.x - start) <= _distance){
            rigidBody.AddRelativeForce(new Vector2((-1)*_speed*transform.localScale.x,0));
            yield return null;
        }
        rigidBody.velocity = Vector2.zero;
        IsRunning = false;
    }
}