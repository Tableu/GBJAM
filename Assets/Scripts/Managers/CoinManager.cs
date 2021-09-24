using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum Levels { Level1, Level2, Level3 }

public class CoinManager : MonoBehaviour
{
    [SerializeField]
    LevelStats level1Stats = new LevelStats(Levels.Level1);
    [SerializeField]
    LevelStats level2Stats = new LevelStats(Levels.Level2);
    [SerializeField]
    LevelStats level3Stats = new LevelStats(Levels.Level3);


    static CoinManager _instance;
    public static CoinManager Instance
    {
        get 
        {
            if (_instance == null && !FindObjectOfType<CoinManager>())
            {
                _instance = new GameObject("GameManager").AddComponent<CoinManager>();
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
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnLevelLoaded;
    }


    public void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"A new level was loaded! {scene.name}");
        switch (scene.name)
        {
            case "Level1":
                SetupLevel1();
                break;
            case "Level2":
                SetupLevel2();
                break;
        }
    }

    void SetupLevel1()
    {
        //level1Stats.SetTotalCoins(FindObjectsOfType<Coin>().Length);
    }
    public void EndLevel1()
    {

    }
    void SetupLevel2()
    {

    }
    public void EndLevel2()
    {

    }
}

[System.Serializable]
public class LevelStats
{
    [SerializeField]
    Levels level;
    [SerializeField]
    int totalCoins = 0;
    [SerializeField]
    int coinsCollected = 0;

    public int TotalCoins
    {
        get { return totalCoins; }
    }
    public int CoinsCollected
    {
        get { return coinsCollected; }
    }

    public LevelStats(Levels level)
    {
        this.level = level;
    }

    public void SetTotalCoins(int newCoins)
    {
        totalCoins = newCoins;
    }
    public void OverrideCoins(int newCoins)
    {
        coinsCollected = newCoins;
    }
    public void AddCoins(int toAdd)
    {
        coinsCollected += toAdd;
    }
}