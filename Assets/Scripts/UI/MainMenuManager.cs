using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainMenuManager : MonoBehaviour
{
    List<UIPanel> panels = new List<UIPanel>();

    /// <summary>
    /// Loads all panels inside the object (if panels is empty)
    /// 
    /// Hides all of the panels and then shows the first one on the list.
    /// </summary>
    void Start()
    {
        HUDManager.Instance.Show(false);
        if (panels.Count == 0)
        {
            LoadAllUIPanels();
        }
        HideAllPanels();
        if (panels.Count > 0)
        {
            panels[0].Show(true);
        }
    }

    /// <summary>
    /// Loads all panels into panels
    /// ContextMenu lets you call it from the editor (right click the script).
    /// </summary>
    [ContextMenu("Load all UIPanels")]
    public void LoadAllUIPanels()
    {
        panels.Clear();
        panels.AddRange(transform.GetComponentsInChildren<UIPanel>());
    }

    /// <summary>
    /// Hides all panels
    /// </summary>
    public void HideAllPanels()
    {
        foreach (UIPanel panel in panels)
        {
            panel.Show(false);
        }
    }

    /// <summary>
    /// Debug method, button navigation wasn't working for a moment hah
    /// </summary>
    /// <param name="message"></param>
    public void DebugButtonThing(string message)
    {
        Debug.Log($"Ayy click {message}");
    }

    /// <summary>
    /// Kinda debug? Loads IojiojiTestScene
    /// </summary>
    public void GoToIojiTestScene()
    {
        SceneNagivationManager.Instance.GoToIojiojiTestScene();
    }
    /// <summary>
    /// Kinda debug? Loads TableuTestScene
    /// </summary>
    public void GoToTableuTestScene()
    {
        SceneNagivationManager.Instance.GoToTableuTestScene();
    }
}
