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
    [SerializeField]
    protected float startSpeed = 1f;
    [SerializeField]
    protected float triggerSpeed = 1f;
    public bool EnableBehaviour
    {
        get { return enableBehaviour; }
        set { enableBehaviour = value; }
    }

    public virtual void Awake()
    {
        animCont = GetComponent<Animator>();
    }
    public virtual void Start()
    {
        animCont.speed = startSpeed;
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
        animCont.speed = triggerSpeed;
        //Do Something;
    }
}
