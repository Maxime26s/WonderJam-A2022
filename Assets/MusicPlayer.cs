using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioClip musicClip = null;
    [SerializeField]
    private float musicVolume = 0.2f;

    void Start()
    {
        if (musicClip == null)
            return;

        MusicManager.Instance?.PlayMusic(musicClip, musicVolume);
    }
}
