using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigationManager : MonoBehaviour
{

    static SceneNavigationManager _instance;
    public int difficulty = NORMAL;
    public const int NORMAL = 0;
    public const int HARD = 1;
    public const int VERYHARD = 2;
    private string[] diffStrings = {"Normal", "Hard", "VeryHard"};

    public static SceneNavigationManager Instance
    {
        get
        {
            if (_instance == null && !FindObjectOfType<SceneNavigationManager>())
            {
                _instance = new GameObject("SceneNavigationManager").AddComponent<SceneNavigationManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        _instance = this;
        DontDestroyOnLoad(this);
    }

    public Scene GetCurrentlyActiveScene()
    {
        return SceneManager.GetActiveScene();
    }

    public void GoToMainMenu(float delay = 0)
    {
        if (delay == 0)
        {
            SwitchScene("MainScene", false, Music.MainMenu);
        }
        else
        {
            StartCoroutine(SwitchSceneAfterTime("MainScene", false, Music.MainMenu));
        }
    }
    public void GoToIojiojiTestScene(float delay = 0)
    {
        if (delay == 0)
        {
            SwitchScene("IojiojiTestScene", true, Music.Level1);
        }
        else
        {
            StartCoroutine(SwitchSceneAfterTime("IojiojiTestScene", true, Music.Level1));
        }
    }
    public void GoToTableuTestScene(float delay = 0)
    {
        if (delay == 0)
        {
            SwitchScene("TableuTest", true, Music.Level1);
        }
        else
        {
            StartCoroutine(SwitchSceneAfterTime("TableuTest", true, Music.Level1));
        }
    }
    public void GoToLevel1(float delay = 0)
    {
        string scene = "Level1-" + diffStrings[difficulty];
        
        if (delay == 0)
        {
            SwitchScene(scene, true, Music.Level1);
        }
        else
        {
            StartCoroutine(SwitchSceneAfterTime(scene, true, Music.Level1));
        }
    }
    public void GoToLevel2(float delay = 0)
    {
        string scene = "Level2-" + diffStrings[difficulty];
        if (delay == 0)
        {
            SwitchScene(scene, true, Music.Level2);
        }
        else
        {
            StartCoroutine(SwitchSceneAfterTime(scene, true, Music.Level2));
        }
    }
    public void GoToLevel3(float delay = 0)
    {
        string scene = "Level3-" + diffStrings[difficulty];
        if (delay == 0)
        {
            SwitchScene(scene, true, Music.Level3);
        }
        else
        {
            StartCoroutine(SwitchSceneAfterTime(scene, true, Music.Level3));
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator SwitchSceneAfterTime(string sceneName, bool showHUD, Music music)
    {
        yield return new WaitForSeconds(1f);
        SwitchScene(sceneName, showHUD, music);
    }
    void SwitchScene(string sceneName, bool showHUD, Music music)
    {
        SceneManager.LoadScene(sceneName);
        MusicManager.MusicInstance.PlayMusic(music);
        ///If you're gonna show the HUD, preferably do it when you're already inside a scene that uses it. Hiding it before is cool but do call show after (or manually update each value from inside the scene).
        HUDManager.Instance.Show(showHUD);
    }

    public void GoToNextLevel()
    {
        string currentScene = GetCurrentlyActiveScene().name;
        switch (currentScene)
        {
            case "Level1":
                //GoToMainMenu();
                GoToLevel2();
                break;
            case "Level2":
                GoToLevel3();
                break;
            default:
                Debug.Log($"Uuuh what? How did you manage to trigger a level change if you're not on one of the levels? o:\r\nFrom: '{currentScene}'");
                GoToMainMenu();
                break;
        }
    }
}