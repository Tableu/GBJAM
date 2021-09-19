
using UnityEngine;

public struct Damage
{
    public Damage(Vector2 direction, float knockback, int rawDamage)
    {
        Direction = direction.normalized;
        Knockback = knockback;
        RawDamage = rawDamage;
    }

    public readonly Vector2 Direction;
    public readonly float Knockback;
    public readonly int RawDamage;
}
