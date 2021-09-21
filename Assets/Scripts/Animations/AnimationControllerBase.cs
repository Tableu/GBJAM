using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControllerBase : MonoBehaviour
{
    protected Animator animCont;

    private void Awake()
    {
        animCont = GetComponent<Animator>();
    }
}
