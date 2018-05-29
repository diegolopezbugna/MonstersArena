using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour { 

    private Player player;
    private Animator anim;
    private CharacterController characterController;

    private Player targetPlayer;

    private bool isAttacking = false;

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
        if (isAttacking)
            return;
        if (targetPlayer.Monster.HitPoints <= 0)  // TODO: AIPlayer should have been disabled when player dies
            return;
        
        var direction = targetPlayer.transform.position - transform.position;

        var stopDistance = player.Monster.GetStopDistance(targetPlayer.Monster);
        var stopDistanceSqr = stopDistance * stopDistance;

        var diferenceMagnitudeToStopDistance = direction.magnitude - stopDistance;
        if (diferenceMagnitudeToStopDistance <= 0.01)
        {
            anim.SetFloat("Forward", 0);
            TryAttack();
            return;
        }

        if (diferenceMagnitudeToStopDistance > 1)
            diferenceMagnitudeToStopDistance = 1f;

        var forward = transform.TransformDirection(Vector3.forward);
//        transform.Translate(Vector3.forward * diferenceMagnitude * player.Speed * Time.deltaTime, Space.Self);
        characterController.Move(forward * diferenceMagnitudeToStopDistance * player.Speed * Time.deltaTime);
        transform.LookAt(targetPlayer.transform); // TODO: lerp

        anim.SetFloat("Forward", diferenceMagnitudeToStopDistance);
        //anim.SetFloat("Turn", inputAxisH);

        // TODO: fight UI
    }

    void TryAttack() {
        if (!isAttacking)
            Attack();
    }

    void Attack() {
        isAttacking = true;
        anim.SetTrigger("Attack1"); // TODO: attack2
        StartCoroutine(EndAttack());
    }

    IEnumerator EndAttack() {
        yield return new WaitForSecondsRealtime(2f); // TODO: check animation or Animation event?
        isAttacking = false;
    }

}
