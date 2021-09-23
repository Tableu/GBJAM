using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum Music
{
    MainMenu,
    Level1,
}
public class MusicManager : MonoBehaviour
{
    public AudioSource musicSource;
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
        musicSource.Stop();
        musicSource.clip = musicTracks[(int)toChangeTo];
        StartCoroutine(PlayMusicAfterTime());
    }

    IEnumerator PlayMusicAfterTime()
    {
        yield return new WaitForSeconds(1f);
        musicSource = GetComponent<AudioSource>();
        musicSource.Play();
        musicSource.loop = true;
    }

}
