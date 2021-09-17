using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerAnimatorController : MonoBehaviour
{
    Animator animCont;

    private void Awake()
    {
        animCont = GetComponent<Animator>();
    }

    public void SetIsGrounded(bool grounded)
    {
        animCont.SetBool("IsGrounded", grounded);
    }
    public void SetIsMoving(bool moving)
    {
        animCont.SetBool("IsMoving", moving);
    }
    //Jump?? Depends on if we get multiple states of airborne (going up and going down or something like that)
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