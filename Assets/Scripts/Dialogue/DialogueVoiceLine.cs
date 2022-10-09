using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Voice Line")]
public class DialogueVoiceLine : ScriptableObject
{
    public string text;
    public DialogueFace face;
    public AudioClip voiceLine;
    public float closedMouthThreshold = 0.175f;
    public bool showFullText = false;
    public Location faceLocation = Location.Left;
    public float customDuration = -1;
    [Range(0.0f, 1.0f)]
    public float volume = 1;
    public bool useRandomFlicking = false;
}

public enum Location
{
    Left,
    Right,
}
