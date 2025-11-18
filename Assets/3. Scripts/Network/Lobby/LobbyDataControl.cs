using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System;
using System.Threading.Tasks;

public class LobbyDataControl : MonoBehaviour
{
    public Lobby currentLobby { get; set; }

    public event Action<string, Dictionary<string, string>> OnPlayerDataChanged;
    public event Action<Dictionary<string, string>> OnLobbyUpdated;

    public event Action<Player> OnPlayerJoined;
    public event Action<int> OnPlayerLeft;

    public event Action OnLobbyNoLongerExistsError;
    public event Action OnLocalPlayerRemovedError;

    private LobbyEventCallbacks lobbyEventCallbacks;

    public List<Player> players
    {
        get
        {
            if (isLobbyAvailable == false)
                return new List<Player>();

            return currentLobby.Players;
        }
    }

    public Player localPlayer
    {
        get
        {
            if (isLobbyAvailable == false) return null;

            return players?.Find(x => x.Id == AuthenticationService.Instance.PlayerId);
        }
    }



    public int maxPlayers
    {
        get
        {
            if (isLobbyAvailable == false)
                return -1;

            return currentLobby.MaxPlayers;
        }
    }

    public bool isLobbyAvailable => currentLobby != null;

    public bool isLobbyOwner
    {
        get
        {
            if (isLobbyAvailable == false)
                return false;

            return currentLobby.HostId == AuthenticationService.Instance.PlayerId;
        }
    }


    public LobbyDataControl()
    {
        GeneratePlayerObject();

        //Authenticate
        // ProcessInitialization();

        //Log player data updates
        // OnPlayerDataChanged += (id, changes) =>
        // {
        //     foreach (var parameter in changes)
        //     {
        //         Debug.Log($"Player: {id} IsLocalPlayer: {AuthenticationService.Instance.PlayerId == id} Parameter: {parameter.Key} Value: {parameter.Value}");
        //     }
        // };

        // //Log lobby data updates
        // OnLobbyUpdated += (changes) =>
        // {
        //     foreach (var parameter in changes)
        //     {
        //         Debug.Log($"Lobby updated. Parameter: {parameter.Key} Value: {parameter.Value}");
        //     }
        // };

        // RunLobbyAutoRefreshLoop();
    }

    private async void ProcessInitialization()
    {
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
            await UnityServices.InitializeAsync();

        if (AuthenticationService.Instance.IsSignedIn == false)
        {
            //Uncomment code below if you are using ParrelSync
            //#if UNITY_EDITOR

            //if (Application.isEditor && ParrelSync.ClonesManager.IsClone())
            //{
            //    string customArgument = ParrelSync.ClonesManager.GetArgument();
            //    AuthenticationService.Instance.SwitchProfile($"Clone_{customArgument}_Profile");
            //    PlayerDataKeeper.authProfileName = customArgument;
            //}
            //#endif

            // if (CommandLineHelper.TryGetArgumentValue("authProfileName", out string profileName))
            // {
            //     AuthenticationService.Instance.SwitchProfile(profileName);
            //     PlayerDataKeeper.authProfileName = profileName;
            //     Debug.Log("Profile switched: " + profileName);
            // }

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public async void InitializeLobbyCallbacks()
    {
        lobbyEventCallbacks = new LobbyEventCallbacks();

        lobbyEventCallbacks.PlayerDataAdded += (playerDataChanges) =>
        {
            HandlePlayerDataChanges(playerDataChanges);
        };

        lobbyEventCallbacks.PlayerDataChanged += (playerDataChanges) =>
        {
            HandlePlayerDataChanges(playerDataChanges);
        };

        lobbyEventCallbacks.LobbyChanged += (lobbyChanges) =>
        {
            HandleLobbyDataChanges(lobbyChanges);
        };

        lobbyEventCallbacks.PlayerJoined += (playersJoined) =>
        {
            for (int i = 0; i < playersJoined.Count; i++)
            {
                OnPlayerJoined?.Invoke(playersJoined[i].Player);
            }
        };

        lobbyEventCallbacks.PlayerLeft += (playersLeft) =>
        {
            for (int i = 0; i < playersLeft.Count; i++)
            {
                OnPlayerLeft?.Invoke(playersLeft[i]);
            }
        };

        //Subscribe to lobby updates
        // 유니티 로비 서버와 클라이언트 사이에 채널이 있음. 웹소켓 형태일 듯.
        // 이 채널을 통해 로비의 변화 알림을 받게 되는데 그 변화 알림을 처리할 핸들러를 등록하는 함수인 거임.
        await LobbyService.Instance.SubscribeToLobbyEventsAsync(currentLobby.Id, lobbyEventCallbacks);
    }

    private void HandlePlayerDataChanges(Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> changes)
    {
        string localPlayerId = AuthenticationService.Instance.PlayerId;

        foreach (var player in changes)
        {
            int playerIndex = player.Key;

            Dictionary<string, string> parameters = new Dictionary<string, string>();

            //Convert lobby response to Dictionary<string, string>
            foreach (var parameter in changes[playerIndex])
            {
                parameters.Add(parameter.Key, parameter.Value.Value.Value);
            }

            //Send player update callback (other player, not local)
            //See local player update below in UpdatePlayerParameters method
            if (players[playerIndex].Id != localPlayerId)
                OnPlayerDataChanged?.Invoke(players[playerIndex].Id, parameters);
        }
    }

    private void HandleLobbyDataChanges(ILobbyChanges lobbyChanges)
    {
        lobbyChanges.ApplyToLobby(currentLobby);

        if (lobbyChanges.Data.Value != null && isLobbyOwner == false)
        {
            Dictionary<string, string> output = new Dictionary<string, string>();

            //Convert lobby response to Dictionary<string, string>
            foreach (var parameter in lobbyChanges.Data.Value)
                output.Add(parameter.Key, parameter.Value.Value.Value);

            //Send lobby update callback
            OnLobbyUpdated?.Invoke(output);
        }
    }

    // 로비 목록에서 볼 때 어떤 클래스의 캐릭터가 있는지 표시를 해줘야 하니까, 플레이어 데이터에 타입도 있어야 한다.
    public Player GeneratePlayerObject(string playerName = null)
    {
        //Create default player
        string playerClass = "knight";
        if (playerName == null)
        {
            playerName = "Test";
        }
        Player player = new Player(id: AuthenticationService.Instance.PlayerId,
        data: new Dictionary<string, PlayerDataObject>()
        {
            { "name", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) },   // 로비 내에서 플레이어의 이름이 떠야 함. 
            { "class", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerClass) } // 로비 목록에 어떤 타입이 있는지 표시해줘야 되기 때문에 필요함.
        });

        return player;
    }

