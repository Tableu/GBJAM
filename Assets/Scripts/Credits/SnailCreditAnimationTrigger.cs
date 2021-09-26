using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailCreditAnimationTrigger : CreditAnimationTrigger
{
    enum SnailAnimation { Idle, Moving, Hit, Attack }
    [SerializeField]
    SnailAnimation startState;
    [SerializeField]
    SnailAnimation triggerState;
    private void Start()
    {
        UpdateAnimator(startState);
    }
    public override void TriggerAnimation()
    {
        base.TriggerAnimation();
        UpdateAnimator(triggerState);
    }
    void UpdateAnimator(SnailAnimation toChangeTo)
    {
        switch (toChangeTo)
        {
            case (SnailAnimation.Idle):
                animCont.SetBool("IsMoving", false);
                animCont.Play("Snail-Idle");
                break;
            case (SnailAnimation.Moving):
                animCont.SetBool("IsMoving", true);
                animCont.Play("Snail-Move");
                break;
            case (SnailAnimation.Attack):
                animCont.SetTrigger("Attack");
                break;
            case (SnailAnimation.Hit):
                animCont.SetTrigger("Hit");
                break;
        }
    }
}
