using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MapManager : MonoBehaviour
{
    Transform confinerContainer;
    PlayerController player;

    [SerializeField]
    AudioSource bgmPlayer;

    bool isEndingLevel = false;
    public bool IsEndingLevel
    {
        get { return isEndingLevel; }
    }

    static MapManager _instance;
    public static MapManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        _instance = this;

        bgmPlayer = GetComponent<AudioSource>();
    }


    void Start()
    {
        pSoundManager.ClearNonexistentSources();

        confinerContainer = transform.Find("CameraConfiner");

        player = FindObjectOfType<PlayerController>();

        HUDManager.Instance.Show(true);
        if (player)
        {
            HUDManager.Instance.UpdateHealth(player.Health);
            HUDManager.Instance.UpdateArmor(player.Armor);
            //TODO: Update coins.
        }

        if (confinerContainer)
        {
            CinemachineConfiner2D camConfiner = FindObjectOfType<CinemachineConfiner2D>();
            camConfiner.m_BoundingShape2D = confinerContainer.GetComponent<PolygonCollider2D>();
        }

        //UpdateCameraBG();
    }

    public void PlayerDied()
    {
        StartCoroutine(ReloadSceneAfterSeconds());
    }
    IEnumerator ReloadSceneAfterSeconds()
    {
        yield return new WaitForSeconds(2f);
        SceneNavigationManager.Instance.ReloadScene();
    }

    void UpdateCameraBG()
    {
        Transform mainCamera = Camera.main.transform;
        if (mainCamera.Find("LevelBG"))
        {
            Destroy(mainCamera.Find("LevelBG").gameObject);
        }
        Transform bgObject = GameObject.Find("- - - World - - -").transform.Find("LevelBG");
        bgObject.SetParent(mainCamera);
        bgObject.localPosition = new Vector3(0, 0, 10f);
    }

    IEnumerator LevelEndCoroutine()
    {
        MusicManager.MusicInstance.PlayMusic(Music.EndJingle, false);
        yield return new WaitForSeconds(7.25f);
        SceneNavigationManager.Instance.GoToNextLevel();
    }

    public void EndLevel()
    {
        isEndingLevel = true;
        StartCoroutine(LevelEndCoroutine());
    }
}
