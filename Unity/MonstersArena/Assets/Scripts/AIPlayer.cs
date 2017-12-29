using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour { 

    private Player player;
    private Animator anim;
    private CharacterController characterController;

    private Player targetPlayer;

    public void SetTargetPlayer(Player targetPlayer)
    {
        this.targetPlayer = targetPlayer;
    }

    void Start()
    {
        player = GetComponent<Player>();
        anim = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        //transform.LookAt(playerTransform); // TODO: lerp

        var direction = targetPlayer.transform.position - transform.position;

        var stopDistance = player.Monster.GetStopDistance(targetPlayer.Monster);
        var stopDistanceSqr = stopDistance * stopDistance;

        var diferenceMagnitude = direction.magnitude - stopDistance;
        if (diferenceMagnitude <= 0)
        {
            anim.SetFloat("Forward", 0);
            return;
        }

        if (diferenceMagnitude > 1)
            diferenceMagnitude = 1f;

        var forward = transform.TransformDirection(Vector3.forward);
//        transform.Translate(Vector3.forward * diferenceMagnitude * player.Speed * Time.deltaTime, Space.Self);
        characterController.Move(forward * diferenceMagnitude * player.Speed * Time.deltaTime);
        transform.LookAt(targetPlayer.transform);

        anim.SetFloat("Forward", diferenceMagnitude);
        //anim.SetFloat("Turn", inputAxisH);

        // TODO: fight UI
    }

}
