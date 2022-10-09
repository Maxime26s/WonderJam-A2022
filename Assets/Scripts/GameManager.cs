using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

public static GameManager Instance { get; set; }

    public enum GameState
    {
        None,
        Starting,
        Playing,
        Win,
        OutOfTime,
        PlayerDeath,
    }

    [SerializeField]
    private GameObject spawnPoint;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private int startingLives = 3;
    [SerializeField]
    private EnemyManager enemyManager = null;
    [SerializeField]
    private GameObject livesUI = null;
    [SerializeField]
    private TMPro.TextMeshProUGUI timerTMP = null;
    [SerializeField]
    private float gameTimer = 300f;

    [SerializeField]
    private GameObject deathUI;
    [SerializeField]
    private Sprite deathScreen;
    [SerializeField]
    private Sprite outOfTimeScreen;


    [SerializeField]
    public bool wrenchEnabled = false;
    [SerializeField]
    public bool shotgunEnabled = false;
    [SerializeField]
    public bool bombEnabled = false;

    private GameState currentGameState = GameState.None;

    private bool hasBeenInitialized = false;


    private int totalEnemies;

    public void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
            InitMap();
        }
    }

    public void FixedUpdate()
    {
        gameTimer -= Time.deltaTime;
        if (currentGameState == GameState.Playing)
        {
            UpdateTimerUI();
        }

        if (gameTimer <= 0)
        {
            SwitchToGameState(GameState.OutOfTime);
        }
    }

    private void Start()
    {
        deathUI.SetActive(false);
    }

    private void UpdateTimerUI()
    {
        timerTMP.text = TimeFormatter(gameTimer);
    }

    private void SetTimerText(string text)
    {
        timerTMP.text = text;
    }

    public void SwitchToGameState(GameState newGameState)
    {
        currentGameState = newGameState;
        switch (newGameState)
        {
            case GameState.Starting:
                Debug.Log("The game is starting...");
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                break;
            case GameState.Playing:
                Debug.Log("The game has begun :)");
                break;
            case GameState.Win:
                LevelLoader.Instance.LoadNextLevel();

                Debug.Log("You won, rip.");
                break;
            case GameState.OutOfTime:
                SetTimerText("00:00");
                deathUI.SetActive(true);
                deathUI.transform.Find("Panel").GetComponent<Image>().sprite = outOfTimeScreen;

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                Debug.Log("You ran out of time, rip.");
                break;
            case GameState.PlayerDeath:
                deathUI.SetActive(true);
                deathUI.transform.Find("Panel").GetComponent<Image>().sprite = deathScreen;

                Cursor.lockState =  CursorLockMode.None;
                Cursor.visible = true;

                Debug.Log("You ran out of life, rip.");
                break;
        }
    }

    public void IsLevelEnd() {
        Debug.Log("IsLevelEnd Called");
        Debug.Log(GetEnemyCount() + "/" + totalEnemies);

        if (GetEnemyCount() <= (int)(totalEnemies/2))
        {
            SwitchToGameState(GameState.Win);
        }
    }

    public void InitMap() {
        if (hasBeenInitialized)
            return; 

        hasBeenInitialized = true;
        SpawnPlayer();
        SwitchToGameState(GameState.Playing);
    }

    public void SpawnPlayer() {
        if (spawnPoint)
            Instantiate(player, spawnPoint.transform.position, spawnPoint.transform.rotation);
    }

    public int GetEnemyCount()
    {
        if (enemyManager)
            return enemyManager.selectedEnemies.Count;
        else
            return 0;
    }

    public void SetTotalEnemyCount()
    {
        totalEnemies = GetEnemyCount();
    }

    public static string TimeFormatter(float seconds, bool forceHHMMSS = false)
    {
        float secondsRemainder = Mathf.Floor(seconds % 60);
        int minutes = ((int)(seconds / 60)) % 60;
        int hours = (int)(seconds / 3600);

        if (!forceHHMMSS)
        {
            if (hours == 0)
            {
                return String.Format("{0:00}:{1:00}", minutes, secondsRemainder);
            }
        }
        return String.Format("{0}:{1:00}:{2:00}", hours, minutes, secondsRemainder);
    }

}
