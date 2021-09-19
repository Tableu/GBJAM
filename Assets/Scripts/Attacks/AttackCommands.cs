using UnityEngine;
using UnityEngine.InputSystem;

public interface Attacks
{
    void MeleeAttack(GameObject character);
    void SimpleProjectileAttack(GameObject character, GameObject projectile, AttackCommands.AttackStats attackStats);
    bool Dash(GameObject gameObject, AttackCommands.AttackStats attackStats, float start);
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

    public void SimpleProjectileAttack(GameObject character, GameObject projectilePrefab, AttackStats attackStats)
    {
        var pos = character.transform.position + new Vector3(character.transform.localScale.x*(-1),0,0);
        var projectile = Object.Instantiate(projectilePrefab, pos, Quaternion.identity);
        projectile.transform.localScale = character.transform.localScale;
        projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(attackStats.Speed*(-1)*character.transform.localScale.x,0);
    }

    public bool Dash(GameObject character, AttackStats attackStats, float start)
    {
        var transform = character.GetComponent<Transform>();
        var rigidBody = character.GetComponent<Rigidbody2D>();

        if ( Mathf.Abs(transform.position.x - start) > attackStats.Distance)
        {
            rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
            return false;
        }
        rigidBody.AddRelativeForce(new Vector2((-1)*attackStats.Speed*character.transform.localScale.x,0));
        return true;
    }
}
