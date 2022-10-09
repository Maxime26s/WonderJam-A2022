using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//A singleton that handles level loading and unloading, along with its fade-to-black and fade-in transitions.
public class LevelLoader : MonoBehaviour {

    public static LevelLoader Instance { get; set; }
    public Animator transition;
    public float transitionTime = 1f;
    public Canvas animCanvas;

    public AudioSource LoaderAudioSource = null;

    public void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }



    public void LoadMenu() {
        Debug.Log("Loading MainMenu");
        StartCoroutine(LoadScene("MainMenu"));
    }

    public void QuitGame() {
        Debug.Log("Quitting");
        StartCoroutine(Quit());
    }

    public void LoadInstr() {
        Debug.Log("Loading Instructions");
        StartCoroutine(LoadScene("Instructions"));
    }

    public void LoadSelect() {
        Debug.Log("Loading player select");
        StartCoroutine(LoadScene("PlayerSelect"));
    }

    public void LoadScoreboard() {
        Debug.Log("Loading Scoreboard");
        StartCoroutine(LoadScene("Scoreboard"));
    }

    public void PlayButtonSound()
    {
        if (LoaderAudioSource != null && LoaderAudioSource.clip != null)
            LoaderAudioSource.PlayOneShot(LoaderAudioSource.clip);
    }

    public void Disable()
    {
        animCanvas.gameObject.SetActive(false);
    }

    public void LoadNextLevel(string sceneName = "") {
        Debug.Log("Loading next level \"" + sceneName + '\"');
        StartCoroutine(LoadNextLevelCo(sceneName));
    }

    public void ReloadCurrentLevel()
    {
        Debug.Log("Reloading level");
        StartCoroutine(ReloadCurrentLevelCo());
    }

    IEnumerator LoadNextLevelCo(string sceneName = "")
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        animCanvas.gameObject.SetActive(true);
        if (transition != null)
            transition.SetTrigger("Fade_out_tr");

        yield return new WaitForSeconds(transitionTime);

        AsyncOperation asyncOperation;
        if (sceneName == "")
            asyncOperation = SceneManager.LoadSceneAsync(currentScene + 1);
        else
            asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        asyncOperation.completed += (_) =>      
        {
            if (GameManager.Instance)
                GameManager.Instance.InitMap();
        };
    }

    IEnumerator LoadScene(string scene_name)
    {
        animCanvas.gameObject.SetActive(true);
        if (transition != null)
            transition.SetTrigger("Fade_out_tr");

        yield return new WaitForSeconds(transitionTime);
        
        SceneManager.LoadScene(scene_name);
    }

    IEnumerator Quit()
    {
        animCanvas.gameObject.SetActive(true);
        if (transition != null)
            transition.SetTrigger("Fade_out_tr");

        yield return new WaitForSeconds(transitionTime);

        Application.Quit();
    }

    public void Mute() {
        LoaderAudioSource.mute = !LoaderAudioSource.mute;
    }

    IEnumerator ReloadCurrentLevelCo()
    {
        animCanvas.gameObject.SetActive(true);
        if (transition != null)
            transition.SetTrigger("Fade_out_tr");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public string CurrentLevelName()
    {
        return SceneManager.GetActiveScene().name;
    }

}
