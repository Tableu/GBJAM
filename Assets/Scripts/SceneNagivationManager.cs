using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneNagivationManager : MonoBehaviour
{

    static SceneNagivationManager _instance;

    public static SceneNagivationManager Instance
    {
        get
        {
            if (_instance == null && !FindObjectOfType<SceneNagivationManager>())
            {
                _instance = new GameObject("SceneNavigationManager").AddComponent<SceneNagivationManager>();
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

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        HUDManager.Instance.Show(false);
    }
    public void GoToIojiojiTestScene()
    {
        SceneManager.LoadScene("IojiojiTestScene");
        ///If you're gonna show the HUD, preferably do it when you're already inside a scene that uses it. Hiding it before is cool but do call show after (or manually update each value from inside the scene).
        HUDManager.Instance.Show(true);
    }
    public void GoToTableuTestScene()
    {
        SceneManager.LoadScene("TableuTest");
        ///If you're gonna show the HUD, preferably do it when you're already inside a scene that uses it. Hiding it before is cool but do call show after (or manually update each value from inside the scene).
        HUDManager.Instance.Show(true);
    }
}