using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//A singleton that handles level loading and unloading, along with its fade-to-black and fade-in transitions.
public class DeathUIScript : MonoBehaviour {

    public float transitionTime = 1f;

    public void LoadMenu()
    {
        LevelLoader.Instance.LoadMenu();
    }

    public void ReloadCurrentLevel()
    {
        LevelLoader.Instance.ReloadCurrentLevel();
    }
}
