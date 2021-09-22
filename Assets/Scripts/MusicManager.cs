using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MusicManager
{
    public enum Music
    {
        MainMenu,
        Level1,
    }
    public static void PlayMusic(Music music)
    {
        GameObject musicGameObject = new GameObject("Music");
        AudioSource audioSource = musicGameObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(GetMusicClip(music));
    }

    private static AudioClip GetMusicClip(Music music)
    {
        foreach (MusicAssets.MusicAudioClip musicAudioClip in MusicAssets.MusicInstance.musicAudioClipArray)
        {
            if (musicAudioClip.music == music)
            {
                return musicAudioClip.audioClip;
            }
        }
        return null;
    }

}
