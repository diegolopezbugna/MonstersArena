using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private Monster Monster { get; set; }

    private Animator anim;

    [SerializeField] private GameObject monsterSlot;

    public Monster MonsterPrefab;

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

	// Use this for initialization
	void Start()
    {
        var monsterGO = Instantiate(MonsterPrefab, monsterSlot.transform);
        Monster = monsterGO.GetComponentInChildren<Monster>();

        anim = GetComponentInChildren<Animator>();

        GameManager.Instance.SetStartPosition(transform);
	}
	
	// Update is called once per frame
	void Update()
    {
        ForwardTurnMove();

        if (Input.GetMouseButtonDown(0))
            anim.SetTrigger("Attack1");

        if (Input.GetMouseButtonDown(1))
            anim.SetTrigger("Attack2");
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
