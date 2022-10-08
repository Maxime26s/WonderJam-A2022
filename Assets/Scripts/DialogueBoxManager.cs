using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBoxManager : MonoBehaviour
{
    public Image imageHolder;
    public AudioSource audioSource;
    public TextMeshProUGUI textMeshProUGUI;

    public DialogueTemplate currentDialogue;
    public DialogueVoiceLine currentVoiceLine;
    public DialogueFace currentFace;

    public bool isPlaying = false;

    private float clipLoudness;
    private float[] clipSampleData;
    public int sampleDataLength = 1024;
    public float updateStep = 0.05f;
    private float currentUpdateTime = 0f;
    private float letterPerSecond;
    private int lastSpaceIndex;
    private int nextSpaceIndex;
    private float lastWordTime;
    public float wordSpeed = 1.25f;

    private Vector2 faceDefaultAnchorX;
    float deltaFace;
    private Vector2 textDefaultAnchorX;
    float deltaText;

    // Start is called before the first frame update
    void Start()
    {
        clipSampleData = new float[sampleDataLength];

        faceDefaultAnchorX = new Vector2(imageHolder.rectTransform.anchorMin.x, imageHolder.rectTransform.anchorMax.x);
        Debug.Log(faceDefaultAnchorX);
        deltaFace = faceDefaultAnchorX.y - faceDefaultAnchorX.x;
        Debug.Log(deltaFace);
        textDefaultAnchorX = new Vector2(textMeshProUGUI.rectTransform.anchorMin.x, textMeshProUGUI.rectTransform.anchorMax.x);
        Debug.Log(textDefaultAnchorX);
        deltaText = textDefaultAnchorX.y - textDefaultAnchorX.x;
        Debug.Log(deltaText);

        StartDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if (audioSource.isPlaying)
        {
            isPlaying = true;
            currentUpdateTime += Time.deltaTime;
            if (currentUpdateTime >= updateStep)
            {
                currentUpdateTime = 0f;
                audioSource.clip.GetData(clipSampleData, audioSource.timeSamples); //I read 1024 samples, which is about 80 ms on a 44khz stereo clip, beginning at the current sample position of the clip.
                clipLoudness = 0f;
                foreach (var sample in clipSampleData)
                {
                    clipLoudness += Mathf.Abs(sample);
                }
                clipLoudness /= sampleDataLength; //clipLoudness is what you are looking for
            }

            if (clipLoudness > currentVoiceLine.closedMouthThreshold)
                imageHolder.sprite = currentFace.mouthOpened;
            else
                imageHolder.sprite = currentFace.mouthClosed;


            float fastLetterPerSecond = letterPerSecond * wordSpeed;
            float wordTime = 1 / letterPerSecond * (nextSpaceIndex - lastSpaceIndex);
            float fastWordTime = 1 / fastLetterPerSecond * (nextSpaceIndex - lastSpaceIndex);

            if (audioSource.time - lastWordTime <= fastWordTime)
            {
                textMeshProUGUI.text = currentVoiceLine.text.Substring(0, Mathf.Clamp(Mathf.CeilToInt(lastWordTime * letterPerSecond + fastLetterPerSecond * (audioSource.time - lastWordTime)), 0, currentVoiceLine.text.Length-1));
            }
            else if (audioSource.time > lastWordTime + wordTime)
            {
                lastSpaceIndex = nextSpaceIndex;
                nextSpaceIndex = currentVoiceLine.text.IndexOf(' ', lastSpaceIndex + 1);
                lastWordTime = audioSource.time;
            }
        }
        else if (isPlaying)
        {
            StartCoroutine(WaitThenPlay(currentDialogue.pauseLengthBetweenVoiceLines));
        }
    }
    public void StartDialogue(DialogueTemplate dialogueTemplate = null)
    {
        if (dialogueTemplate != null)
            currentDialogue = dialogueTemplate;
        currentDialogue.voiceLines = new Queue<DialogueVoiceLine>(currentDialogue.voiceLinesList);
        Debug.Log(currentDialogue.voiceLines);
        PlayNextVoiceLine();
    }

    public void PlayNextVoiceLine()
    {
        if (!currentDialogue.voiceLines.TryDequeue(out currentVoiceLine))
        {
            isPlaying = false;
            DialogueFinished?.Invoke();
            return;
        }

        currentFace = currentVoiceLine.face;

        switch (currentVoiceLine.faceLocation)
        {
            case Location.Left:
                imageHolder.rectTransform.anchorMin = new Vector2(faceDefaultAnchorX.x, imageHolder.rectTransform.anchorMin.y);
                imageHolder.rectTransform.anchorMax = new Vector2(faceDefaultAnchorX.y, imageHolder.rectTransform.anchorMax.y);

                textMeshProUGUI.rectTransform.anchorMin = new Vector2(textDefaultAnchorX.x, textMeshProUGUI.rectTransform.anchorMin.y);
                textMeshProUGUI.rectTransform.anchorMax = new Vector2(textDefaultAnchorX.y, textMeshProUGUI.rectTransform.anchorMax.y);
                
                break;
            case Location.Right:
                imageHolder.rectTransform.anchorMin = new Vector2(textDefaultAnchorX.y - deltaFace, imageHolder.rectTransform.anchorMin.y);
                imageHolder.rectTransform.anchorMax = new Vector2(textDefaultAnchorX.y, imageHolder.rectTransform.anchorMax.y);

                textMeshProUGUI.rectTransform.anchorMin = new Vector2(faceDefaultAnchorX.x, textMeshProUGUI.rectTransform.anchorMin.y);
                textMeshProUGUI.rectTransform.anchorMax = new Vector2(faceDefaultAnchorX.x + deltaText, textMeshProUGUI.rectTransform.anchorMax.y);

                break;
        }


        audioSource.clip = currentVoiceLine.voiceLine;
        letterPerSecond = currentVoiceLine.text.Length / audioSource.clip.length;
        lastSpaceIndex = 0;
        nextSpaceIndex = currentVoiceLine.text.IndexOf(' ', lastSpaceIndex);
        if (nextSpaceIndex == -1)
            nextSpaceIndex = currentVoiceLine.text.Length;
        lastWordTime = 0;
        audioSource.Play();
    }

    IEnumerator WaitThenPlay(float waitTime)
    {
        isPlaying = false;
        yield return new WaitForSeconds(waitTime);
        PlayNextVoiceLine();
    }

    public delegate void DialogueFinishedHandler();
    public event DialogueFinishedHandler DialogueFinished;
}
