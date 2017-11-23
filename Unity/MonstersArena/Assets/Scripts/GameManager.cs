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

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this.gameObject);

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
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        StartCoroutine(DelayedStart(1f, LoadScene("Arena")));
    }

    void SceneManager_sceneLoaded (Scene arg0, LoadSceneMode arg1)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponentInChildren<Player>().MonsterPrefab = SelectedMonster;
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
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

}
