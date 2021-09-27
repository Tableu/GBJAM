using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCreditAnimationTrigger : CreditAnimationTrigger
{
    enum PlayerAnimation { Idle, Crouching, Moving, Dancing, CrouchMoving, Dying }
    [SerializeField]
    PlayerAnimation startState;
    [SerializeField]
    PlayerAnimation triggerState;
    // Start is called before the first frame update
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

    void UpdateAnimator(PlayerAnimation toChangeTo)
    {
        switch (toChangeTo)
        {
            case PlayerAnimation.Idle:
                animCont.SetBool("IsGrounded", true);
                animCont.SetBool("IsMoving", false);
                animCont.SetBool("IsHiding", false);
                animCont.SetBool("IsDancing", false);
                animCont.Play("PlayerIdle");
                break;
            case PlayerAnimation.Crouching:
                animCont.SetBool("IsGrounded", true);
                animCont.SetBool("IsMoving", false);
                animCont.SetBool("IsHiding", true);
                animCont.SetBool("IsDancing", false);
                animCont.Play("PlayerHide");
                break;
            case PlayerAnimation.CrouchMoving:
                animCont.SetBool("IsGrounded", true);
                animCont.SetBool("IsMoving", true);
                animCont.SetBool("IsHiding", true);
                animCont.SetBool("IsDancing", false);
                animCont.Play("PlayerHideWalk");
                break;
            case PlayerAnimation.Moving:
                animCont.SetBool("IsGrounded", true);
                animCont.SetBool("IsMoving", true);
                animCont.SetBool("IsHiding", false);
                animCont.SetBool("IsDancing", false);
                animCont.Play("PlayerWalk");
                break;
            case PlayerAnimation.Dancing:
                animCont.SetBool("IsGrounded", true);
                animCont.SetBool("IsMoving", false);
                animCont.SetBool("IsHiding", false);
                animCont.SetBool("IsDancing", true);
                animCont.Play("PlayerDance");
                break;
            case PlayerAnimation.Dying:
                animCont.SetTrigger("DeathTrigger");
                break;
        }
    }
}
