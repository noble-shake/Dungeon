using System.Collections.Generic;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using Unity.Services.Relay.Models;
using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Qos;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using System;
using Unity.Services.Core;
using System.Collections;

public class LobbyGameHostingControl
{
    private LobbyDataControl dataControl;
    private UnityTransport unityTransport;

    public event Action OnRelayAllocationError;

    public List<string> availableRegions { get; private set; } = new List<string>();

    public LobbyGameHostingControl(LobbyDataControl dataControl)
    {
        this.dataControl = dataControl;
        // this.unityTransport = unityTransport;
        // TODO : 추후 필요에 따라 추가
        // UpdateRegions();
    }

    // 일차적으로는 로비 씬으로 이동한다.
    // 추후 다른 함수로 진짜 던전으로 이동한다. 
    public async Task StartGameAsHost(int playersCount)
    {
        // string selectedRegion = PlayerDataKeeper.selectedRegion;
        string joinCode;

        try
        {
            //Allocate game session in Relay service
            // 지역을 따로 설정 안 해주면 아마 가장 가까운 지역으로 설정할 텐데...
            // Allocation allocation = await RelayService.Instance.CreateAllocationAsync(playersCount, selectedRegion);
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(playersCount);

            //Get join code for other players to connect this game
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            //Link current network client to allocated game
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            OnRelayAllocationError?.Invoke();
            return;
        }

        // 접속콜백 테스트용 코드
        // NetworkManager.Singleton.OnClientConnectedCallback += TestFunction;
        //Start game session
        NetworkManager.Singleton.StartHost();
        // SceneLoadManager.Instance.SubscribeOnNetworkSceneEvents();


        //Load game scene
        // TODO : 추후 SceneLoadManager 개편 후 수정
        SceneLoadManager.Instance.LoadNetworkScene(SceneLoadManager.Instance.Scene_Lobby);

        //Make sure host is on game scene before we allow other users to connect
        int delayBeforePublishingJoinCode = 1000;
        await Task.Delay(delayBeforePublishingJoinCode);

        //Store join code in lobby
        UpdateLobbyOptions options = new UpdateLobbyOptions();
        options.Data = new Dictionary<string, DataObject>()
        {
            {
              "joinCode", new DataObject(
              visibility: DataObject.VisibilityOptions.Public,
              value: joinCode)
            }
        };

        //Send join code
        dataControl.currentLobby = await LobbyService.Instance.UpdateLobbyAsync(dataControl.currentLobby.Id, options);
    }
  
    public async void JoinHostedGameAsClient(Lobby lobby)
    {
        string joinCode = lobby.Data["joinCode"].Value;
        try
        {
            //Find and join game session
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            //Link current network client to allocated game
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            OnRelayAllocationError?.Invoke();
            return;
        }

        // StartClientLate();
        NetworkManager.Singleton.StartClient();

        //Join game session as client

        // 씬이 수동으로 로드되지 않는다.
        // SceneLoadManager.Instance.SubscribeOnNetworkSceneEvents();
        // 서버가 자동으로 클라이언트에게 씬 동기화 요청을 보낸다.

    }

    // private async void UpdateRegions()
    // {
    //     //Update regions once in few days
    //     if (PlayerDataKeeper.lastRegionsUpdateTime + new TimeSpan(SettingsManager.Instance.lobby.regionsUpdateRateHours, 0, 0) > DateTime.Now)
    //     {
    //         availableRegions = PlayerDataKeeper.availableRegions;
    //         return;
    //     }

    //     int fixedDeltaTime = (int)(Time.fixedDeltaTime * 1000);

    //     //Wait for initialization
    //     while (UnityServices.State != ServicesInitializationState.Initialized)
    //         await Task.Delay(fixedDeltaTime);

    //     //Wait for sign in
    //     while (AuthenticationService.Instance.IsSignedIn == false)
    //         await Task.Delay(fixedDeltaTime);

    //     //Get regions
    //     var regionSearchResult = await QosService.Instance.GetSortedQosResultsAsync("relay", null);

    //     foreach (var result in regionSearchResult)
    //     {
    //         Debug.Log("Add region: " + result.Region);
    //         availableRegions.Add(result.Region);
    //     }

    //     //Save regions
    //     PlayerDataKeeper.availableRegions = availableRegions;
    //     PlayerDataKeeper.selectedRegion = availableRegions[0];
    //     PlayerDataKeeper.lastRegionsUpdateTime = DateTime.Now;
    // }
}
