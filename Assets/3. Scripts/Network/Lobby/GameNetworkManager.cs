using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine.SceneManagement;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;

public class GameNetworkManager : MonoBehaviour
{
    public string demoVersion = "Dev";
    private static GameNetworkManager instance;

    private LobbyActivityControl activityControl;
    private LobbyDataControl dataControl;
    private LobbyGameHostingControl gameHostingControl;

    private Player playerData;

    private bool isSearchingLobby = false;

    public List<Player> players => dataControl.players;
    public int maxPlayers => dataControl.maxPlayers;

    private int maxPlayerCount = 4;

    public bool isLobbyAvailable => dataControl.isLobbyAvailable;
    public bool isLobbyOwner => dataControl.isLobbyOwner;
    public string lobbyCode => dataControl.currentLobby.LobbyCode;

    public bool isOfflineMode { get; set; }

    public List<string> availableRegions => gameHostingControl.availableRegions;
    public GameLaunchStatus gameLaunchStatus => activityControl.gameLaunchStatus;

    public event Action<string> OnErrorOccurred;
    public event Action OnRelayAllocationErrorOccurred;

    public static GameNetworkManager Instance { get => instance; private set => instance = value; }
    public int maxLobbyPlayerCount { get => maxPlayerCount; set => maxPlayerCount = value; }
    // 유니티 로비의 Player 객체를 이용하는 것이기 때문에 나중에 Steam으로 교체할 때 이 부분을 고려해야 한다. 
    public Player player { get => playerData; set => playerData = value; }
    public bool IsSearchingLobby { get => isSearchingLobby; set => isSearchingLobby = value; }
    public Lobby CurrentLobby { get => dataControl.currentLobby; }

    private void Awake()
    {
        ProcessInitialization();
    }

