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
    private bool shotgunEnabled = false;
    [SerializeField]
    private bool bombEnabled = false;

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

    private void UpdateTimerUI()
    {
        timerTMP.text = gameTimer.ToString();
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
                Debug.Log("You ran out of time, rip.");
                break;
            case GameState.PlayerDeath:
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
        Instantiate(player, spawnPoint.transform.position, spawnPoint.transform.rotation);
    }

    public int GetEnemyCount()
    {
        return enemyManager.selectedEnemies.Count;
    }

    public void UpdateGameUI()
    {

    }
}
