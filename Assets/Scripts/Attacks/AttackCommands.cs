using System.Collections;
using UnityEngine;

public interface AttackCommand
{
    public IEnumerator DoAttack(GameObject attacker);
    public bool IsRunning { get; }
    public bool LockInput { get; }
}

public abstract class AttackScriptableObject : ScriptableObject
{
    public abstract AttackCommand MakeAttack();
}
