using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanel : MonoBehaviour
{
    /// <summary>
    /// The selectable that will have focus when Showing
    /// </summary>
    [SerializeField]
    Selectable toSelectOnStart;

    /// <summary>
    /// Shows or hides UI panel
    /// If it's shown and toSelectOnStart isn't null, that control will have focus.
    /// </summary>
    /// <param name="show">True to show, false to hide.</param>
    public void Show(bool show)
    {
        gameObject.SetActive(show);
        if (show)
        {
            if (toSelectOnStart != null)
            {
                toSelectOnStart.Select();
            }
            else
            {
                Debug.LogError($"There was no selectable to select on object '{name}' unu");
            }
        }
        else
        {

        }
    }
}
