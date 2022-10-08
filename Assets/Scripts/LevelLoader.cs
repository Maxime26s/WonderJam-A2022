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

    public AudioSource buttonAudioSource = null;

    public void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void LoadMenu()
    {
        if (GameManager.Instance != null && GameManager.Instance.gameObject != null)
            Destroy(GameManager.Instance.gameObject);
        //if (PlayerManager.Instance != null && PlayerManager.Instance.gameObject != null)
        //    Destroy(PlayerManager.Instance.gameObject);

        GameManager.Instance = null;
        //PlayerManager.Instance = null;

        StartCoroutine(LoadScene("Menu"));
    }

    public void QuitGame() {
        Debug.Log("QUIT");
        StartCoroutine(Quit());
    }

    public void LoadInstr() {
        Debug.Log("INSTR");
        StartCoroutine(LoadScene("Instructions"));
    }

    public void LoadSelect() {
        Debug.Log("LOAD PLAYER SELECT");
        StartCoroutine(LoadScene("PlayerSelect"));
    }

    public void LoadScoreboard()
    {
        StartCoroutine(LoadScene("Scoreboard"));
    }

    public void PlayButtonSound()
    {
        if (buttonAudioSource != null && buttonAudioSource.clip != null)
            buttonAudioSource.PlayOneShot(buttonAudioSource.clip);
    }

    public void Disable()
    {
        animCanvas.gameObject.SetActive(false);
    }

    public void LoadNextLevel(string sceneName = "") {
        Debug.Log("LOAD NEXT LVL");
        StartCoroutine(LoadNextLevelCo(sceneName));
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

}
