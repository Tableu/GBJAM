using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
#if UNITY_EDITOR
using UnityEditor;
#endif
public enum Music
{
    MainMenu,
    EndJingle,
    TransitionJingle,
    Level1,
    Level2,
    Level3,
    Credits
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
            if (_musicInstance == null && !FindObjectOfType<MusicManager>())
            {
                _musicInstance = Instantiate(Resources.Load<MusicManager>("MusicManager"));
            }
            return _musicInstance;
        }
    }

    public float Volume
    {
        get { return musicSource.volume; }
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

        musicSource = GetComponent<AudioSource>();
    }

    public void SetAudioSourceSpeed(float newSpeed)
    {
        if (musicSource.pitch != newSpeed)
        {
            musicSource.pitch = newSpeed;
        }
    }
    public void SetAudioVolume(float volume)
    {
        if (musicSource.volume != volume)
        {
            musicSource.volume = volume;
        }
    }

    public void PlayMusic(Music toChangeTo, bool loop = true, float delay = 0)
    {
        musicSource.Stop();
        musicSource.clip = musicTracks[(int)toChangeTo];
        musicSource.loop = loop;
        musicSource.pitch = 1f;
        if (delay == 0)
        {
            musicSource.Play();
        }
        else
        {
            StartCoroutine(PlayMusicAfterTime(delay));
        }
    }
    public void StopMusic(float time)
    {
        StartCoroutine(FadeOut(time));
    }

    IEnumerator FadeOut(float time)
    {
        if (time == 0)
            time = 0.02f;

        float timeLapsed = 0;
        float currentProgress = 0;
        float startingVolume = musicSource.volume;
        while (currentProgress < 1)
        {
            currentProgress = Mathf.Clamp(timeLapsed / time, 0, 1f);
            musicSource.volume = startingVolume * (1f - currentProgress);

            yield return null;
            timeLapsed += Time.unscaledDeltaTime;
        }
        musicSource.Stop();
    }

    IEnumerator PlayMusicAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        musicSource.Play();
    }


}



#if UNITY_EDITOR
[CustomEditor(typeof(MusicManager))]
class MusicManagerEditor : Editor
{
    MusicManager man { get { return target as MusicManager; } }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (Application.isPlaying)
        {
            EditorExtensionMethods.DrawSeparator(Color.gray);
            if (GUILayout.Button("Play Main menu"))
            {
                man.PlayMusic(Music.MainMenu);
            }
            if (GUILayout.Button("Play End of level"))
            {
                man.PlayMusic(Music.EndJingle);
            }
            if (GUILayout.Button("Play Cutscene"))
            {
                man.PlayMusic(Music.TransitionJingle);
            }
            if (GUILayout.Button("Play song 1"))
            {
                man.PlayMusic(Music.Level1);
            }
        }
    }
}
#endif