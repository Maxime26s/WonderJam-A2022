using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//A singleton that handles level loading and unloading, along with its fade-to-black and fade-in transitions.
public class MainMenuUIScript : MonoBehaviour {

    public float transitionTime = 1f;

    public void ButtonStart()
    {
        LevelLoader.Instance.LoadNextLevel("Introduction");
    }

    public void ButtonTutorial() {
        //LevelLoader.Instance.ReloadCurrentLevel();
        Debug.Log("No tutorial yet");
    }

    public void ButtonExit() {
        LevelLoader.Instance.QuitGame();
    }

    public void ToggleMute() {
        LevelLoader.Instance.Mute();
    }
}
