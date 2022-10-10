using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseScreen;
    public GameObject deathScreen;
    public Slider slider;
    public TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        slider.value = Option.sensibility / 10f;
        slider.onValueChanged.AddListener(delegate { OnChange(); }); ;
        text.text = string.Format("{0:0.#}", slider.value);
    }

    // Update is called once per frame
    private void Update()
    {
        if (InputManager.Instance.PlayerPause())
            if (!deathScreen.activeSelf)
                pauseScreen.SetActive(!pauseScreen.activeSelf);
    }
    private void OnChange()
    {
        Option.sensibility = slider.value * 10;
        text.text = string.Format("{0:0.#}", slider.value);
    }

    public void Unpause()
    {
        pauseScreen.SetActive(false);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
