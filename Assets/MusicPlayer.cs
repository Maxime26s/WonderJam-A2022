using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioClip musicClip = null;

    void Start()
    {
        MusicManager.Instance.PlayMusic(musicClip);
    }
}
