using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LoadingScreen : MonoBehaviour
{
    public delegate void showCallbackEvent();
    RectTransform backgroundImage;
    TextMeshProUGUI textThing;
    RectTransform textRect;
    RectTransform creditsRect;

    [SerializeField, Range(0, 5f)]
    internal float transitionTime = 1f;
    [SerializeField]
    float fullBackgroundWidth = 480;
    [SerializeField, Range(1f, 100f), Header("Text Scrolling Speeds")]
    float textScrollSpeed = 5f;
    [SerializeField, Range(20f, 200f)]
    float spedUpTextScrollSpeed = 100f;
    [SerializeField, Range(50f, 400f)]
    float skipTextScrollSpeed = 300f;
    [SerializeField, Range(1f, 100f), Header("Credits Scrolling Speeds")]
    float creditsScrollSpeed = 15f;
    [SerializeField, Range(20f, 200f)]
    float spedUpDreditsScrollSpeed = 100f;
    [SerializeField, Range(50f, 400f)]
    float skipCreditsScrollSpeed = 300f;

    [SerializeField]
    LoadingScreenText textsToShow;

    bool isShowing = false;
    bool isHiding = false;

    bool isScrollingText = false;

    bool areCreditsRolling = false;

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
    public bool AreCreditsRolling
    {
        get { return areCreditsRolling; }
    }
    /// <summary>
    /// If it should be blocking other behaviour because you're in a loading screen silly, why would you be able to move stuff around.
    /// </summary>
    public bool ShouldBlockStuff
    {
        get { return _isScreenShown || isShowing || isScrollingText || areCreditsRolling; }
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
        creditsRect = transform.Find("CreditsContainer").GetComponent<RectTransform>();

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
        EnableAllCreditAnimationTriggers(false);
        GetComponent<GraphicRaycaster>().enabled = false;
    }

    private void SetCamera()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.worldCamera = FindObjectOfType<Camera>();
    }

    public void ShowLoadingScreen(showCallbackEvent callbackEvent = null, string levelOverride = "")
    {
        if (!_isScreenShown && !isShowing)
        {
            SetCamera();
            savedCallbackEvents = callbackEvent;
            StartCoroutine(ShowLoadingScreenCoroutine(levelOverride));
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


    IEnumerator ShowLoadingScreenCoroutine(string levelOverride)
    {
        isShowing = true;
        yield return StartCoroutine(ShowBackground(true));
        isShowing = false;
        if (SceneNavigationManager.Instance.GetCurrentlyActiveScene().name == "Scene3" || levelOverride == "Credits")
        {
            yield return StartCoroutine(ScrollCreditsCoroutine());
        }
        else
        {
            yield return StartCoroutine(ScrollTextCoroutine(levelOverride));
        }

        HandleCallbacks();
    }
    IEnumerator HideLoadingScreenCoroutine()
    {
        isHiding = true;
        yield return StartCoroutine(ShowBackground(false));

        isHiding = false;
    }

    IEnumerator ScrollTextCoroutine(string levelOverride)
    {
        textThing.enabled = true;
        textThing.text = GetLevelText(levelOverride);
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

                SetObjectVerticalAnchoredPosition(textRect, textRect.anchoredPosition.y + finalSpeed * Time.unscaledDeltaTime);
                yield return null;
            }
            SetObjectVerticalAnchoredPosition(textRect, targetPos);
        }
        isScrollingText = false;
        _playerInputActions.Disable();
    }
    IEnumerator ScrollCreditsCoroutine()
    {
        areCreditsRolling = true;
        EnableAllCreditAnimationTriggers(true);
        MusicManager.MusicInstance.PlayMusic(Music.Credits);
        creditsRect.anchoredPosition = new Vector2(0, -creditsRect.sizeDelta.y * 0.5f - 72f);

        float targetPos = -creditsRect.anchoredPosition.y;
        float finalSpeed = creditsScrollSpeed;
        _playerInputActions.Enable();
        while (creditsRect.anchoredPosition.y < targetPos)
        {
            SetObjectVerticalAnchoredPosition(creditsRect, creditsRect.anchoredPosition.y + finalSpeed * Time.unscaledDeltaTime);
            yield return null;
        }
        SetObjectVerticalAnchoredPosition(creditsRect, targetPos);
        areCreditsRolling = false;
        EnableAllCreditAnimationTriggers(false);
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

    void EnableAllCreditAnimationTriggers(bool enable)
    {
        List<CreditAnimationTrigger> triggers = new List<CreditAnimationTrigger>();
        triggers.AddRange(FindObjectsOfType<CreditAnimationTrigger>());
        foreach (CreditAnimationTrigger trigger in triggers)
        {
            trigger.EnableBehaviour = enable;
        }
    }

    string GetLevelText(string overrideSceneName)
    {
        string sceneName = overrideSceneName;
        if (sceneName == "")
        {
            sceneName = SceneNavigationManager.Instance.GetCurrentlyActiveScene().name;
        }
        string result = "String not found\r\nBlame Iojioji lmao";
        if (sceneName.Equals("MainScene"))
        {
            result = textsToShow.IntroText;
        }
        else
        {
            switch (sceneName.Substring(0,6))
            {
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
        }

        return result;
    }

    void SetObjectVerticalAnchoredPosition(RectTransform transformToSet, float yPos)
    {
        transformToSet.anchoredPosition = new Vector2(transformToSet.anchoredPosition.x, yPos);
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
            if (loadScreen.IsScreenShown)
            {
                if (GUILayout.Button("Hide loading screen"))
                {
                    loadScreen.HideLoadingScreen();
                }
            }
            else
            {
                if (GUILayout.Button("Show loading screen"))
                {
                    loadScreen.ShowLoadingScreen();
                }
            }
            EditorExtensionMethods.DrawSeparator(Color.gray);
            if (!loadScreen.IsScreenShown)
            {
                if (GUILayout.Button("Show loading screen (Credits)"))
                {
                    loadScreen.ShowLoadingScreen(null, "Credits");
                }
            }
        }
    }
}
#endif