using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;

public class GameManager : Singleton<GameManager> {

    private const float TIME_TO_WAIT = 5f;

    private ScreenManager screenManager;
    private AutoNetworkManager networkManager;
    private bool isWaitingOtherPlayers;

    public Monster SelectedMonster { get; set; }

    public Monster[] monsters;

    public int InitialCredits = 1000;
    public int Credits;

//    private List<int> _alreadyUsedStartedPositions;
//    private GameObject[] _respawnPositions;

	void Start() 
    {
        screenManager = FindObjectOfType<ScreenManager>();

        networkManager = NetworkManager.singleton as AutoNetworkManager;
        networkManager.Initialize();
        networkManager.OnClientSceneChangedEvent += NetworkManager_OnClientSceneChangedEvent;
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

    void NetworkManager_OnClientSceneChangedEvent(object sender, EventArgs e)
    {
        ClientScene.Ready(networkManager.client.connection);
        ClientScene.AddPlayer(0);
    }

    void Update()
    {
        if (isWaitingOtherPlayers)
        {
            if (networkManager.PlayersCount > 1 && (Time.time - networkManager.LastTimePlayerJoined) > TIME_TO_WAIT)
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
            Credits = Credits -= monster.CreditsCost;
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
//        ClientScene.Ready(networkManager.client.connection);
//        ClientScene.AddPlayer(0);
    }

    IEnumerator DelayedStart(float delay, IEnumerator enumerator)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(enumerator);
    }

    IEnumerator LoadScene(string sceneName)
    {
        yield return new WaitForSecondsRealtime(1f);
        networkManager.ServerChangeScene(sceneName);
        yield return null;
//        var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
//        while (!asyncLoad.isDone)
//        {
//            yield return null;
//        }
    }

//    public void SetStartPosition(Transform transform)
//    {
//        int startPositionIndex;
//
//        if (_alreadyUsedStartedPositions == null)
//            _alreadyUsedStartedPositions = new List<int>();
//
//        lock (_alreadyUsedStartedPositions)
//        {
//            if (_respawnPositions == null)
//                _respawnPositions = GameObject.FindGameObjectsWithTag("Respawn");
//
//            if (_alreadyUsedStartedPositions.Count >= _respawnPositions.Length)
//            {
//                Debug.Log("No more starting positions left!");
//                return;
//            }
//
//            startPositionIndex = Random.Range(0, _respawnPositions.Length);
//            while (_alreadyUsedStartedPositions.Contains(startPositionIndex))
//                startPositionIndex = Random.Range(0, _respawnPositions.Length);
//
//            _alreadyUsedStartedPositions.Add(startPositionIndex);
//        }
//
//        var respawnPositionTransform = _respawnPositions[startPositionIndex].transform;
//        transform.position = respawnPositionTransform.position;
//        transform.rotation = respawnPositionTransform.rotation;
//    }
}
