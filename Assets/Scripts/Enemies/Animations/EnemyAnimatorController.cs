using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class EnemyAnimatorController : AnimationControllerBase
{
    public abstract void SetIsMoving(bool moving);
    public abstract void TriggerHurt();
    public abstract void TriggerAttack();
    public abstract void TriggerDeath();
    public abstract void IsAngry(bool angry);
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnemyAnimatorController), true)]
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
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set CanSee True"))
            {
                anim.IsAngry(true);
            }
            if (GUILayout.Button("Set CanSee False"))
            {
                anim.IsAngry(false);
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
