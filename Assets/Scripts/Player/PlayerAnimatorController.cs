using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerAnimatorController : AnimationControllerBase
{

    public void SetIsGrounded(bool grounded)
    {
        animCont.SetBool("IsGrounded", grounded);
    }
    public void SetIsMoving(bool moving)
    {
        animCont.SetBool("IsMoving", moving);
    }
    public void SetIsHiding(bool hiding)
    {
        animCont.SetBool("IsHiding", hiding);
    }
    public void TriggerJump()
    {
        animCont.SetTrigger("JumpTrigger");
    }

    public void TriggerAttack()
    {
        animCont.SetTrigger("AttackTrigger");
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(PlayerAnimatorController))]
class PlayerAnimatorEditor : Editor
{
    PlayerAnimatorController anim { get { return target as PlayerAnimatorController; } }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (Application.isPlaying)
        {
            EditorExtensionMethods.DrawSeparator(Color.gray);
            if (GUILayout.Button("Set IsGrounded True"))
            {
                anim.SetIsGrounded(true);
            }
            if (GUILayout.Button("Set IsGrounded False"))
            {
                anim.SetIsGrounded(false);
            }
            EditorExtensionMethods.DrawSeparator(Color.gray);
            if (GUILayout.Button("Set IsMoving True"))
            {
                anim.SetIsMoving(true);
            }
            if (GUILayout.Button("Set IsMoving False"))
            {
                anim.SetIsMoving(false);
            }
        }
    }
}
#endif