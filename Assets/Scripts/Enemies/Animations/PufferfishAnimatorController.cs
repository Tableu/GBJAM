using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PufferfishAnimatorController : EnemyAnimatorController
{

    public override void SetIsMoving(bool moving)
    {
    }

    public override void TriggerHurt()
    {
    }

    public override void TriggerAttack()
    {
    }

    public override void TriggerDeath()
    {
    }

    public override void SetCanSeePlayer(bool canSee)
    {
        animCont.SetBool("IsSmall", !canSee);
    }
}
