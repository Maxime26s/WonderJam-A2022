using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndKill : MonoBehaviour
{
    [SerializeField]
    private string entity;
    [SerializeField]
    private bool ended = false;

    public Fade fade;

    public TextMeshProUGUI titleObject;
    public TextMeshProUGUI loreObject;
    public string title;
    public string lore;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FinishGame()
    {
        if (loreObject.color.a != 0)
            return;

        titleObject.text = title;
        loreObject.text = lore;

        fade.gameObject.SetActive(true);
        fade.StartFading();
        //LevelLoader.Instance.LoadNextLevel();
    }
}
