using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DialogueDatabase : MonoBehaviour
{

    public static DialogueDatabase Instance { get; set; }


    [SerializeField]
    private GameObject dialoguePrefab = null;
    [SerializeField]
    private List<DialogueTemplate> dialogueEnemyTemplates = new List<DialogueTemplate>();
    [SerializeField]
    private List<DialogueTemplate> defaultEnemyDialogues = new List<DialogueTemplate>();
    [SerializeField]
    private List<DialogueTemplate> loseHpDialogues = new List<DialogueTemplate>();
    [SerializeField]
    private DialogueTemplate poutineDialogue = new DialogueTemplate();

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

    public DialogueTemplate FindEnemyDialogueByName(string name)
    {
        name = name.ToLower();
        return dialogueEnemyTemplates.Find(dialogue => dialogue.name.ToLower() == name) ?? defaultEnemyDialogues[UnityEngine.Random.Range(0, defaultEnemyDialogues.Count)];
    }

    public bool TryPlayEnemyDialogue(string name)
    {
        if (isPlaying)
            return false;

        return PlayDialogue(FindEnemyDialogueByName(name));
    }

    public bool TryPlayLoseHpDialogue()
    {
        if (isPlaying)
            return false;

        return PlayDialogue(loseHpDialogues[UnityEngine.Random.Range(0, loseHpDialogues.Count)]);
    }

    public bool TryPlayPoutineDialogue()
    {
        if (isPlaying)
            return false;

        return PlayDialogue(poutineDialogue);
    }

    public bool PlayDialogue(DialogueTemplate dialogue)
    {
        if (dialogue == null)
            return false;

        isPlaying = true;
        GameObject go = Instantiate(dialoguePrefab);
        var dialogueBoxManager = go.GetComponent<DialogueBoxManager>();
        dialogueBoxManager.StartDialogue(dialogue, 0f);
        dialogueBoxManager.DialogueFinished += () =>
        {
            isPlaying = false;
            Destroy(go);
        };
        return true;
    }
}