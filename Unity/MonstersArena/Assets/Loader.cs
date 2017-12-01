using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {

    public GameManager GameManagerPrefab;

	void Awake() 
    {
        if (GameManager.Instance == null)
            Instantiate(GameManagerPrefab);
	}
	
}
