using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: remove duplicated code from Player
public class AIPlayer : MonoBehaviour { 

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

    private Transform playerTransform;
    private Monster playerMonster;

    // Use this for initialization
    void Start()
    {
        var monsterGO = Instantiate(MonsterPrefab, monsterSlot.transform);
        Monster = monsterGO.GetComponentInChildren<Monster>();

        anim = GetComponentInChildren<Animator>();

        GameManager.Instance.SetStartPosition(transform);

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerMonster = playerTransform.GetComponentInChildren<Monster>();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.LookAt(playerTransform); // TODO: lerp

        var direction = playerTransform.position - transform.position;

        var stopDistance = Monster.GetStopDistance(playerMonster);
        var stopDistanceSqr = stopDistance * stopDistance;

        var diferenceMagnitude = direction.magnitude - stopDistance;
        if (diferenceMagnitude <= 0)
        {
            anim.SetFloat("Forward", 0);
            return;
        }

        if (diferenceMagnitude > 1)
            diferenceMagnitude = 1f;

        //direction.Normalize();
        //transform.Translate(direction * Speed * Time.deltaTime, Space.World);

        transform.Translate(Vector3.forward * diferenceMagnitude * Speed * Time.deltaTime, Space.Self);
        transform.LookAt(playerTransform);

        anim.SetFloat("Forward", diferenceMagnitude);
        //anim.SetFloat("Turn", inputAxisH);

        // TODO: fight UI
    }

}
