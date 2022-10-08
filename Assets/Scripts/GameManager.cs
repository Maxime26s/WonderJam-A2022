using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; set; }

    public GameObject player;

    public int score;

    public List<GameObject> spawnPoints;

    public void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
            DontDestroyOnLoad(this);
            //playerList = PlayerManager.Instance.playerList;
            InitMap();
            score = 0;
        }
    }

    public void IsLevelEnd() {

    }

    public void InitMap() {
        spawnPoints = GameObject.FindGameObjectsWithTag("spawnpoint").ToList();
    }

    public void SpawnThePachinko() {
    }

    private void CleanUp() {



    }

    public void AddScore(GameObject gameObject, bool win) {
        if (win)
            score += 1;
        else
            score += 0;
    }
}
