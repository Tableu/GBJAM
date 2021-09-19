using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class pSoundManager
{
    static List<AudioSource> sources = new List<AudioSource>();
    public enum Sound
    { 
        pJump,
        pAttack,
        pHit,
        pDie,
        pCoin,
        pPickup,
    
    }

    public static void PlaySound(Sound sound)
    {
        //GameObject soundGameObject = new GameObject("Sound");
        //AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        //audioSource.PlayOneShot(GetAudioClip(sound));
        GetAvailableAudioSource().PlayOneShot(GetAudioClip(sound));
    }

    private static AudioSource GetAvailableAudioSource()
    {
        foreach (AudioSource auSource in sources)
        {
            if (!auSource.isPlaying)
            {
                return auSource;
            }
        }
        return AddNewAudioSource();
    }

    private static AudioSource AddNewAudioSource()
    {
        GameObject soundContainer = GameObject.Find("AudioSourceContainer");
        if (!soundContainer)
        {
            soundContainer = new GameObject("AudioSourceContainer");
        }
        GameObject soundGameObject = new GameObject($"Sound-{sources.Count}");
        soundGameObject.transform.SetParent(soundContainer.transform);
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        sources.Add(audioSource);
        return audioSource;
    }

    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach (AudioAssets.SoundAudioClip soundAudioClip in AudioAssets.i.soundAudioClipArray)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip;
            }
        }
        return null;
    }
}
