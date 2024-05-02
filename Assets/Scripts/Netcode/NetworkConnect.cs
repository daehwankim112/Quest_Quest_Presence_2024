// David Kim 2024/4/27
// Description: Use Relay and Netcode to create a networked VR experience. This script is used to connect to the network and create or join a room. This script is used to connect to the network and create or join a room.
// Following the tutorial from https://youtu.be/Pry4grExYQQ?si=7Jh1pwQdKrPFnWrz and https://youtu.be/sPKS3vjwvpU?si=4zhDWuL8SApYPniC

using UnityEngine;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Collections.Generic;

[HelpURL("https://youtu.be/Pry4grExYQQ?si=7Jh1pwQdKrPFnWrz")]

public class NetworkConnect : MonoBehaviour
{
    public static NetworkConnect instance;
    
    [SerializeField] TextMeshProUGUI debugConsole;

    [SerializeField] int maxConnections = 20;
    [SerializeField] UnityTransport transport;

    Lobby _currentLobbby;

    async void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
        
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed In" + AuthenticationService.Instance.PlayerId);
            debugConsole.text += "Signed In" + AuthenticationService.Instance.PlayerId;
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public static async void Create(string lobbyName)
    {
        try
        {
            Debug.Log("Host - Creating an allocation.");
            instance.debugConsole.text += "Host - Creating an allocation.";

            // Once the allocation is created, you have ten seconds to BIND
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(instance.maxConnections);
            
            instance.transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

            // Create a lobby
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = false;
            lobbyOptions.Data = new Dictionary<string, DataObject>();
            
            
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("newJoinCode" + joinCode);
            instance.debugConsole.text += "newJoinCode" + joinCode;
            
            DataObject joinCodeDataObj = new DataObject(DataObject.VisibilityOptions.Public, joinCode);
            lobbyOptions.Data.Add("joinCode", joinCodeDataObj);

            DataObject lobbyNameDataObj = new DataObject(DataObject.VisibilityOptions.Public, lobbyName);
            lobbyOptions.Data.Add("lobbyName", lobbyNameDataObj);

            instance._currentLobbby = await Lobbies.Instance.CreateLobbyAsync("Lobby Name", instance.maxConnections, lobbyOptions);
            
            NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e.Message);
            instance.debugConsole.text += e.Message;
        }
    }

    public static async void Join(string relayJoinCode)
    {
        try
        {
            Debug.Log("Joining Relay with " + relayJoinCode);
            //debugConsole.text += "Joining Relay with " + relayJoinCode;
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode); 
            instance.transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);
            
            NetworkManager.Singleton.StartClient();

            if (NetworkManager.Singleton.IsClient)
            {
                Debug.Log("Client - Connected to the server.");
                instance.debugConsole.text += "Client - Connected to the server.";
            }
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e.Message);
            instance.debugConsole.text += e.Message;
        }
    }

    public static string GetJoinCode(Lobby lobby)
    {
        return lobby.Data.TryGetValue("joinCode", out var joinCode) ? joinCode.Value : string.Empty;
    }

    public static string GetLobbyName(Lobby lobby)
    {
        return lobby.Data.TryGetValue("lobbyName", out var hostName) ? hostName.Value : string.Empty;
    }
}
