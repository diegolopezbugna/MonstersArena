using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    private Monster Monster { get; set; }

    private Animator anim;
    private NetworkAnimator netAnim;

    [SerializeField] private GameObject monsterSlot;

    [SerializeField] private Monster DebugMonsterPrefab;

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
            return;
        
        ForwardTurnMove();

        if (Input.GetMouseButtonDown(0))
            netAnim.SetTrigger("Attack1");

        if (Input.GetMouseButtonDown(1))
            netAnim.SetTrigger("Attack2");
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

}
