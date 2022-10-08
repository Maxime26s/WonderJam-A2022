using System.Collections;
using System.Collections.Generic;
using UnityEngine;  

[CreateAssetMenu(menuName = "Dialogue/Template")]
public class DialogueTemplate : ScriptableObject
{
    public float pauseLengthBetweenVoiceLines = 1.25f;

    public List<DialogueVoiceLine> voiceLinesList = new List<DialogueVoiceLine>();

    public Queue<DialogueVoiceLine> voiceLines = new Queue<DialogueVoiceLine>();
}