using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkManagerManager : NetworkBehaviour
{
    public NetworkVariable<int> playerCount = new NetworkVariable<int>(0);

    // Update is called once per frame
    void Update()
    {
        
    }
}
