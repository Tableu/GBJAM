using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(AttackCommands.AttackStats attackStats, Transform otherPos);
}