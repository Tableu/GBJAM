using UnityEngine;

public class SnailAnimatorController : EnemyAnimatorController
{
    public override void SetIsMoving(bool moving)
    {
        animCont.SetBool("IsMoving", moving);
    }
    public override void TriggerHurt()
    {
        //pSoundManager.PlaySound(pSoundManager.Sound.eHit);
        animCont.SetTrigger("Hit");
    }
    public override void TriggerAttack()
    {
        animCont.SetTrigger("Attack");
    }
    public override void TriggerDeath()
    {
        pSoundManager.PlaySound(pSoundManager.Sound.eDie);
        animCont.SetTrigger("Death");
    }

    public override void IsAngry(bool angry)
    { }
}