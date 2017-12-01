using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {

    public GameManager GameManagerPrefab;
    public GameObject NetworkManagerPrefab;

	void Awake() 
    {
        if (GameManager.Instance == null)
            Instantiate(GameManagerPrefab);

        if (GameObject.Find("NetworkManager") == null)
            Instantiate(NetworkManagerPrefab);
	}
	
}
