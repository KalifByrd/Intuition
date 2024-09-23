using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System.Threading.Tasks;

public class RelayHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text _joinCodeText;
    private List<string> validJoinCodes = new List<string>();
    [SerializeField] private TMP_InputField _joinInput;
    //[SerializeField] private GameObject _buttons;
    [SerializeField] private GameObject joinGameScreen;
    [SerializeField] private GameObject lobbyScreen;

    private UnityTransport _transport;
    private const int MaxPlayers = 2;

    private async void Awake()
    {
        _transport = FindObjectOfType<UnityTransport>();

        //_buttons.SetActive(false);

        await Authenticate();

        //_buttons.SetActive(true);
    }

    private static async Task Authenticate()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateGame()
    {
        joinGameScreen.SetActive(false);
        lobbyScreen.SetActive(true);

        Allocation a = await RelayService.Instance.CreateAllocationAsync(MaxPlayers);
        _joinCodeText.text = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);
        validJoinCodes.Add(_joinCodeText.text);

        _transport.SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);

        NetworkManager.Singleton.StartHost();
    }

    public async void JoinGame()
    { 
        joinGameScreen.SetActive(false);
        lobbyScreen.SetActive(false);
        

        JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(_joinInput.text);

        _transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);

        NetworkManager.Singleton.StartClient();
        StartCoroutine(WaitForConnection());
    }
    private IEnumerator WaitForConnection()
    {
        // Wait until the client is connected
        yield return new WaitUntil(() => NetworkManager.Singleton.IsClient && NetworkManager.Singleton.IsConnectedClient);

        // Now that the client is connected, execute the desired method
        NetworkGameManager.Instance.RequestLobbyScreenSetActiveServerRpc(NetworkManager.Singleton.LocalClientId, false);
    }
}
