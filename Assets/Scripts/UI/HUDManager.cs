using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    static HUDManager _instance;
    public static HUDManager Instance
    {
        get 
        {
            if (_instance == null && !FindObjectOfType<HUDManager>())
            {
                Debug.LogError("Uhoh, something very wrong is going on lmao\r\nSomeone tried to call HUDManager but none was present");
                //Maybe instantiate one if there is none? Shouldn't happen but we'll see.
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

        healthCounter = transform.Find("Health").GetComponent<HUDCounter>();
        armorCounter = transform.Find("Armor").GetComponent<HUDCounter>();
        coinCounter = transform.Find("Coins").GetComponent<HUDCounter>();
    }

    public void UpdateHealth(int newHealth)
    {
        healthCounter.SetValue(IntToSprite(newHealth));
    }
    public void UpdateArmor(int newArmor)
    {
        armorCounter.SetValue(IntToSprite(newArmor));
    }
    public void UpdateCoins(int newCoins)
    {
        coinCounter.SetValue(IntToSprite(newCoins));
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