    private async void ProcessInitialization()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLand는 어차피 NetworkManager가 게임 오브젝트에 붙어있기 때문에 자동으로 실행 됨.
        }
        else
        {
            Destroy(this);
            return;
        }

        // if (UnityServices.State == ServicesInitializationState.Uninitialized)
        // 유니티 서비스 초기화. 유니티 로비, 릴레이 서비스를 이용하려면 필요하다.
        await UnityServices.InitializeAsync();
        // if (AuthenticationService.Instance.IsSignedIn == false)
        // 익명으로 서비스 시스템에 로그인하는 코드인 듯? 두 개의 코드가 모두 필요하단 것만 안다.
        // await AuthenticationService.Instance.SignInAnonymouslyAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

    }

    private void Start()
    {
        dataControl = new LobbyDataControl();
        gameHostingControl = new LobbyGameHostingControl(dataControl);
        activityControl = new LobbyActivityControl(dataControl, gameHostingControl);

        player = dataControl.GeneratePlayerObject();

        dataControl.OnLobbyNoLongerExistsError += OnLobbyNoLongerExistsError;
        dataControl.OnLocalPlayerRemovedError += OnLocalPlayerRemovedError;
        // gameHostingControl.OnRelayAllocationError += OnRelayAllocationError;

        OnRelayAllocationErrorOccurred += OnRelayAllocationError;
        NetworkManager.Singleton.GetComponent<UnityTransport>().OnTransportEvent += OnTransportEvent;
        // 모두가 로드를 완료했으면 화면을 다시 밝게 해준다.
        // 노멀 씬에서 네트워크 씬으로 전환할 때 
        // 1. 메인 메뉴에서 로비 씬으로 갈 때 -> 로컬에서 자체적으로 페이드 아웃 처리
        // 2. 로비 리스트 씬에서 로비 씬으로 갈 때 -> 로컬에서 자체적으로 페이드 아웃 처리
        // 즉 노멀 씬에서 네트워크 씬으로 전환할 때 페이드 아웃 처리는 네트워크적으로 처리해줄 게 없음.

        // 네트워크 씬에서 네트워크 씬으로 전환할 때 -> 게임매니저가 분명 있음. 
        // 1. 로비 씬에서 던전 씬으로 갈 때 -> 게임 매니저에게 RPC 보내서 페이드 아웃 처리 -> 그후 네트워크 씬 로드
        // 2. 던전 씬에서 로비 씬으로 갈 때 -> 게임 매니저에게 RPC 보내서 페이드 아웃 처리 -> 그후 네트워크 씬 로드

        // 네트워크 씬 로드가 완료됐을 때
        // 1. 클라이언트마다 자체적으로 씬 로드가 완료 됐을 때 -> 플레이어의 네트워크 스폰이 끝났을 때 페이드인 처리
        // 2. 모든 클라이언드의 씬 로드가 끝났을 때 -> OnLoadEventCompleted 델리게이트를 통해서 페이드인 처리
        NetworkManager.Singleton.OnClientConnectedCallback += SubscribeOnNetworkSceneEvents;
        NetworkManager.Singleton.OnClientDisconnectCallback += UnsubscribeOnNetworkSceneEvents;
        NetworkManager.Singleton.OnClientDisconnectCallback += GoToBootingScene;
    }

    private void GoToBootingScene(ulong clientID)
    {
        if (clientID == NetworkManager.Singleton.LocalClientId && NetworkManager.Singleton.IsHost == false)
        {
            Debug.Log("나 실행됐음");
            NetworkManager.Singleton.Shutdown();
            SceneLoadManager.Instance.LoadRegularScene("Scene_Booting", false);
        }
    }

    private void UnsubscribeOnNetworkSceneEvents(ulong clientID)
    {
        if (clientID == NetworkManager.Singleton.LocalClientId)
        {
            // 씬 로드가 완료됐을 때 페이드인 처리

            // NetworkManager.Singleton.SceneManager.OnLoadComplete -= FadeInWhenSceneLoaded;
        }
    }

    private void SubscribeOnNetworkSceneEvents(ulong clientID)
    {
        Debug.Log("되긴 된겅미?");
        if (clientID == NetworkManager.Singleton.LocalClientId)
        {
            // 씬 로드가 완료됐을 때 페이드인 처리
            Debug.Log("얘가 된건가?");
            if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost)
            {
                StartCoroutine(SceneLoadManager.Instance.FadeIn(1f));
            }
            NetworkManager.Singleton.SceneManager.OnLoadComplete += FadeInWhenSceneLoaded;
        }
    }

    private void FadeInWhenSceneLoaded(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        Debug.Log("네트워크 씬 로드가 완료됐습니다.");
        StartCoroutine(SceneLoadManager.Instance.FadeIn(1f));
    }



    // TODO : quckijoin으로 바꿀 것. 지금은 조건에 맞는 방 검색임.
    // public async void JoinOrCreateLobby(LobbyParameters lobbyParameters, Action onSuccess = null, Action<string> onFail = null)
    public async void JoinOrCreateLobby(Action onSuccess = null, Action<string> onFail = null)
    {
        //Search options
        QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();
        options.Filter = new List<QueryFilter>();

        //Game mode filter
        // QueryFilter gameModeFilter = new QueryFilter(QueryFilter.FieldOptions.S1, lobbyParameters.mode, QueryFilter.OpOptions.EQ);
        // options.Filter.Add(gameModeFilter);

        //Region filter
        // QueryFilter regionFilter = new QueryFilter(QueryFilter.FieldOptions.S2, PlayerDataKeeper.selectedRegion, QueryFilter.OpOptions.EQ);
        // options.Filter.Add(regionFilter);

        //Version filter
        // QueryFilter versionFilter = new QueryFilter(QueryFilter.FieldOptions.S3, lobbyParameters.version, QueryFilter.OpOptions.EQ);
        // options.Filter.Add(versionFilter);

        //Player (lobby user)
        // options.Player = dataControl.GeneratePlayerObject(PlayerDataKeeper.name);

        try
        {
            //Try join any free lobby using search parameters
            dataControl.currentLobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);

            //Subscribe to lobby updates
            dataControl.InitializeLobbyCallbacks();

            onSuccess?.Invoke();

            Debug.Log("Successfully connected.");
            Debug.Log("Status: " + gameLaunchStatus);

            //Joined player will wait for lobby updates
            //Check LobbyActivityControl.OnLobbyUpdated method for more details
        }
        catch (Exception exception)
        {
            onFail?.Invoke(exception.Message);
            Debug.Log("No free lobby found. Creating new one.");

            //It seems there are no appropriate lobbies, so we got to create new one
            // CreateLobby(lobbyParameters, onSuccess);
        }

        isOfflineMode = false;
    }

    public async Task CreateLobby(LobbyParameters lobbyParameters, string dungeonName, Action onSuccess = null)
    {
        if (lobbyParameters == null)
        {
            lobbyParameters = new LobbyParameters();
        }
        lobbyParameters.version = demoVersion;

        string lobbyName = "lobby"; //default name.

        CreateLobbyOptions options = new CreateLobbyOptions();

        options.IsPrivate = !lobbyParameters.isPublic; //Is it free to join?
        // options.Player = dataControl.GeneratePlayerObject(PlayerDataKeeper.name);
        options.Player = player; // start에서 초기화되었음. 추후 세팅 필요.

        //Add search parameters, for other players to find this lobby
        options.Data = new Dictionary<string, DataObject>
        {
               {
                   "dungeon",new DataObject(
                    visibility: DataObject.VisibilityOptions.Public,
                    value: dungeonName, //game mode
                    index: DataObject.IndexOptions.S1) //search parameters suppose to be indexed
               },
               {
                   "version",new DataObject(
                    visibility: DataObject.VisibilityOptions.Public,
                    value: lobbyParameters.version, //project version
                    index: DataObject.IndexOptions.S3) //indexed
               },
               {
                    "time", new DataObject(
                    visibility: DataObject.VisibilityOptions.Public,
                    value: Time.time.ToString()) // 생성된 시간
               },
            //    {
            //        "region",new DataObject(
            //         visibility: DataObject.VisibilityOptions.Public,
            //         // value: PlayerDataKeeper.selectedRegion, //game region
            //         index: DataObject.IndexOptions.S2) //indexed
            //    },
        };

        //Create (and join) new lobby
        dataControl.currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxLobbyPlayerCount, options);

        //Subscribe to lobby updates
        dataControl.InitializeLobbyCallbacks();

        isOfflineMode = false;



        //Send callback
        onSuccess?.Invoke();
        Debug.Log("Lobby created. Access code: " + dataControl.currentLobby.LobbyCode);
        Debug.Log("Status: " + gameLaunchStatus);

        LobbyHeartBeat(dataControl.currentLobby);

    }

    private async void LobbyHeartBeat(Lobby lobby)
    {
        while (true)
        {
            if (lobby == null)
            {
                return;
            }
            await LobbyService.Instance.SendHeartbeatPingAsync(lobby.Id);
            Debug.Log("로비에 심장박동중");
            await Task.Delay(15 * 1000);
        }
    }

    public async void JoinLobbyWithAccessCode(string code, Action onSuccess = null, Action<string> onFail = null)
    {
        try
        {
            //Find and join lobby by access code
            dataControl.currentLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(code);

            //Subscribe to lobby updates
            dataControl.InitializeLobbyCallbacks();

            //Send callback
            onSuccess?.Invoke();

        }
        catch (Exception exception)
        {
            //Send callback with error info
            onFail?.Invoke(exception.Message);
        }

        isOfflineMode = false;
    }

    public void StartSinglePlayer()
    {
        isOfflineMode = true;

        //Load game scene
        // TODO : 추후 수정
        // SceneLoadManager.Instance.LoadRegularScene(PlayerDataKeeper.selectedScene);
    }

    private void FixedUpdate()
    {
        //Wait until lobby becomes full and ready to start game
        // if (isLobbyOwner == true && players.Count == maxPlayers)
        // {
        //     activityControl.StartNewGame();
        // }
    }

    public async void QuitLobby()
    {
        if (isLobbyAvailable == false)
            return;

        string playerId = AuthenticationService.Instance.PlayerId;
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(dataControl.currentLobby.Id, playerId);
        }
        catch
        {
            Debug.Log("Unable to remove player. It seems it's already removed.");
        }

        dataControl.currentLobby = null;
        activityControl.ResetGameLaunchStatus();

        Debug.Log("Quit lobby");
    }

    private void OnLobbyNoLongerExistsError()
    {
        OnErrorOccurred?.Invoke("Lobby is no longer exists.");
        QuitLobby();
        Debug.Log("이 땐가?");
        //Custom logic
    }

    private void OnLocalPlayerRemovedError()
    {
        OnErrorOccurred?.Invoke("Local player was removed.");
        Debug.Log("이 땐가? 1 ");
        QuitLobby();
        //Custom logic
    }

    private void OnRelayAllocationError()
    {
        OnErrorOccurred?.Invoke("Error on Relay service.");
        Debug.Log("이 땐가? 2 ");
        QuitLobby();
        //Custom logic
    }

    private void OnTransportEvent(NetworkEvent eventType, ulong clientId, ArraySegment<byte> payload, float receiveTime)
    {
        // TODO : 추후 주석 해제
        // && GameManager.Instance == null)
        if (eventType == NetworkEvent.Disconnect)
        {
            OnErrorOccurred?.Invoke("Failed to connect to server.");
            QuitLobby();
        }
    }

    // 로비를 만든다.
    // 로비가 완성이 되면 그에 대한 처리를 한다.
    // 그 후 릴레이 서버 열어서 호스트가 되어서 로비 씬씬에 입장한다.
    public async void StartAsHost(LobbyParameters lobbyParameters, string dungeonName)
    {
        // 방 생성 및 참가
        StartCoroutine(SceneLoadManager.Instance.FadeOut(1f));
        await CreateLobby(lobbyParameters, dungeonName);

        await StartGameAsHost(maxPlayerCount);
        // await gameHostingControl.StartGameAsHost(maxPlayerCount);
    }

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
            OnRelayAllocationErrorOccurred?.Invoke();
            return;
        }

        // 접속콜백 테스트용 코드
        // NetworkManager.Singleton.OnClientConnectedCallback += TestFunction;
        //Start game session
        // 시간 체크
        NetworkManager.Singleton.StartHost();
        // SceneLoadManager.Instance.SubscribeOnNetworkSceneEvents();


        //Load game scene
        // TODO : 추후 SceneLoadManager 개편 후 수정
        // StartCoroutine(StartHostLateCoroutine(joinCode));
        SceneLoadManager.Instance.LoadNetworkScene(SceneLoadManager.Instance.Scene_Lobby);

        //Make sure host is on game scene before we allow other users to connect
        int delayBeforePublishingJoinCode = 2000;
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

    private IEnumerator StartHostLateCoroutine(string joinCode)
    {
        yield return SceneLoadManager.Instance.FadeOut(1f);
        SceneLoadManager.Instance.LoadNetworkScene(SceneLoadManager.Instance.Scene_Lobby);

    }

    public async void JoinAsClient(string lobbyId)
    {
        dataControl.currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, new JoinLobbyByIdOptions { Player = player });
        if (dataControl.currentLobby.Players.Count > 4)
        {
            return;
        }
        dataControl.InitializeLobbyCallbacks();

        // gameHostingControl.JoinHostedGameAsClient(dataControl.currentLobby);
        // 로직을 GameNetworkmanager에 몰아주고 있음.
        JoinHostedGameAsClient(dataControl.currentLobby);
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
            OnRelayAllocationErrorOccurred?.Invoke();
            return;
        }

        // StartClientLate();
        StartCoroutine(StartClientLateCoroutine());
        // NetworkManager.Singleton.StartClient();

        //Join game session as client

        // 씬이 수동으로 로드되지 않는다.
        // SceneLoadManager.Instance.SubscribeOnNetworkSceneEvents();
        // 서버가 자동으로 클라이언트에게 씬 동기화 요청을 보낸다.

    }

    private IEnumerator StartClientLateCoroutine()
    {
        // 1초 동안 페이드 아웃
        yield return SceneLoadManager.Instance.FadeOut(1f);
        // 뒤늦게 클라이언트 접속하는 건 접속하는 순간 네트워크 씬 로드가 자동으로 이루어지기 때문.
        // 씬 로드를 페이드 아웃을 기다렸다가 하겠다는 뜻.
        NetworkManager.Singleton.StartClient();
    }


    // 로비에서 현재 던전을 확인한 후 다같이 네트워크 로딩 시작! 
    public void EnterDungeon()
    {
        string dungeonSceneName = dataControl.currentLobby.Data["dungeon"].Value;
        SceneLoadManager.Instance.LoadNetworkScene(dungeonSceneName);
    }

    public async void QuickJoin(GameObject quickJoinPopUp)
    {
        IsSearchingLobby = true;

        while (Application.isPlaying && IsSearchingLobby)
        {
            try
            {
                dataControl.currentLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

                //Subscribe to lobby updates
                dataControl.InitializeLobbyCallbacks();

                gameHostingControl.JoinHostedGameAsClient(dataControl.currentLobby);

                Debug.Log("Successfully connected.");
                Debug.Log("Status: " + gameLaunchStatus);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log($"Quick join failed: {e.Reason}");
                Debug.Log($"Quick join failed: {e.GetType()}");
                await Task.Delay(1000);
            }
        }
    }

    public void CancleQuickJoin()
    {
        IsSearchingLobby = false;
    }

    public void ExitLobby()
    {

    }

    public async Task RemoveLobby()
    {
        await LobbyService.Instance.DeleteLobbyAsync(dataControl.currentLobby.Id);
    }

    public string GetLobbyCode()
    {
        return dataControl.currentLobby.LobbyCode;
    }
}
