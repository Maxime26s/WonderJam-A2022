using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//A singleton that handles level loading and unloading, along with its fade-to-black and fade-in transitions.
public class MainMenuUIScript : MonoBehaviour {

    public float transitionTime = 1f;
    public bool actionPending = false;

    public void ButtonStart() {
        if (TryMakeAction()) {
            LevelLoader.Instance.LoadNextLevel("Introduction");
        }
    }

    public void ButtonTutorial() {
        if (TryMakeAction()) {
            Debug.Log("No tutorial yet, sending player to main game");
            LevelLoader.Instance.LoadNextLevel("Introduction");
        }
    }

    public void ButtonExit() {
        if (TryMakeAction()) {
            LevelLoader.Instance.QuitGame();
            Debug.Log("You're probably softlocked since this button doesn't work in editor but stops you from clicking other buttons.");
        }
    }

    public void ToggleMute() {
        if (!actionPending) {
            LevelLoader.Instance.Mute();
        }
    }

    private bool TryMakeAction() {
        if (!actionPending)
            actionPending = true;
        else
            return false;
        return true;
    }
}
