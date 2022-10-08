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

    public Sprite mouthClosed;
    public Sprite mouthOpened;

    public string text;

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

    // Start is called before the first frame update
    void Start()
    {
        clipSampleData = new float[sampleDataLength];
        StartDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if (audioSource.isPlaying)
        {
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

            if (clipLoudness > 0.175f)
                imageHolder.sprite = mouthOpened;
            else
                imageHolder.sprite = mouthClosed;


            float fastLetterPerSecond = letterPerSecond * wordSpeed;
            float wordTime = 1 / letterPerSecond * (nextSpaceIndex - lastSpaceIndex);
            float fastWordTime = 1 / fastLetterPerSecond * (nextSpaceIndex - lastSpaceIndex);

            if (audioSource.time - lastWordTime <= fastWordTime)
            {
                textMeshProUGUI.text = text.Substring(0, Mathf.CeilToInt(lastWordTime * letterPerSecond + fastLetterPerSecond * (audioSource.time - lastWordTime)));
            }
            else if (audioSource.time > lastWordTime + wordTime)
            {
                lastSpaceIndex = nextSpaceIndex;
                nextSpaceIndex = text.IndexOf(' ', lastSpaceIndex + 1);
                lastWordTime = audioSource.time;
            }
       }
    }
    public void StartDialogue(AudioClip dialogue = null)
    {
        if (dialogue != null)
            audioSource.clip = dialogue;
        letterPerSecond = text.Length / audioSource.clip.length;
        lastSpaceIndex = 0;
        nextSpaceIndex = text.IndexOf(' ', lastSpaceIndex);
        lastWordTime = 0;
        audioSource.Play();
    }
}
