using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameDialogue : MonoBehaviour
{
    // NEED THESE VARIABLES ********************
    public DialogueTemplate dialogue;
    public GameObject dialoguePrefab;
    // *****************************************

    void Start()
    {
        IEnumerator WaitThenDialogue()
        {
            yield return new WaitForSeconds(2f);

            // HOW TO USE IN GAME DIALOGUE *****
            GameObject go = Instantiate(dialoguePrefab);
            var dialogueBoxManager = go.GetComponent<DialogueBoxManager>();
            dialogueBoxManager.StartDialogue(dialogue, 0f);
            dialogueBoxManager.DialogueFinished += () => Destroy(go);
            // THIS IS ALL YOU NEED ************

            Destroy(gameObject);
        }

        StartCoroutine(WaitThenDialogue());
    }
}
