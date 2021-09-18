using UnityEngine;

public interface Attacks
{
    void MeleeAttack(GameObject character);
    void SimpleProjectileAttack(GameObject character, GameObject projectile);
    void Dash(Rigidbody2D character, float speed);
    void Collision(GameObject character);
}

public class AttackCommands : Attacks
{
    [System.Serializable]
    public struct AttackStats
    {
        public AttackStats(string powerUp, float speed, int damage, Vector2 knockback)
        {
            _powerUp = powerUp;
            _speed = speed;
            _damage = damage;
            _knockback = knockback;
        }

        [SerializeField] private string _powerUp;
        [SerializeField] private float _speed;
        [SerializeField] private int _damage;
        [SerializeField] private Vector2 _knockback;
        public string PowerUp => _powerUp;
        public float Speed => _speed;
        public int Damage => _damage;
        public Vector2 Knockback => _knockback;
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

    public void Dash(Rigidbody2D characterRigid, float speed)
    {
        characterRigid.AddForce(new Vector2(speed,0));
    }

    public void Collision(GameObject character)
    {
        
    }
}
