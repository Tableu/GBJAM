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


    public void PlayMusic(Music toChangeTo, bool loop = true, float delay = 0)
    {
        musicSource.Stop();
        musicSource.clip = musicTracks[(int)toChangeTo];
        musicSource.loop = loop;
        if (delay == 0)
        {
            musicSource.Play();
        }
        else
        {
            StartCoroutine(PlayMusicAfterTime(delay));
        }
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