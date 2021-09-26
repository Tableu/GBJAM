using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class LoadingScreenText : ScriptableObject
{
    [SerializeField, TextArea]
    string _introText = "";
    [SerializeField, TextArea]
    string _level1Text = "";
    [SerializeField, TextArea]
    string _level2Text = "";
    [SerializeField, TextArea]
    string _level3Text = "";
    [SerializeField, TextArea]
    string _endGameText = "";

    public string IntroText
    {
        get { return _introText; }
    }
    public string Level1Text
    {
        get { return _level1Text; }
    }
    public string Level2Text
    {
        get { return _level2Text; }
    }
    public string Level3Text
    {
        get { return _level3Text; }
    }
    public string EndGameText
    {
        get { return _endGameText; }
    }
}
