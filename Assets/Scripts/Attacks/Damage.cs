
using UnityEngine;

public struct Damage
{
    public Damage(Vector2 source, float knockback, int rawDamage)
    {
        Source = source;
        Knockback = knockback;
        RawDamage = rawDamage;
    }

    public Vector2 Source;
    public float Knockback;
    public int RawDamage;
}
