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
    private DialogueTemplate gameOverDialogue;

    private bool isPlaying = false;
    public GameObject go;

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
        return dialogueEnemyTemplates.Find(dialogue => name.Contains(dialogue.name.ToLower())) ?? defaultEnemyDialogues[UnityEngine.Random.Range(0, defaultEnemyDialogues.Count)];
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

    public bool PlayDialogue(DialogueTemplate dialogue, bool shouldForce = false)
    {
        if (dialogue == null)
            return false;

        isPlaying = true;
        go = Instantiate(dialoguePrefab);
        var dialogueBoxManager = go.GetComponent<DialogueBoxManager>();
        if (shouldForce)
            dialogueBoxManager.ForcePlay(dialogue);
        else
            dialogueBoxManager.StartDialogue(dialogue, 0f);
        dialogueBoxManager.DialogueFinished += DestroyAfterFinish;
        return true;
    }

    public bool TryPlayGameOverDialogue()
    {
        //if(isPlaying)
        //DialogueBoxManager.Instance.GetComponent<DialogueBoxManager>().ForceDestroy();

        return PlayDialogue(gameOverDialogue, true);
    }

    private void DestroyAfterFinish()
    {
        isPlaying = false;
        Destroy(go);
    }

    public void TryStopSound()
    {
        if(go != null)
        {
            go.GetComponent<DialogueBoxManager>().DialogueFinished -= DestroyAfterFinish;
            isPlaying = false;
        }
        DialogueBoxManager.Instance?.Reset();
    }
}