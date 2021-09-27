using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PufferfishCreditAnimationTrigger : CreditAnimationTrigger
{
    [SerializeField]
    bool startPuffed = false;
    [SerializeField]
    bool isSmall = false;
    public override void Start()
    {
        base.Start();
        isSmall = !startPuffed;
        animCont.SetBool("IsSmall", isSmall);
    }
    public override void TriggerAnimation()
    {
        base.TriggerAnimation();
        animCont.SetBool("IsSmall", !isSmall);
    }
}
