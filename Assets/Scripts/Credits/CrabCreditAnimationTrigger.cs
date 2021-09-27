using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabCreditAnimationTrigger : CreditAnimationTrigger
{
    enum CrabAnimation { Idle, MoveLeft, MoveRight }
    [SerializeField]
    CrabAnimation startState;
    [SerializeField]
    CrabAnimation triggerState;

    public override void Start()
    {
        base.Start();
        UpdateAnimator(startState);
    }

    public override void TriggerAnimation()
    {
        base.TriggerAnimation();
        UpdateAnimator(triggerState);
    }
    void UpdateAnimator(CrabAnimation toChangeTo)
    {
        switch (toChangeTo)
        {
            case CrabAnimation.Idle:
                animCont.SetBool("IsMoving", false);
                animCont.Play("Idle");
                break;
            case CrabAnimation.MoveLeft:
                animCont.SetBool("IsMoving", true);
                animCont.Play("CrabWalkLeft");
                break;
            case CrabAnimation.MoveRight:
                animCont.SetBool("IsMoving", true);
                animCont.Play("CrabWalkRight");
                break;
        }
    }
}