    public async void UpdatePlayerParameters(Dictionary<string, PlayerDataObject> parameters)
    {
        UpdatePlayerOptions options = new UpdatePlayerOptions();
        options.Data = parameters;

        string playerId = AuthenticationService.Instance.PlayerId;
        Player player = currentLobby.Players.Find(player => player.Id == playerId);

        currentLobby = await LobbyService.Instance.UpdatePlayerAsync(currentLobby.Id, playerId, options);

        //Convert lobby response to Dictionary<string, string>
        Dictionary<string, string> output = new Dictionary<string, string>();
        foreach (var parameter in parameters)
        {
            output.Add(parameter.Key, parameter.Value.Value);
        }

        //Send player update callback (local player)
        OnPlayerDataChanged?.Invoke(playerId, output);
    }

    public async void UpdateLobbyParameters(Dictionary<string, DataObject> lobbyParameters)
    {
        if (isLobbyOwner == false) return;

        UpdateLobbyOptions lobbyOptions = new UpdateLobbyOptions();
        lobbyOptions.Data = lobbyParameters;

        currentLobby = await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, lobbyOptions);

        //Convert lobby response to Dictionary<string, string>
        Dictionary<string, string> output = new Dictionary<string, string>();
        foreach (var parameter in lobbyParameters)
        {
            output.Add(parameter.Key, parameter.Value.Value);
        }

        //Send lobby update callback
        OnLobbyUpdated?.Invoke(output);
    }

    public async void RunLobbyAutoRefreshLoop()
    {
        while (Application.isPlaying)
        {
            await Task.Delay(1000);

            // Skip lobby updates in game mode
            // if (GameManager.Instance != null) continue;

            RefreshLobby();
        }
    }

    public async void RefreshLobby()
    {
        //Get last lobby state
        if (isLobbyAvailable == true)
        {
            try
            {
                currentLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);

                if (localPlayer == null)
                {
                    Debug.Log("Local player no longer exists in this lobby.");
                    OnLocalPlayerRemovedError?.Invoke();
                }
            }
            catch (LobbyServiceException e)
            {
                if (e.Reason == LobbyExceptionReason.LobbyNotFound)
                {
                    Debug.Log(e.Reason);
                    OnLobbyNoLongerExistsError?.Invoke();
                }
            }
        }
    }
}
