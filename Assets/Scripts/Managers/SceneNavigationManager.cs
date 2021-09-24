using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneNavigationManager : MonoBehaviour
{

    static SceneNavigationManager _instance;

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

    public void GoToMainMenu()
    {
        StartCoroutine(SwitchSceneAfterTime("MainMenu", false));
    }
    public void GoToIojiojiTestScene()
    {
        StartCoroutine(SwitchSceneAfterTime("IojiojiTestScene", true));
    }
    public void GoToTableuTestScene()
    {
        StartCoroutine(SwitchSceneAfterTime("TableuTest", true));
    }
    public void GoToLevel1()
    {
        StartCoroutine(SwitchSceneAfterTime("Level1", true));
        MusicManager.MusicInstance.PlayMusic(Music.Level1);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator SwitchSceneAfterTime(string sceneName, bool showHUD)
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneName);
        ///If you're gonna show the HUD, preferably do it when you're already inside a scene that uses it. Hiding it before is cool but do call show after (or manually update each value from inside the scene).
        HUDManager.Instance.Show(showHUD);
    }

}