using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditAnimationTrigger : MonoBehaviour
{
    [SerializeField]
    protected bool enableBehaviour = false;
    protected Animator animCont;
    [SerializeField, Range(-9, 8)]
    protected float triggerHeight = 0;
    public bool EnableBehaviour
    {
        get { return enableBehaviour; }
        set { enableBehaviour = value; }
    }

    public virtual void Awake()
    {
        animCont = GetComponent<Animator>();
    }
    public virtual void Update()
    {
        if (enableBehaviour)
        {
            if (transform.position.y >= triggerHeight)
            {
                TriggerAnimation();
            }
        }
    }
    public virtual void TriggerAnimation()
    {
        Debug.Log($"This dude should have triggered by now! '{name}'");
        //Do Something;
    }
}
