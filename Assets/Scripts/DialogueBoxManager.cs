using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBoxManager : MonoBehaviour
{
    public static DialogueBoxManager Instance { get; set; }

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public Image imageHolder;
    public AudioSource audioSource;
    public TextMeshProUGUI textDialogue;
    public TextMeshProUGUI textName;

    public DialogueTemplate currentDialogue;
    public DialogueVoiceLine currentVoiceLine;
    public DialogueFace currentFace;

    public bool isPlaying = false;
    public bool playOnLaunch = false;

    private float clipLoudness;
    private float[] clipSampleData;
    public int sampleDataLength = 1024 / 4;
    public float updateStep = 0.01f / 4;
    private float currentUpdateTime = 0f;
    private float letterPerSecond;
    private int lastSpaceIndex;
    private int nextSpaceIndex;
    private float lastWordTime;
    public float wordSpeed = 1.25f;

    private Vector2 faceDefaultAnchorX;
    float deltaFace;
    private Vector2 textDialogueDefaultAnchorX;
    float deltaTextDialogue;

    // Start is called before the first frame update
    void Start()
    {
        clipSampleData = new float[sampleDataLength];

        faceDefaultAnchorX = new Vector2(imageHolder.rectTransform.anchorMin.x, imageHolder.rectTransform.anchorMax.x);
        deltaFace = faceDefaultAnchorX.y - faceDefaultAnchorX.x;

        textDialogueDefaultAnchorX = new Vector2(textDialogue.rectTransform.anchorMin.x, textDialogue.rectTransform.anchorMax.x);
        deltaTextDialogue = textDialogueDefaultAnchorX.y - textDialogueDefaultAnchorX.x;

        if (playOnLaunch)
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
                textDialogue.text = currentVoiceLine.text.Substring(0, Mathf.Clamp(Mathf.CeilToInt(lastWordTime * letterPerSecond + fastLetterPerSecond * (audioSource.time - lastWordTime)), 0, currentVoiceLine.text.Length - 1));
            }
            else if (audioSource.time > lastWordTime + wordTime)
            {
                lastSpaceIndex = nextSpaceIndex;
                nextSpaceIndex = currentVoiceLine.text.IndexOf(' ', lastSpaceIndex + 1);
                lastWordTime = audioSource.time;
            }
        }
        else if (isPlaying && currentVoiceLine != null)
        {
            textDialogue.text = currentVoiceLine.text;
            StartCoroutine(WaitThenPlay(currentDialogue.pauseLengthBetweenVoiceLines));
        }
    }
    public void StartDialogue(DialogueTemplate dialogueTemplate = null)
    {
        if (dialogueTemplate != null)
            currentDialogue = dialogueTemplate;
        if (currentDialogue == null)
            return;
        currentDialogue.voiceLines = new Queue<DialogueVoiceLine>(currentDialogue.voiceLinesList);
        imageHolder.enabled = false;
        textDialogue.text = "";
        textName.text = "";
        StartCoroutine(WaitThenPlay(1f));
    }

    public void PlayNextVoiceLine()
    {
        if (!currentDialogue.voiceLines.TryDequeue(out currentVoiceLine))
        {
            isPlaying = false;
            DialogueFinished?.Invoke();
            return;
        }

        imageHolder.enabled = true;

        currentFace = currentVoiceLine.face;

        switch (currentVoiceLine.faceLocation)
        {
            case Location.Left:
                imageHolder.rectTransform.anchorMin = new Vector2(faceDefaultAnchorX.x, imageHolder.rectTransform.anchorMin.y);
                imageHolder.rectTransform.anchorMax = new Vector2(faceDefaultAnchorX.y, imageHolder.rectTransform.anchorMax.y);

                textDialogue.rectTransform.anchorMin = new Vector2(textDialogueDefaultAnchorX.x, textDialogue.rectTransform.anchorMin.y);
                textDialogue.rectTransform.anchorMax = new Vector2(textDialogueDefaultAnchorX.y, textDialogue.rectTransform.anchorMax.y);

                textName.rectTransform.anchorMin = new Vector2(textDialogueDefaultAnchorX.x, textName.rectTransform.anchorMin.y);
                textName.rectTransform.anchorMax = new Vector2(textDialogueDefaultAnchorX.y, textName.rectTransform.anchorMax.y);

                break;
            case Location.Right:
                imageHolder.rectTransform.anchorMin = new Vector2(textDialogueDefaultAnchorX.y - deltaFace, imageHolder.rectTransform.anchorMin.y);
                imageHolder.rectTransform.anchorMax = new Vector2(textDialogueDefaultAnchorX.y, imageHolder.rectTransform.anchorMax.y);

                textDialogue.rectTransform.anchorMin = new Vector2(faceDefaultAnchorX.x, textDialogue.rectTransform.anchorMin.y);
                textDialogue.rectTransform.anchorMax = new Vector2(faceDefaultAnchorX.x + deltaTextDialogue, textDialogue.rectTransform.anchorMax.y);

                textName.rectTransform.anchorMin = new Vector2(faceDefaultAnchorX.x, textName.rectTransform.anchorMin.y);
                textName.rectTransform.anchorMax = new Vector2(faceDefaultAnchorX.x + deltaTextDialogue, textName.rectTransform.anchorMax.y);

                break;
        }

        textName.color = currentFace.nameColor;
        textName.text = currentFace.name;

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
