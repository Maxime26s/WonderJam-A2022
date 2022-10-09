using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; set; }

    [SerializeField]
    private AudioSource musicAudioSource = null;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this);
    }

    public void PlayMusic(AudioClip audioClip)
    {
        StartCoroutine(PlayMusicCoroutine(audioClip));
    }

    public IEnumerator PlayMusicCoroutine(AudioClip audioClip)
    {
        yield return AudioFadeOut.FadeOut(musicAudioSource, 1f);
        musicAudioSource.clip = audioClip;
        musicAudioSource.Play();
        yield return null;
    }


    public static class AudioFadeOut
    {
        public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
        {
            float startVolume = audioSource.volume;

            while (audioSource.volume > 0)
            {
                audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

                yield return null;
            }

            audioSource.Stop();
            audioSource.volume = startVolume;
        }

    }
}
