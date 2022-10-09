using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndKill : MonoBehaviour
{
    [SerializeField]
    private string entity;
    [SerializeField]
    private bool ended = false;

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
        LevelLoader.Instance.LoadNextLevel();
    }
}
