using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EnemyDialogueDatabase : MonoBehaviour
{

    public static EnemyDialogueDatabase Instance { get; set; }


    [SerializeField]
    private GameObject dialoguePrefab = null;
    [SerializeField]
    private List<DialogueTemplate> dialogueTemplates = new List<DialogueTemplate>();

    private bool isPlaying = false;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public DialogueTemplate FindDialogueByName(string name)
    {
        name = name.ToLower();
        return dialogueTemplates[0];
        return dialogueTemplates.Find(dialogue => dialogue.name.ToLower() == name);
    }

    public void TryPlayDialogue(string name)
    {
        if (isPlaying)
            return;

        PlayDialogue(FindDialogueByName(name));
    }

    public void PlayDialogue(DialogueTemplate dialogue)
    {
        if (dialogue == null)
            return;

        isPlaying = true;
        GameObject go = Instantiate(dialoguePrefab);
        var dialogueBoxManager = go.GetComponent<DialogueBoxManager>();
        dialogueBoxManager.StartDialogue(dialogue, 0f);
        dialogueBoxManager.DialogueFinished += () =>
        {
            isPlaying = false;
            Destroy(go);
        };
    }



}