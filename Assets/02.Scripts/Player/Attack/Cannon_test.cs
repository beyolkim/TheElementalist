﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Photon.Pun;
using Photon.Realtime;

public class Cannon_test : MonoBehaviourPunCallbacks
{
    public SteamVR_Input_Sources righthand = SteamVR_Input_Sources.RightHand;
    public SteamVR_Action_Boolean grab = SteamVR_Actions.default_GrabGrip;
    public SteamVR_Action_Boolean Trigger = SteamVR_Actions.default_InteractUI;
    public SteamVR_Action_Pose pose = SteamVR_Actions.default_Pose;
    public SteamVR_Action_Vibration haptic = SteamVR_Actions.default_Haptic;


    private GameObject roundShot;
    private Transform firePos;
    private Transform barrel;

    void OnEnable()
    {
        PlayerManager.CannonAttack += CannonAttack;
    }
    void OnDisable()
    {
        PlayerManager.CannonAttack -= CannonAttack;
    }

    private void Awake()
    {
        roundShot = (GameObject)Resources.Load("Round_shot");
        barrel = transform.GetChild(0).Find("Small_cannon");
        firePos = transform.GetChild(0).Find("Small_cannon").Find("FirePos");
        transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;
        photonView.RPC("MoveCannonRPC", RpcTarget.All,null);
    }

    [PunRPC]
    void MoveCannonRPC()
    {
        transform.Rotate(new Vector3(0, -pose.GetVelocity(righthand).x, 0));
        barrel.Rotate(new Vector3(pose.GetVelocity(righthand).y, 0, 0));
    }

    void CannonAttack()
    {
        if (!photonView.IsMine)
            return;

        photonView.RPC("CannonAttackRPC", RpcTarget.All, null);
    }

    [PunRPC]
    void CannonAttackRPC()
    {
        GameObject shot = Instantiate(roundShot, firePos.position, Quaternion.identity);
        shot.GetComponent<Rigidbody>().AddForce(firePos.forward * 2600f);
    }
}
