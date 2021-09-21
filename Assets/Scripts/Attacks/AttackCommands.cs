using System;
using System.Collections;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public interface AttackCommand
{
    public IEnumerator DoAttack(GameObject attacker);
    public bool IsRunning { get; }
    public bool LockInput { get; }
}
[Serializable]
public class DashAttack : AttackCommand
{
    [SerializeField]private float _distance;
    [SerializeField]private float _speed;
    [SerializeField]private int _damage;
    
    public DashAttack(float distance, float speed, int damage)
    {
        _distance = distance;
        _speed = speed;
        _damage = damage;
    }

    public bool IsRunning { get; private set; }
    public bool LockInput { get; private set; }

    public IEnumerator DoAttack(GameObject attacker)
    {
        IsRunning = true;
        LockInput = true;
        var transform = attacker.GetComponent<Transform>();
        var rigidBody = attacker.GetComponent<Rigidbody2D>();
        var collider = attacker.GetComponent<Collider2D>();
        // todo: use movement controller
        var controller = attacker.GetComponent<PlayerController>();
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

public class ProjectileAttack : AttackCommand
{
    [SerializeField] private float _speed;
    [SerializeField] private GameObject _projectilePrefab;
    private float _horizontalOffset;
    private float _attackDuration;

    public ProjectileAttack(GameObject projectilePrefab, float speed, float attackDuration=0.5f, float horizontalOffset=1)
    {
        _speed = speed;
        _attackDuration = attackDuration;
        _horizontalOffset = horizontalOffset;
        _projectilePrefab = projectilePrefab;
    }
    public bool IsRunning { get; private set; }
    public bool LockInput { get; private set; }

    public IEnumerator DoAttack(GameObject attacker)
    {
        IsRunning = true;

        // needed to line up with attack animations
        // todo: make parameter?
        yield return new WaitForSeconds(0.5f);
        
        var transform = attacker.GetComponent<Transform>();
        var dir = -1 * Mathf.Sign(transform.localScale.x);
        var pos = transform.position + dir * _horizontalOffset * Vector3.right;
        var projectile = GameObject.Instantiate(_projectilePrefab, pos, Quaternion.identity);
        projectile.transform.localScale = transform.localScale;
        projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(_speed*(-1)*transform.localScale.x,0);
        yield return new WaitForSeconds(_attackDuration);
        IsRunning = false;
    }
}

public abstract class AttackScriptableObject : ScriptableObject
{
    public abstract AttackCommand MakeAttack();
}
