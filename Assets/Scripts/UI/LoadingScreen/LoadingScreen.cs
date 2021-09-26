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

    [SerializeField, Range(0, 5f)]
    internal float transitionTime = 1f;
    [SerializeField]
    float fullBackgroundWidth = 480;


    bool isShowing = false;
    bool isHiding = false;

    bool hideWhenShowFinish = false;

    showCallbackEvent savedCallbackEvents;
    showCallbackEvent backupCallbackEvents;

    [SerializeField]
    bool _isScreenShown;
    public bool IsScreenShown
    {
        get { return _isScreenShown; }
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

        backgroundImage = transform.Find("Background").GetComponent<RectTransform>();

        Reset();
    }

    public void Reset()
    {
        StopAllCoroutines();
        _isScreenShown = false;
        isShowing = false;
        isHiding = false;
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

        HandleCallbacks();

        //---------------------------------------------------------------------------------------
        //isShowing = true;
        ////yield return StartCoroutine(ChangeLoadingScreenAlpha(1));
        //yield return null;

        //HandleCallbacks();

        //isShowing = false;
        //if (hideWhenShowFinish)
        //{
        //    hideWhenShowFinish = false;
        //    savedCallbackEvents = backupCallbackEvents;
        //    backupCallbackEvents = null;
        //    HideLoadingScreen();
        //}
    }
    IEnumerator HideLoadingScreenCoroutine()
    {
        isHiding = true;
        yield return StartCoroutine(ShowBackground(false));

        isHiding = false;
        //---------------------------------------------------------------------------------------
        //isHiding = true;
        ////yield return new WaitForSeconds(timeBeforeHidingTexts);
        ////yield return StartCoroutine(ChangeLoadingScreenAlpha(0));
        //yield return null;
        //GetComponent<GraphicRaycaster>().enabled = false;
        //HandleCallbacks();

        //isHiding = false;
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
    //IEnumerator ChangeLoadingScreenAlpha(float value)
    //{
    //    isTransitioning = true;
    //    float currentTime = 0;
    //    float currentPercentage = 0;
    //    float startValue = screenCanvasGroup.alpha;

    //    while (currentTime <= fadeTime)
    //    {
    //        currentTime += Time.deltaTime;
    //        currentPercentage = Mathf.Clamp(currentTime / fadeTime, 0f, 1f);

    //        ////screenCanvasGroup.alpha = func(startValue, value, currentPercentage);
    //        yield return null;
    //    }
    //    _isScreenShown = value == 1f ? true : false;
    //    isTransitioning = false;
    //}

    void HandleCallbacks()
    {
        if (savedCallbackEvents != null)
        {
            savedCallbackEvents.Invoke();
            savedCallbackEvents = null;
        }
    }

    //public void ShowHideTexts()
    //{
    //    StopCoroutine(ShowTextsComplete());
    //    StopCoroutine(ShowText(episodeText));
    //    StopCoroutine(ShowText(titleText));
    //    StopCoroutine(HideTexts(new Text[] { episodeText, titleText }));

    //    StartCoroutine(ShowTextsComplete());
    //}

    //IEnumerator ShowTextsComplete()
    //{
    //    float startTime = Time.time;
    //    Debug.Log($"Starting ({startTime})");
    //    yield return StartCoroutine(ShowText(episodeText));
    //    yield return new WaitForSeconds(timeBetweenTexts);
    //    yield return StartCoroutine(ShowText(titleText));
    //    yield return new WaitForSeconds(timeBeforeHidingTexts);
    //    yield return StartCoroutine(HideTexts(new Text[] { episodeText, titleText }));
    //    Debug.Log($"Ended with: {Time.time - startTime} ({Time.time})");
    //}

    //void WriteEpisodeTexts()
    //{
    //    episodeText.text = episodeString;
    //    titleText.text = titleString;
    //}

    //IEnumerator ShowText(Text toShow)
    //{
    //    UIEffect effect = toShow.GetComponent<UIEffect>();
    //    float currentTime = 0;
    //    float progress = 0;

    //    do
    //    {
    //        currentTime += Time.deltaTime;
    //        progress = currentTime / showTextDuration;

    //        ChangeTextAlpha(toShow, GetEquivalentProgress(showAlphaMinVal, showAlphaMaxVal, progress));
    //        effect.colorFactor = 1 - GetEquivalentProgress(showColorMinVal, showColorMaxVal, progress);
    //        effect.blurFactor = 1 - GetEquivalentProgress(showBlurMinVal, showBlurMaxVal, progress);
    //        //Debug.Log($"Prog: {progress:F4}, text alpha: {toShow.color.a:F4}");
    //        yield return null;
    //    } while (currentTime < showTextDuration);
    //}

    //IEnumerator HideTexts(Text[] toHide)
    //{
    //    UIEffect[] effects = new UIEffect[] { toHide[0].GetComponent<UIEffect>(), toHide[1].GetComponent<UIEffect>() };
    //    float currentTime = 0;
    //    float progress = 0;

    //    do
    //    {
    //        currentTime += Time.deltaTime;
    //        progress = currentTime / hideTextDuration;

    //        for (int i = 0; i < toHide.Length; i++)
    //        {
    //            ChangeTextAlpha(toHide[i], 1 - GetEquivalentProgress(hideAlphaMinVal, hideAlphaMaxVal, progress));
    //            effects[i].colorFactor = GetEquivalentProgress(hideColorMinVal, hideColorMaxVal, progress);
    //            effects[i].blurFactor = GetEquivalentProgress(hideBlurMinVal, hideBlurMaxVal, progress);
    //        }
    //        yield return null;
    //    } while (currentTime <= hideTextDuration);
    //}

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