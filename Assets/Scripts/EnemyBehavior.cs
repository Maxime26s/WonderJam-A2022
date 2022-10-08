using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    static int numberOfGlitchType = 2;

    [SerializeField]
    int glitchType = 0;

    [SerializeField]
    float glitchTimer = 20.0f;

    private void Start()
    {

    }

    private void Glitch()
    {
        int choix = Random.Range(0, numberOfGlitchType);

        switch (choix)
        {
            case 0:         //move to random navmesh location
                move();
                break;
            case 1:         //vibrate for x seconds
                vibrate();
                break;
        }
    }

    private void move()
    {
        
    }
    private void vibrate()
    {

    }
    private void stretch()
    {

    }
}
