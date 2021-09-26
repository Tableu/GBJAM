using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LoadingScreen : MonoBehaviour
{
    public delegate void showCallbackEvent();
    RectTransform backgroundImage;
    TextMeshProUGUI textThing;
    RectTransform textRect;

    [SerializeField, Range(0, 5f)]
    internal float transitionTime = 1f;
    [SerializeField]
    float fullBackgroundWidth = 480;
    [SerializeField, Range(1f, 100f)]
    float textScrollSpeed = 5f;
    [SerializeField, Range(20f, 200f)]
    float spedUpTextScrollSpeed = 100f;
    [SerializeField, Range(50f, 400f)]
    float skipTextScrollSpeed = 300f;

    [SerializeField]
    LoadingScreenText textsToShow;

    bool isShowing = false;
    bool isHiding = false;

    bool isScrollingText = false;

    bool hideWhenShowFinish = false;

    showCallbackEvent savedCallbackEvents;
    showCallbackEvent backupCallbackEvents;

    private PlayerInputActions _playerInputActions;

    [SerializeField]
    bool _isScreenShown;
    public bool IsScreenShown
    {
        get { return _isScreenShown; }
    }
    public bool IsShowing
    {
        get { return isShowing; }
    }
    public bool IsHiding
    {
        get { return isHiding; }
    }
    public bool IsScrollingText
    {
        get { return isScrollingText; }
    }
    /// <summary>
    /// If it should be blocking other behaviour because you're in a loading screen silly, why would you be able to move stuff around.
    /// </summary>
    public bool ShouldBlockStuff
    {
        get { return _isScreenShown || isShowing || isScrollingText; }
    }

    static LoadingScreen _instance;
    public static LoadingScreen Instance
    {
        get
        {
            if (_instance == null)
            {
                if (!FindObjectOfType<LoadingScreen>())
                {
                    _instance = (Instantiate(Resources.Load("UI/LoadingScreen")) as GameObject).GetComponent<LoadingScreen>();
                    _instance.gameObject.name = "LoadingScreen";
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        _playerInputActions = new PlayerInputActions();
        backgroundImage = transform.Find("Background").GetComponent<RectTransform>();
        textThing = transform.Find("LevelText").GetComponent<TextMeshProUGUI>();
        textRect = textThing.GetComponent<RectTransform>();

        Reset();
    }

    public void Reset()
    {
        StopAllCoroutines();
        _isScreenShown = false;
        isShowing = false;
        isHiding = false;
        isScrollingText = true;
        backgroundImage.sizeDelta = new Vector2(32f, backgroundImage.sizeDelta.y);
        textThing.text = "";
        GetComponent<GraphicRaycaster>().enabled = false;
    }

    private void SetCamera()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.worldCamera = FindObjectOfType<Camera>();
    }

    public void ShowLoadingScreen(showCallbackEvent callbackEvent = null)
    {
        if (!_isScreenShown && !isShowing)
        {
            SetCamera();
            savedCallbackEvents = callbackEvent;
            StartCoroutine(ShowLoadingScreenCoroutine());
        }
    }
    public void HideLoadingScreen(showCallbackEvent callbackEvent = null)
    {
        if ((_isScreenShown && !isHiding) || isShowing)
        {
            SetCamera();
            textRect.anchoredPosition = new Vector2(150, 0);
            textThing.text = "";
            textThing.enabled = false;
            if (isShowing)
            {
                hideWhenShowFinish = true;
                backupCallbackEvents = callbackEvent;
            }
            else
            {
                savedCallbackEvents = callbackEvent;
                StartCoroutine(HideLoadingScreenCoroutine());
            }
        }
    }


    IEnumerator ShowLoadingScreenCoroutine()
    {
        isShowing = true;
        yield return StartCoroutine(ShowBackground(true));
        isShowing = false;
        yield return StartCoroutine(ScrollTextCoroutine());

        HandleCallbacks();
    }
    IEnumerator HideLoadingScreenCoroutine()
    {
        isHiding = true;
        yield return StartCoroutine(ShowBackground(false));

        isHiding = false;
    }

    IEnumerator ScrollTextCoroutine()
    {
        textThing.enabled = true;
        textThing.text = GetLevelText();
        if (textThing.text != "")
        {
            MusicManager.MusicInstance.PlayMusic(Music.TransitionJingle);
            yield return null;
            textRect.anchoredPosition = new Vector2(0, -textRect.sizeDelta.y * 0.5f - 72f);

            float targetPos = -textRect.anchoredPosition.y;
            float finalSpeed = textScrollSpeed;
            _playerInputActions.Enable();
            while (textRect.anchoredPosition.y < targetPos)
            {
                //if (_playerInputActions.UI.Submit.)
                if (_playerInputActions.UI.Submit.phase.ToString() == "Started" || _playerInputActions.UI.Cancel.phase.ToString() == "Started")
                {
                    finalSpeed = skipTextScrollSpeed;
                    MusicManager.MusicInstance.SetAudioSourceSpeed(1.75f);
                }
                else
                {
                    finalSpeed = textScrollSpeed;
                    MusicManager.MusicInstance.SetAudioSourceSpeed(1f);
                }
                //Debug.Log($"InputActionPhase: {_playerInputActions.UI.Submit.phase}\r\nTriggered: {_playerInputActions.UI.Submit.triggered}");

                //finalSpeed += textScrollSpeed;

                SetTextPosition(textRect.anchoredPosition.y + finalSpeed * Time.unscaledDeltaTime);
                yield return null;
            }
            SetTextPosition(targetPos);
        }
        isScrollingText = false;
        _playerInputActions.Disable();
    }
    IEnumerator ShowBackground(bool show)
    {
        float currentTime = 0;
        float currentPercentage = 0;
        float startValue = show ? 32f : fullBackgroundWidth;
        float targetValue = show ? fullBackgroundWidth : 32f;
        float transitionDuration = show ? transitionTime : transitionTime;
        backgroundImage.sizeDelta = new Vector2(startValue, backgroundImage.sizeDelta.y);

        while (currentTime <= transitionTime)
        {
            currentTime += Time.unscaledDeltaTime;
            currentPercentage = Mathf.Clamp(currentTime / transitionDuration, 0, 1f);
            float newWidth = show ? (targetValue - 32f) * currentPercentage : (fullBackgroundWidth - 32f) * (1f - currentPercentage);

            backgroundImage.sizeDelta = new Vector2(newWidth + 32f, backgroundImage.sizeDelta.y);
            yield return null;
        }
        backgroundImage.sizeDelta = new Vector2(targetValue, backgroundImage.sizeDelta.y);
        _isScreenShown = show;
    }

    void HandleCallbacks()
    {
        Debug.Log("Handling callbacks!");
        if (savedCallbackEvents != null)
        {
            savedCallbackEvents.Invoke();
            savedCallbackEvents = null;
        }
    }

    string GetLevelText()
    {
        string sceneName = SceneNavigationManager.Instance.GetCurrentlyActiveScene().name;
        string result = "String not found\r\nBlame Iojioji lmao";
        switch (sceneName)
        {
            case "MainScene":
                result = textsToShow.IntroText;
                break;
            case "Level1":
                result = textsToShow.Level1Text;
                break;
            case "Level2":
                result = textsToShow.Level2Text;
                break;
            case "Level3":
                result = textsToShow.Level3Text;
                break;
        }
        return result;
    }

    void SetTextPosition(float yPos)
    {
        textRect.anchoredPosition = new Vector2(textRect.anchoredPosition.x, yPos);
    }
    float GetEquivalentProgress(float minLimit, float maxLimit, float currentValue)
    {
        float auxMin = minLimit - minLimit;
        float auxMax = maxLimit - minLimit;
        float auxVal = currentValue - minLimit;

        if (auxVal >= auxMax)
        {
            return 1f;
        }

        float newProgress = 1f / auxMax * auxVal;
        return newProgress;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(LoadingScreen))]
public class LoadingScreenEditor : Editor
{
    LoadingScreen loadScreen { get { return target as LoadingScreen; } }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (Application.isPlaying)
        {
            EditorExtensionMethods.DrawSeparator(Color.gray);

            if (GUILayout.Button("Show loading screen"))
            {
                loadScreen.ShowLoadingScreen();
            }
            if (GUILayout.Button("Hide loading screen"))
            {
                loadScreen.HideLoadingScreen();
            }
        }
    }
}
#endif