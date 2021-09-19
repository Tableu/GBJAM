using UnityEngine;
using UnityEngine.InputSystem;

public interface Attacks
{
    void MeleeAttack(GameObject character);
    void SimpleProjectileAttack(GameObject character, GameObject projectile);
    bool Dash(GameObject gameObject, AttackCommands.AttackStats attackStats, float start);
    void Collision(GameObject character);
}

public class AttackCommands : Attacks
{
    [System.Serializable]
    public struct AttackStats
    {
        public AttackStats(string powerUp, float speed, int damage, Vector2 knockback, float distance)
        {
            _powerUp = powerUp;
            _speed = speed;
            _damage = damage;
            _knockback = knockback;
            _distance = distance;
        }

        [SerializeField] private string _powerUp;
        [SerializeField] private float _speed;
        [SerializeField] private int _damage;
        [SerializeField] private Vector2 _knockback;
        [SerializeField] private float _distance;
        public string PowerUp => _powerUp;
        public float Speed => _speed;
        public int Damage => _damage;
        public Vector2 Knockback => _knockback;
        public float Distance => _distance;
    }
    
    public const System.String DASH = "Dash";
    public const System.String MELEE_ATTACK = "Melee Attack";
    public const System.String SIMPLE_PROJECTILE_ATTACK = "Simple Projectile Attack";
    public const System.String COLLISION = "Collision";
    public void MeleeAttack(GameObject character)
    {
        
    }

    public void SimpleProjectileAttack(GameObject character, GameObject projectile)
    {
        
    }

    public bool Dash(GameObject gameObject, AttackStats attackStats, float start)
    {
        var transform = gameObject.GetComponent<Transform>();
        var rigidBody = gameObject.GetComponent<Rigidbody2D>();

        if ( Mathf.Abs(transform.position.x - start) > attackStats.Distance)
        {
            rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
            return false;
        }
        rigidBody.AddRelativeForce(new Vector2((-1)*attackStats.Speed*gameObject.transform.localScale.x,0));
        return true;
    }

    public void Collision(GameObject character)
    {
        
    }
}
