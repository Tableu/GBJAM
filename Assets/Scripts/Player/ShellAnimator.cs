using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellAnimator : AnimationControllerBase
{
    public void SetIsDead()
    {
        animCont.SetBool("Dead", true);
    }

    public void SetIsDamaged()
    {
        animCont.SetBool("Damaged", true);
    }
}
