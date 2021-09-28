using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Swaps palettes using shader magic
/// </summary>
[RequireComponent(typeof(Camera))]
public class PaletteSwap : MonoBehaviour
{
    enum BackgroundColor { Color1, Color2, Color3, Color4 }

    [SerializeField, Tooltip("The starting background color (1-4, darkest to brightest)")]
    BackgroundColor startingBGColor = BackgroundColor.Color2;

    [SerializeField, Tooltip("The source colors to be changed")]
    ColorPalette inputPalette;

    [SerializeField, Tooltip("The palette pool we'll pick our palettes from")]
    List<ColorPalette> palettes = new List<ColorPalette>();
    [SerializeField, Range(0, 3), Tooltip("The current palette index, used for debugging purposes")]
    int paletteIndex = 0;

    private Material paletteSwapMaterial;

    //[SerializeField, Range(0, 20f)]
    //float colorTolerance = 0.004f;

    [HideInInspector]
    public bool reset = false;

    /// <summary>
    /// Static reference for singleton implementation
    /// </summary>
    static PaletteSwap _instance;
    public static PaletteSwap Instance
    {
        get
        {
            if (!FindObjectOfType<PaletteSwap>())
            {
                GameObject cameraObject = FindObjectOfType<Camera>().gameObject;
                if (cameraObject)
                {
                    _instance = cameraObject.AddComponent<PaletteSwap>();
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// Initializes stuff
    /// Remove SetPalettes() or the palette list will be overridden.
    /// </summary>
    void Init()
    {
        Shader matShader = Shader.Find("Thing/PaletteSwap");
        if (matShader == null)
        {
            Debug.LogError("Shader was null, trying to find it via Resources");
            matShader = Resources.Load<Shader>("Shaders/PaletteSwap");
            if (matShader == null)
            {
                Debug.LogError("Dang, that didn't fix it unu");
            }
            else
            {
                Debug.Log("Yessss we da bessss");
            }
        }
        paletteSwapMaterial = new Material(matShader);
        SetPalettes();
    }

    /// <summary>
    /// Delete object if another PaletteSwap exists, to enforce singleton thingies
    /// Also initializes, changes background color to match palette and sets the colors
    /// </summary>
    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        _instance = this;
        //DontDestroyOnLoad(this);

        Init();
        //MakeBackgroundColor2();
        SetStartingBGColor();
        SetColors();
        //SetColorTolerance();
    }

    void SetStartingBGColor()
    {
        switch (startingBGColor)
        {
            case BackgroundColor.Color1:
                MakeBackgroundColor1();
                break;
            case BackgroundColor.Color2:
                MakeBackgroundColor2();
                break;
            case BackgroundColor.Color3:
                MakeBackgroundColor3();
                break;
            case BackgroundColor.Color4:
                MakeBackgroundColor4();
                break;
        }
    }

    /// <summary>
    /// Overrides color palette, just for debugging purposes
    /// </summary>
    void SetPalettes()
    {
        palettes.Clear();
        palettes.Add(new ColorPalette() { color1 = new Color32(0x23, 0x49, 0x5d, 0xff), color2 = new Color32(0x39, 0x70, 0x7a, 0xff), color3 = new Color32(0x95, 0xe0, 0xcc, 0xff), color4 = new Color32(0xda, 0xf2, 0xe9, 0xff) });
        palettes.Add(new ColorPalette() { color1 = new Color32(0x62, 0x2e, 0x4c, 0xff), color2 = new Color32(0x75, 0x50, 0xe8, 0xff), color3 = new Color32(0x60, 0x8f, 0xcf, 0xff), color4 = new Color32(0x8b, 0xe5, 0xff, 0xff) });
        palettes.Add(new ColorPalette() { color1 = new Color32(0x77, 0x43, 0x46, 0xff), color2 = new Color32(0xb8, 0x76, 0x52, 0xff), color3 = new Color32(0xac, 0xb9, 0x65, 0xff), color4 = new Color32(0xf5, 0xf2, 0x9e, 0xff) });
        //palettes.Add(new ColorPalette() { color1 = new Color32(0x0f, 0x38, 0x0f, 0xff), color2 = new Color32(0x30, 0x62, 0x30, 0xff), color3 = new Color32(0x8b, 0xac, 0x0f, 0xff), color4 = new Color32(0x9b, 0xbc, 0x0f, 0xff) });
        palettes.Add(new ColorPalette() { color1 = new Color(0, 0, 0, 1), color2 = new Color(0.33f, 0.33f, 0.33f, 1), color3 = new Color(0.66f, 0.66f, 0.66f, 1), color4 = new Color(1, 1, 1, 1) });
        //palettes.Add(new ColorPalette() { color1 = new Color32(0x74, 0x56, 0x9b, 0xff), color2 = new Color32(0x96, 0xfb, 0xc7, 0xff), color3 = new Color32(0xf7, 0xff, 0xae, 0xff), color4 = new Color32(0xff, 0xb3, 0xcb, 0xff) });

        //622e4c, 7550e8, 608fcf, 8be5ff
        //0f380f, 306230, 8bac0f, 9bbc0f

        //74569b, 96fbc7, f7ffae, ffb3cb

        //23495d, 39707a, 95e0cc, daf2e9
    }

    /// <summary>
    /// Makes background color the first color in input palette.
    /// This can also be removed if we want the camera's background color something else other than the first color on the palette.
    /// </summary>
    [ContextMenu("Make Background Color1")]
    public void MakeBackgroundColor1()
    {
        GetComponent<Camera>().backgroundColor = inputPalette.color1;
    }
    [ContextMenu("Make Background Color2")]
    public void MakeBackgroundColor2()
    {
        GetComponent<Camera>().backgroundColor = inputPalette.color2;
    }
    [ContextMenu("Make Background Color3")]
    public void MakeBackgroundColor3()
    {
        GetComponent<Camera>().backgroundColor = inputPalette.color3;
    }
    [ContextMenu("Make Background Color4")]
    public void MakeBackgroundColor4()
    {
        GetComponent<Camera>().backgroundColor = inputPalette.color4;
    }

    /// <summary>
    /// Sets the color variables to the shader.
    /// Should change in the future to avoid using paletteIndex maybe.
    /// </summary>
    [ContextMenu("Update Colors")]
    public void SetColors()
    {
        if (paletteSwapMaterial == null)
        {
            Init();
        }

        paletteSwapMaterial.SetColor("_InColorA", inputPalette.color1);
        paletteSwapMaterial.SetColor("_InColorB", inputPalette.color2);
        paletteSwapMaterial.SetColor("_InColorC", inputPalette.color3);
        paletteSwapMaterial.SetColor("_InColorD", inputPalette.color4);

        paletteSwapMaterial.SetColor("_OutColorA", palettes[paletteIndex].color1);
        paletteSwapMaterial.SetColor("_OutColorB", palettes[paletteIndex].color2);
        paletteSwapMaterial.SetColor("_OutColorC", palettes[paletteIndex].color3);
        paletteSwapMaterial.SetColor("_OutColorD", palettes[paletteIndex].color4);
    }

    //public void SetColorTolerance()
    //{
    //    if (paletteSwapMaterial == null)
    //    {
    //        Init();
    //    }

    //    paletteSwapMaterial.SetFloat("_offset", colorTolerance);
    //}

    /// <summary>
    /// Does rendering magic using the material with the shader over everything else
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, paletteSwapMaterial);
    }

}

/// <summary>
/// Just a struct to store 4 colors in.
/// Could be expanded upon and maybe even made into a class if needed.
/// </summary>
[System.Serializable]
public struct ColorPalette
{
    public Color color1;
    public Color color2;
    public Color color3;
    public Color color4;
}


#if UNITY_EDITOR
/// <summary>
/// Custom editor to add a button to update the current palette color.
/// </summary>
[CustomEditor(typeof(PaletteSwap))]
class PaletteSwapEditor : Editor
{
    PaletteSwap palSwap { get { return target as PaletteSwap; } }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (Application.isPlaying)
        {
            EditorExtensionMethods.DrawSeparator(Color.gray);
            if (GUILayout.Button("Swap palette"))
            {
                palSwap.SetColors();
                //palSwap.SetColorTolerance();
            }
            EditorExtensionMethods.DrawSeparator(Color.gray);
            if (GUILayout.Button("Background Color1"))
            {
                palSwap.MakeBackgroundColor1();
            }
            if (GUILayout.Button("Background Color2"))
            {
                palSwap.MakeBackgroundColor2();
            }
            if (GUILayout.Button("Background Color3"))
            {
                palSwap.MakeBackgroundColor3();
            }
            if (GUILayout.Button("Background Color4"))
            {
                palSwap.MakeBackgroundColor4();
            }
        }
    }
}
#endif