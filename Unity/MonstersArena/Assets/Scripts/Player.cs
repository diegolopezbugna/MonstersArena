﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    private Monster Monster { get; set; }

    private Animator anim;
    private NetworkAnimator netAnim;

    [SerializeField] private GameObject monsterSlot;

    [SerializeField] private Monster DebugMonsterPrefab;

    [SyncVar] Vector3 realPosition = Vector3.zero;
    [SyncVar] Quaternion realRotation;
    private float updateInterval;

    public int Speed
    {
        get {
            return Monster.Speed;
        }
    }

    public int RotationSpeed
    {
        get {
            return Monster.RotationSpeed;
        }
    }

	void Start()
    {
        var monsterPrefab = GameManager.Instance.SelectedMonster != null ? GameManager.Instance.SelectedMonster : DebugMonsterPrefab;
        var monsterGO = Instantiate(monsterPrefab, monsterSlot.transform);
        Monster = monsterGO.GetComponentInChildren<Monster>();

        anim = GetComponentInChildren<Animator>();
        anim.runtimeAnimatorController = Monster.AnimatorController;
        anim.avatar = Monster.Avatar;
        netAnim = GetComponentInChildren<NetworkAnimator>();

        for (int i = 0; i < anim.parameterCount; i++)
            netAnim.SetParameterAutoSend(i, true);
	}
	
    public override void OnStartLocalPlayer()
    {
        GetComponent<AudioListener>().enabled = true;
        CameraManager.Instance.SetPlayer(this);
    }

	void Update()
    {
        if (!isLocalPlayer)
        {
            transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
            return;
        }
        
        ForwardTurnMove();

        if (Input.GetMouseButtonDown(0))
            netAnim.SetTrigger("Attack1");

        if (Input.GetMouseButtonDown(1))
            netAnim.SetTrigger("Attack2");

        // update the server with position/rotation
        updateInterval += Time.deltaTime;
        if (updateInterval > 0.11f) // 9 times per second
        {
            updateInterval = 0;
            CmdSync(transform.position, transform.rotation);
        }
	}

    private void ForwardTurnMove()
    {
        var inputAxisV = Input.GetAxis("Vertical");
        var inputAxisH = Input.GetAxis("Horizontal");

        float translation = inputAxisV * Speed * Time.deltaTime;
        float rotation = inputAxisH * RotationSpeed * Time.deltaTime;
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);

        anim.SetFloat("Turn", inputAxisH);
        anim.SetFloat("Forward", inputAxisV);
    }

    private void FirstPersonCameraMove()
    {
        // TODO: need to move the head
    }

    [Command]
    void CmdSync(Vector3 position, Quaternion rotation)
    {
        realPosition = position;
        realRotation = rotation;
    }
}
