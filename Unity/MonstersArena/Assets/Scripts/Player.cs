using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    public Monster Monster { get; set; }

    private Animator anim;
    private NetworkAnimator netAnim;
    private CharacterController characterController;

    public bool isAI = false;

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
        Monster = GetComponent<Monster>();
        anim = GetComponent<Animator>();
        netAnim = GetComponent<NetworkAnimator>();
        characterController = GetComponent<CharacterController>();

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
        if (isAI)
            return;
        
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
//        transform.Translate(0, 0, translation);
        var forward = transform.TransformDirection(Vector3.forward);
        characterController.Move(forward * translation);
        transform.Rotate(0, rotation, 0);

        anim.SetFloat("Turn", inputAxisH); // netAnim??
        anim.SetFloat("Forward", inputAxisV); // netAnim??
    }

    private void FirstPersonCameraMove()
    {
        // TODO: need to move the head
    }

    private void AnimationHitStart() {
        Debug.LogFormat("HIT START {0}", Monster.Damage1);
        SetWeaponsCollidersEnabled(true);
    }

    private void AnimationHitEnd() {
        Debug.LogFormat("HIT END {0}", Monster.Damage1);
        SetWeaponsCollidersEnabled(false);
    }

    private void SetWeaponsCollidersEnabled(bool enabled) {
        var boxColliders = gameObject.GetComponentsInChildren<BoxCollider>(true);
        foreach (var c in boxColliders) {
            // TODO: this enables all colliders!!! not only the corresponding attack!!  or check tag or add a lists of weapons colliders to Monster 
            c.enabled = enabled;
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.transform.IsChildOf(transform))
            return; // collided with itself!
        if (Monster.HitPoints <= 0)
            return; // dead

        var attackingMonster = other.GetComponentInParent<Monster>();
        var damage = attackingMonster.GetDamageByWeapon(other.gameObject);

        Monster.HitPoints -= damage;
        Debug.LogFormat("HIT {0}, current HitPoints: {1}", Monster.Name, Monster.HitPoints);

        if (Monster.HitPoints > 0)
        {
            anim.SetTrigger("TakeHit"); // todo: disable movement   // anim o netAnim??
        }
        else
        {
            anim.SetTrigger("Die"); // todo: disable movement   // anim o netAnim??
            StopComponents();
        }

    }

    private void StopComponents() {
        SetWeaponsCollidersEnabled(false);
        var aiPlayer = GetComponent<AIPlayer>();
        if (aiPlayer != null)
            aiPlayer.enabled = false;
        Destroy(characterController);
        enabled = false;
    }

    [Command]
    void CmdSync(Vector3 position, Quaternion rotation)
    {
        realPosition = position;
        realRotation = rotation;
    }
}
