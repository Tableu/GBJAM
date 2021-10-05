using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
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
        PlayerPrefs.SetInt(PlayerController.COINS, 0);
        PlayerPrefs.SetInt(PlayerController.ARMOR, 0);
        PlayerPrefs.SetInt(PlayerController.SHELL, 0);
        LoadingScreen.Instance.HideLoadingScreen();
        MusicManager.MusicInstance.PlayMusic(Music.MainMenu);
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

    public void SetDifficultyNormal()
    {
        SceneNavigationManager.Instance.difficulty = SceneNavigationManager.NORMAL;
        PlayerPrefs.SetInt(PlayerController.HEALTH, 3);
        PlayerPrefs.SetInt(PlayerController.REDEEM_AMOUNT, 20);
    }

    public void SetDifficultyHard()
    {
        SceneNavigationManager.Instance.difficulty = SceneNavigationManager.HARD;
        PlayerPrefs.SetInt(PlayerController.HEALTH, 2);
        PlayerPrefs.SetInt(PlayerController.REDEEM_AMOUNT, 30);
    }

    public void SetDifficultyVeryHard()
    {
        SceneNavigationManager.Instance.difficulty = SceneNavigationManager.VERYHARD;
        PlayerPrefs.SetInt(PlayerController.HEALTH, 1);
        PlayerPrefs.SetInt(PlayerController.REDEEM_AMOUNT, 30);
    }
    public void GoToLevel1()
    {
        FindObjectOfType<InputSystemUIInputModule>().enabled = false;
        LoadingScreen.Instance.ShowLoadingScreen(LoadingScreenCallback);
        MusicManager.MusicInstance.PlayMusic(Music.TransitionJingle);
    }

    void LoadingScreenCallback()
    {
        SceneNavigationManager.Instance.GoToLevel1();
    }

    /// <summary>
    /// Kinda debug? Loads IojiojiTestSceney
    /// </summary>
    public void GoToIojiTestScene()
    {
        SceneNavigationManager.Instance.GoToIojiojiTestScene();
    }
    /// <summary>
    /// Kinda debug? Loads TableuTestScene
    /// </summary>
    public void GoToTableuTestScene()
    {
        SceneNavigationManager.Instance.GoToTableuTestScene();
    }
}
