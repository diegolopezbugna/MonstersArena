using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {

    public Animator screenPlayMode;
    public Animator screenSelectMonster;
    public GameObject loading;
    public GameObject monsterSelectGridContainer;
    public GameObject monsterButtonPrefab;

    public Monster SelectedMonster { get; set; }

    public Monster[] monsters;

    private List<int> _alreadyUsedStartedPositions;
    private GameObject[] _respawnPositions;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this.gameObject);

        if (monsterSelectGridContainer != null)
        {
            foreach (var m in monsters)
            {
                var mb = Instantiate(monsterButtonPrefab, monsterSelectGridContainer.transform);
                mb.name = "mb_" + m.Code;
                mb.GetComponentInChildren<Text>().text = m.Name;
                mb.GetComponentInChildren<Button>().onClick.AddListener(() =>
                    {
                        OnSelectMonster(m);
                    });
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnSelectOnePlayer()
    {
        ScreenManager.Instance.OpenPanel(screenSelectMonster);
    }

    public void OnSelectMultiplayer()
    {
    }

    public void OnSelectMonster(Monster monster)
    {
        SelectedMonster = monster;
        loading.SetActive(true);
        ScreenManager.Instance.CloseCurrent();
        SceneManager.sceneLoaded += SceneManager_SceneLoaded;
        StartCoroutine(DelayedStart(1f, LoadScene("Arena")));
    }

    void SceneManager_SceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponentInChildren<Player>().MonsterPrefab = SelectedMonster;
        SceneManager.sceneLoaded -= SceneManager_SceneLoaded;
    }

    IEnumerator DelayedStart(float delay, IEnumerator enumerator)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(enumerator);
    }

    IEnumerator LoadScene(string sceneName)
    {
        var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void SetStartPosition(Transform transform)
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
                return;
            }

            startPositionIndex = Random.Range(0, _respawnPositions.Length);
            while (_alreadyUsedStartedPositions.Contains(startPositionIndex))
                startPositionIndex = Random.Range(0, _respawnPositions.Length);

            _alreadyUsedStartedPositions.Add(startPositionIndex);
        }

        var respawnPositionTransform = _respawnPositions[startPositionIndex].transform;
        transform.position = respawnPositionTransform.position;
        transform.rotation = respawnPositionTransform.rotation;
    }
}
