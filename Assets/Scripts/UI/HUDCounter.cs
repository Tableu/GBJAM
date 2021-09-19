using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class HUDCounter : MonoBehaviour
{
    [SerializeField]
    Image imageUnits;
    [SerializeField]
    Image imageTens;



    private void Awake()
    {
        Transform auxImageUnits = transform.Find("CounterUnits");
        imageUnits = auxImageUnits ? auxImageUnits.GetComponent<Image>() : null;
        Transform auxImageTens = transform.Find("CounterTens");
        imageTens = auxImageTens ? auxImageTens.GetComponent<Image>() : null;
    }

    public void SetValue(Sprite[] toSet)
    {
        if (imageTens)
        {
            imageTens.sprite = toSet[0];
        }
        if (imageUnits)
        {
            imageUnits.sprite = toSet[1];
        }
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(HUDCounter))]
class HUDCounterEditor : Editor
{
    HUDCounter counter { get { return target as HUDCounter; } }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (Application.isPlaying)
        {
            EditorExtensionMethods.DrawSeparator(Color.gray);
        }
    }
}
#endif