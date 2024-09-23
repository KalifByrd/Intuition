using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using System;

public class PlayerSpawnManager : NetworkBehaviour
{
    private Vector3 playerBattlePos = new Vector3(-427.5f, -39.5f, 232.1171f); // left side of the screen
    private Vector3 opponentBattlePos = new Vector3(427.5f, -39.5f, 232.1171f); // right side of the screen
    private Transform thrDWorld; // parent of all 3D elements
    // Variable that is consistant on all clients and server
    [SerializeField] private static NetworkVariable<int> playerCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Start is called on the frame when a script is enabled
    void Start()
    {
        // Get reference to parent object of all 3D elements
        thrDWorld = GameObject.Find("3D Elements").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(IsLocalPlayer && playerCount.Value == 2)
        {
            TellClientToParentServerRpc();
        }
    }

    // When a Player spawns this function is called  
    public override void OnNetworkSpawn()
    {
        if(IsLocalPlayer) // checks if the player spawned *NOT* the opponent
        {
            if(NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
            { // this shows the players character on the left side of the screen
                transform.position = playerBattlePos;
                IncreasePlayerCountServerRpc(); // the player told the server they spawned and asks the server to add 1 to playerCount
            }
        }
        else // checks if the opponent spawned *NOT* the player
        {
            if(NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
            { // this shows the opponents character on the right side of the screen
                transform.position = opponentBattlePos;
            }
        }
    }

    // Server Rpc's
    // *** The Client askes the Server to do something, then the Server executes it *** \\
    [ServerRpc]
    private void IncreasePlayerCountServerRpc() // adds 1 to playerCount
    {
        playerCount.Value++;
    }
    [ServerRpc]
    // This is a special case:
    // This method is called by the client but a client cannot call a ClientRpc,
    // so we must ask the server to call the ClientRpc.
    // This happens when we want to manipulate gameobjects that arent local on the local client
    // ** Like a waterfall ** \\
    private void TellClientToParentServerRpc()
    {
        SetParentClientRpc();
    }

    // Client Rpc's
    // *** The Server orders the Clients to do something *** \\
    [ClientRpc]
    private void SetParentClientRpc()
    {
        // This will be executed on all clients
        // the server adding the localplayer on the local player's client to the parent 
        // and the non-local player on the non-local player's client to the parent
        // this must be called on both clients
        transform.SetParent(thrDWorld, false);
    }
}
