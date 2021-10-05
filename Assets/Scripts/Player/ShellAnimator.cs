using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellAnimator : AnimationControllerBase
{
    public void SetIsDead()
    {
        animCont.SetBool("Dead", true);
        pSoundManager.PlaySound(pSoundManager.Sound.shellDestroy);
    }

    public void SetIsDamaged()
    {
        animCont.SetBool("Damaged", true);
    }

    private void Update()
    {
        if (gameObject.GetComponent<ShellStats>().armor == 1)
        {
            gameObject.GetComponent<ShellAnimator>().SetIsDamaged();
        }
    }
}
