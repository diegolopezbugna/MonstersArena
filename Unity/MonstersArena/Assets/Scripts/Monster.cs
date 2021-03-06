﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour {

    public string Code;
    public string Name; // TODO: localization
    public int Speed;
    public int RotationSpeed;
    public int Damage1;
    public int Damage2;
    public int HitPoints;
    public int CreditsCost;

    public RuntimeAnimatorController AnimatorController;
    public Avatar Avatar;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public float GetStopDistance(Monster enemy)
    {
        return 5.0f; // TODO: stop distance depending on enemy & self
    }

    public int GetDamageByWeapon(GameObject weapon)
    {
        if (weapon.CompareTag("Weapon2"))
            return Damage2;
        else
            return Damage1;
    }

}
