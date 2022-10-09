using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

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
    public bool wrenchEnabled = false;
    [SerializeField]
    public bool shotgunEnabled = false;
    [SerializeField]
    public bool bombEnabled = false;

    private GameState currentGameState = GameState.None;

    private int currentLives = 0;


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
                break;
            case GameState.Playing:
                Debug.Log("The game has begun :)");
                break;
            case GameState.Win:
                Debug.Log("You won, rip.");
                break;
            case GameState.OutOfTime:
                SetTimerText("00:00");
                Debug.Log("You ran out of time, rip.");
                break;
            case GameState.PlayerDeath:
                deathUI.SetActive(true);

                Cursor.lockState =  CursorLockMode.None;
                Cursor.visible = true;

                Debug.Log("You ran out of life, rip.");
                break;
        }
    }

    public void IsLevelEnd() {

    }

    public void InitMap() {
        SpawnPlayer();
        ResetLifeCount();
        GetEnemyCount();
        SwitchToGameState(GameState.Playing);
    }

    public void ResetLifeCount()
    {
        currentLives = startingLives;
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

    public void UpdateGameUI()
    {

    }

    public static string TimeFormatter(float seconds, bool forceHHMMSS = false)
    {
        float secondsRemainder = Mathf.Floor((seconds % 60) * 100) / 100.0f;
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
