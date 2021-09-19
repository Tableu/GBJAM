using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class HUDManager : MonoBehaviour
{
    [SerializeField]
    List<Sprite> numberSprites = new List<Sprite>();

    [SerializeField]
    HUDCounter healthCounter;
    [SerializeField]
    HUDCounter armorCounter;
    [SerializeField]
    HUDCounter coinCounter;

    //For debugging purposes, editor buttons use this var to set the number.
    [SerializeField]
    internal int numberToSet = 0;

    const int counterMaxDigits = 2;

    Image imageRenderer;

    bool isShown = false;

    static HUDManager _instance;
    public static HUDManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<HUDManager>(true);
                if (!_instance)
                {
                    Debug.LogError("Uhoh, something very wrong is going on lmao\r\nSomeone tried to call HUDManager but none was present");
                    //Maybe instantiate one if there is none? Shouldn't happen but we'll see.
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        //Maybe not a good idea to destroy the camera.
        //Also this should always be attached to the camera that uses HUDs, so this shouldn't be a problem.
        //if (Instance)
        //{
        //    Destroy(gameObject);
        //}
        _instance = this;

        imageRenderer = GetComponent<Image>();

        healthCounter = transform.Find("Health").GetComponent<HUDCounter>();
        armorCounter = transform.Find("Armor").GetComponent<HUDCounter>();
        coinCounter = transform.Find("Coins").GetComponent<HUDCounter>();
    }

    public void UpdateHealth(int newHealth)
    {
        if (!isShown)
        {
            Show(true);
        }
        healthCounter.SetValue(IntToSprite(newHealth));
    }
    public void UpdateArmor(int newArmor)
    {
        if (!isShown)
        {
            Show(true);
        }
        armorCounter.SetValue(IntToSprite(newArmor));
    }
    public void UpdateCoins(int newCoins)
    {
        if (!isShown)
        {
            Show(true);
        }
        coinCounter.SetValue(IntToSprite(newCoins));
    }

    ///If you're gonna show the HUD, preferably do it when you're already inside a scene that uses it. Hiding it before is cool but do call show after (or manually update each value from inside the scene).
    public void Show(bool show)
    {
        imageRenderer.enabled = show;
        healthCounter.gameObject.SetActive(show);
        armorCounter.gameObject.SetActive(show);
        coinCounter.gameObject.SetActive(show);
        if (show)
        {
            PlayerController playerCont = FindObjectOfType<PlayerController>();
            if (playerCont)
            {
                Debug.Log($"There's a player here! Updating stuff!");
                //TODO: Update UI values with player values
            }
        }
        isShown = show;
    }

    internal Sprite[] IntToSprite(int toConvert)
    {
        int toConvertLength = toConvert.ToString().Length;
        int startPos = 0;
        Sprite[] auxSprites = new Sprite[counterMaxDigits];
        if (toConvertLength == 1)
        {
            auxSprites[0] = numberSprites[0];
            auxSprites[1] = numberSprites[toConvert];
            return auxSprites;
        }
        else if (toConvertLength > 2)
        {
            startPos = toConvertLength - 2;
        }

        int spriteCounter = 0;
        for (int i = startPos; i < toConvertLength; i++)
        {
            int currentNumber = int.Parse(toConvert.ToString()[i].ToString());
            auxSprites[spriteCounter] = numberSprites[currentNumber];
            spriteCounter++;
        }

        return auxSprites;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(HUDManager))]
class HUDManagerEditor : Editor
{
    HUDManager manager { get { return target as HUDManager; } }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (Application.isPlaying)
        {
            EditorExtensionMethods.DrawSeparator(Color.gray);
            if (GUILayout.Button("Set Health"))
            {
                manager.UpdateHealth(manager.numberToSet);
            }
            if (GUILayout.Button("Set Armor"))
            {
                manager.UpdateArmor(manager.numberToSet);
            }
            if (GUILayout.Button("Set Coins"))
            {
                manager.UpdateCoins(manager.numberToSet);
            }
        }
    }
}
#endif