using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemyAnimatorController : AnimationControllerBase
{
    public void SetIsMoving(bool moving)
    {
        animCont.SetBool("IsMoving", moving);
    }
    public void TriggerHurt()
    {
        animCont.SetTrigger("Hit");
    }
    public void TriggerAttack()
    {
        animCont.SetTrigger("Attack");
    }
    public void TriggerDeath()
    {
        animCont.SetTrigger("Death");
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(EnemyAnimatorController))]
class EnemyAnimatorEditor : Editor
{
    EnemyAnimatorController anim { get { return target as EnemyAnimatorController; } }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (Application.isPlaying)
        {
            EditorExtensionMethods.DrawSeparator(Color.gray);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set IsMoving True"))
            {
                anim.SetIsMoving(true);
            }
            if (GUILayout.Button("Set IsMoving False"))
            {
                anim.SetIsMoving(false);
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Trigger Attack"))
            {
                anim.TriggerAttack();
            }
            if (GUILayout.Button("Trigger Hurt"))
            {
                anim.TriggerHurt();
            }
            if (GUILayout.Button("Trigger Death"))
            {
                anim.TriggerDeath();
            }
        }
    }
}
#endif