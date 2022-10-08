using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public float fadeSpeed = 1f;
    public Image image;
    public bool fading = false;
    public int nextSceneIndex;

    // Start is called before the first frame update
    void Start()
    {
        DialogueBoxManager.Instance.DialogueFinished += StartFading;
        FadeFinished += LoadNextScene;
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneIndex);
    }

    // Update is called once per frame
    void Update()
    {
        if (fading)
        {
            image.color = new Color(0, 0, 0, Mathf.Clamp(image.color.a + Time.deltaTime * fadeSpeed, 0, 1));
            if (image.color.a >= 1)
            {
                fading = false;
                FadeFinished?.Invoke();
            }
        }
    }

    void StartFading()
    {
        fading = true;
    }

    public delegate void FadeFinishedHandler();
    public event FadeFinishedHandler FadeFinished;
}
