using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// General extension methods used for custom editor stuff
/// </summary>
public static class EditorExtensionMethods
{
    /// <summary>
    /// Draws a horizontal line on the editor when called on OnInspectorGUI() on custom editors
    /// </summary>
    /// <param name="color">Color of separator line</param>
    /// <param name="thickness">Thickness of line (defaults to 2)</param>
    /// <param name="padding">Padding of line (Defaults to 10)</param>
    public static void DrawSeparator(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect();
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        EditorGUI.DrawRect(r, color);
    }
}
#endif