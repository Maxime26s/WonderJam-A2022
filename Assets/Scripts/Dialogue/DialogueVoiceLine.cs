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
    public Location faceLocation = Location.Left;
}

public enum Location
{
    Left,
    Right,
}
