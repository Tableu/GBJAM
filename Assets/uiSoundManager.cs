using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uiSoundManager : MonoBehaviour
{
    public AudioSource uiSrc;
    public AudioClip sfxNav;
    public AudioClip sfxSelect;
    // Start is called before the first frame update
    void Start()
    {
        uiSrc = GetComponent<AudioSource>();
    }

    public void Nav()
    {
        uiSrc.PlayOneShot(sfxNav);
    }

    public void Select()
    {
        uiSrc.PlayOneShot(sfxSelect);
    }
}
