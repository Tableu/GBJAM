using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicAssets : MonoBehaviour
{
    private static MusicAssets _musicInstance;

    public static MusicAssets MusicInstance
    {
        get
        {
            if (_musicInstance == null) _musicInstance = Instantiate(Resources.Load<MusicAssets>("MusicAssets"));
            return _musicInstance;
        }
    }

   // public MusicAudioClip[] musicAudioClipArray;

 //   [System.Serializable]
   // public class MusicAudioClip
  //  {
  //      public MusicManager.Music music;
 //       public AudioClip audioClip;
 //   }
}
