// David Kim 2024/4/26
// Description: Use Netcode to synchronize the player's head, hands, and root. This script is attached to the player's prefab.
// Following the tutorial from https://youtu.be/6fZ7LT5AeTw?si=9QcoxIA9VkCT3uWw

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using Unity.Netcode.Components;

[HelpURL("https://youtu.be/6fZ7LT5AeTw?si=9QcoxIA9VkCT3uWw")]

public class NetworkPlayer : NetworkBehaviour
{
    public Transform root;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    public Renderer[] meshToDisable;
    public Collider[] collidersToDestroy;

    private bool scaled = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        var myID = transform.GetComponent<NetworkObject>().NetworkObjectId;
        if (IsOwnedByServer)
        {
            transform.name = "Host:" + myID;    //this must be the host
            DebugConsole.Log("Host:" + myID);
        }
        else
        {
            transform.name = "Client:" + myID; //this must be the client 
            DebugConsole.Log("Client:" + myID);
        }
        if (IsOwner)
        {
            foreach (var item in meshToDisable)
            {
                item.enabled = false;
            }

            foreach (var col in collidersToDestroy)
            {
                Destroy(col);
            }
        }
    }

    void Update()
    {
        if (IsOwner)
        {
            root.position = OVRCameraRigReferencesForNetCode.instance.root.position;
            root.rotation = OVRCameraRigReferencesForNetCode.instance.root.rotation;

            head.position = OVRCameraRigReferencesForNetCode.instance.head.position;
            head.rotation = OVRCameraRigReferencesForNetCode.instance.head.rotation;

            leftHand.position = OVRCameraRigReferencesForNetCode.instance.leftHand.position;
            leftHand.rotation = OVRCameraRigReferencesForNetCode.instance.leftHand.rotation;

            rightHand.position = OVRCameraRigReferencesForNetCode.instance.rightHand.position;
            rightHand.rotation = OVRCameraRigReferencesForNetCode.instance.rightHand.rotation;
        }
        if (!IsServer && IsOwner)
        {
            if (!scaled)
            {
                DebugConsole.Log("Client scaled. ID: " + transform.GetComponent<NetworkObject>().NetworkObjectId);
                head.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                leftHand.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                rightHand.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                scaled = true;
            }
        }
    }
}
