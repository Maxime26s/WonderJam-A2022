using System.Collections;
using System.Collections.Generic;
using UnityEngine;  

[CreateAssetMenu(menuName = "Dialogue/Template")]
public class DialogueTemplate : ScriptableObject
{
    public float pauseLengthBetweenVoiceLines;

    public List<DialogueVoiceLine> voiceLinesList;

    public Queue<DialogueVoiceLine> voiceLines;

    private void Awake() {
    }
}