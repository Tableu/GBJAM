using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class pSoundManager
{
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
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(GetAudioClip(sound));
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
