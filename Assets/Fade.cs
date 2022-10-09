using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public float fadeSpeed = 1f;
    public Image image;
    private bool fading = false;
    public string nextSceneName;
    public float loadNextSceneDelay = 0f;
    public List<TextMeshProUGUI> textToAppear = new List<TextMeshProUGUI>();

    // Start is called before the first frame update
    void Start()
    {
        FadeFinished += LoadNextScene;

        DialogueBoxManager.Instance.DialogueFinished += StartFading;
    }

    private void LoadNextScene()
    {
        IEnumerator Delay()
        {
            yield return new WaitForSeconds(loadNextSceneDelay);
            SceneManager.LoadScene(nextSceneName);
        }

        StartCoroutine(Delay());
    }

    private void Skip()
    {
        StartFading();
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance.PlayerJumpedThisFrame())
        {
            StartFading();
        }

        if (fading)
        {
            image.color = new Color(0, 0, 0, Mathf.Clamp(image.color.a + Time.deltaTime * fadeSpeed, 0, 1));
            foreach(var text in textToAppear)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Clamp(text.color.a + Time.deltaTime * fadeSpeed, 0, 1));
            }
            if (image.color.a >= 1)
            {
                fading = false;
                FadeFinished?.Invoke();
            }
        }
    }

    public void StartFading()
    {
        fading = true;
    }

    public delegate void FadeFinishedHandler();
    public event FadeFinishedHandler FadeFinished;
}
