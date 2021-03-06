﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class GameManager : Singleton<GameManager> {

    private const float TIME_TO_WAIT = 5f;

    private ScreenManager screenManager;
    private AutoNetworkManager networkManager;
    private bool isWaitingOtherPlayers;

    public bool IsOnePlayer = false;
    public Monster SelectedMonster { get; set; }

    public Monster[] monsters;

    public int InitialCredits = 1000;
    public int Credits;

    private List<int> _alreadyUsedStartedPositions; // TODO: move to its own class
    private GameObject[] _respawnPositions;

	void Start() 
    {
        screenManager = FindObjectOfType<ScreenManager>();

        networkManager = NetworkManager.singleton as AutoNetworkManager;
        networkManager.Initialize();
        networkManager.OnPlayersCountChangedEvent += NetworkManager_OnPlayersCountChangedEvent;

        Credits = InitialCredits;
	}

    void NetworkManager_OnPlayersCountChangedEvent(object sender, PlayersCountChangedEventArgs e)
    {
        if (isWaitingOtherPlayers)
        {
            screenManager.RefreshPlayers(e.PlayersCount);
            if (e.PlayersCount > 1)
                screenManager.StartCountDown(Mathf.CeilToInt(TIME_TO_WAIT));
        }
    }

    void Update()
    {
        if (isWaitingOtherPlayers)
        {
            if (IsOnePlayer || 
                (networkManager.PlayersCount > 1 && (Time.time - networkManager.LastTimePlayerJoined) > TIME_TO_WAIT))
            {
                // start game
                isWaitingOtherPlayers = false;
                StartCoroutine(StartMatch());
            }
        }
    }

    public void OnSelectMonster(Monster monster)
    {
        SelectedMonster = monster;

        if (Credits >= monster.CreditsCost)
        {
            Credits -= monster.CreditsCost;
            StartCoroutine(screenManager.RefreshCredits(Credits, () => screenManager.OpenWaitingOtherPlayers()));

            isWaitingOtherPlayers = true;
            networkManager.JoinOrCreateGame();
        }
        else
        {
            // TODO: error message
            Debug.Log("not enough credits");
        }


//        SceneManager.sceneLoaded += SceneManager_SceneLoaded;
//        StartCoroutine(LoadScene("Arena"));
    }

//    void SceneManager_SceneLoaded(Scene arg0, LoadSceneMode arg1)
//    {
//        SceneManager.sceneLoaded -= SceneManager_SceneLoaded;
//    }

    IEnumerator StartMatch()
    {
        screenManager.ShowLoading();
        yield return StartCoroutine(LoadScene("Arena"));
    }

    IEnumerator DelayedStart(float delay, IEnumerator enumerator)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(enumerator);
    }

    IEnumerator LoadScene(string sceneName)
    {
        yield return new WaitForSecondsRealtime(1f);
        networkManager.ServerChangeScene(sceneName);  // TODO: async?
        yield return null;
//        var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
//        while (!asyncLoad.isDone)
//        {
//            yield return null;
//        }
    }

    public Transform GetNextStartPosition()
    {
        int startPositionIndex;

        if (_alreadyUsedStartedPositions == null)
            _alreadyUsedStartedPositions = new List<int>();

        lock (_alreadyUsedStartedPositions)
        {
            if (_respawnPositions == null)
                _respawnPositions = GameObject.FindGameObjectsWithTag("Respawn");

            if (_alreadyUsedStartedPositions.Count >= _respawnPositions.Length)
            {
                Debug.Log("No more starting positions left!");
                return null;
            }

            startPositionIndex = Random.Range(0, _respawnPositions.Length);
            while (_alreadyUsedStartedPositions.Contains(startPositionIndex))
                startPositionIndex = Random.Range(0, _respawnPositions.Length);

            _alreadyUsedStartedPositions.Add(startPositionIndex);
        }

        return _respawnPositions[startPositionIndex].transform;
    }

    public GameObject GetMonsterPrefab(string monsterCode)
    {
        foreach (var m in monsters)
        {
            if (m.Code == monsterCode)
                return m.gameObject;
        }
        return null;
    }
}
