
using UnityEngine;

public struct Damage
{
    public Damage(Vector2 source, float knockback, int rawDamage)
    {
        Source = source;
        Knockback = knockback;
        RawDamage = rawDamage;
    }

    public readonly Vector2 Source;
    public readonly float Knockback;
    public readonly int RawDamage;
}
