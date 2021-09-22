using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Music
{
    MainMenu,
    Level1,
}
public class MusicManager : MonoBehaviour
{
    public AudioClip[] musicTracks;
    private static MusicManager _musicInstance;
    public static MusicManager MusicInstance
    {
        get
        {
            return _musicInstance;
        }
    }

    private void Awake()
    {
        if (_musicInstance != null && _musicInstance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _musicInstance = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    public void PlayMusic(Music toChangeTo)
    {
        GameObject musicGameObject = new GameObject("Music");
        AudioSource audioSource = musicGameObject.AddComponent<AudioSource>();
        audioSource.Stop();
        audioSource.clip = musicTracks[(int)toChangeTo];
        audioSource.loop = true;
//        audioSource.PlayOneShot(GetMusicClip(toChangeTo));
    }

//    private static AudioClip GetMusicClip(Music music)
//    {
//        foreach (MusicAssets.MusicAudioClip musicAudioClip in MusicAssets.MusicInstance.musicAudioClipArray)
//        {
//            if (musicAudioClip.music == music)
//            {
//                return musicAudioClip.audioClip;
//            }
//        }
//        return null;
//    }

}
