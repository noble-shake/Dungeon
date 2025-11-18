using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

// 로비 씬에서 캐릭터 선택 및 UI 세팅(방장인지 아닌지에 따라 시작 버튼 비활성화)

public class LobbySettingManager : NetworkBehaviour
{
    public static LobbySettingManager instance;
    // public Dictionary<int, ulong> ClassPerPlayerDict { get => classPerPlayerDict; private set => classPerPlayerDict = value; }

    [SerializeField] private LobbyCharacterContoller lobbyCharacterContoller;
    [SerializeField] private GameObject connectedPlayerUIPanel;
    [SerializeField] private GameObject connectedPlayerUIPrefab;


    [SerializeField] private GameObject characterInfoUIPanel;
    [SerializeField] private GameObject classSelectUIPanel;
    [SerializeField] private GameObject startButtonUIPanel;
    [SerializeField] private GameObject backButtonUIPanel;
    [SerializeField] private GameObject chattingUIPanel;
    [SerializeField] private GameObject readyButtonUIPanel;
    [SerializeField] private GameObject LobbyExpiredPopUpUIPanel;
    [SerializeField] private TextMeshProUGUI lobbyCode;
    private bool[] positionCheckArray = new bool[4];
    [SerializeField] private LobbyAudioManager lobbyManager;


    private void Awake()
    {
        // NOTE : 아래 코드는 높은 확률로 실행이 안 된다. 그 이유를 나중을 위해 적어보자면...
        // NOTE : 지금 릴레이 서버에 접속한 상황이지만 네트워크 스폰이 안 됐기에 IsClient가 작동이 안 됨. IsHost, IsServer도 마찬가지임.
        // NOTE : 따라서 IsSpawned로 확인한 후의 과정을 거쳐야 함. 
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        // 네트워크 오브젝트는 씬 상에서 서로 동기화 되어야 하는 것들인데
        // 싱글톤 형식이면서 씬이 변경되어도 유지되어야 하는 것들이 사실 없기 때문.
        // 아래 함수는 네트워크 오브젝트와 함께 쓰기 곤란하다.
        // DontDestroyOnLoad(this);
        InputManager.instance.lobbySettingManager = instance;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // 자기 이름으로 하나 띄워놓는다. 그리고 접속 리스트에 자기를 집어넣는다.
        if (IsHost)
        {
            // playerDict.Add(NetworkManager.Singleton.LocalClientId, GameNetworkManager.Instance.player.Data["name"].Value);

            // 지금 비어있는 자리에 
            for (int i = 0; i < 4; i++)
            {
                if (positionCheckArray[i] == false)
                {
                    positionCheckArray[i] = true;
                    DataBinder.instance.AddConnectedPlayer(NetworkManager.Singleton.LocalClientId, "Test Player 1", i);
                    break;
                }
            }
            // DataBinder.instance.AddConnectedPlayer(NetworkManager.Singleton.LocalClientId, GameNetworkManager.Instance.player.Data["name"].Value);


            // 플레이어 목록에 UI를  생성한다.
            // GameObject connectedPlayerInfoUI = Instantiate(connectedPlayerUIPrefab, connectedPlayerUIPanel.transform);
            // connectedPlayerInfoUI.GetComponent<PlayerInfoUI>().clientID = 0;


            // connectedPlayerInfoUI.GetComponentInChildren<TextMeshProUGUI>().text = "Test Player 1";

            // connectedPlayerInfoUI.GetComponentInChildren<TextMeshProUGUI>().text = GameNetworkManager.Instance.player.Data["name"].Value;
        }
        else // 서버에 현재 목록을 전파하라고 요청한다. 전파된 요청을 받고 UI 객체를 생성한다. 
        {
            // string playerName = GameNetworkManager.Instance.player.Data["name"].Value;
            string playerName = "Test Player " + NetworkManager.Singleton.LocalClientId;
            UpdateConnectedPlayerServerRpc(NetworkManager.Singleton.LocalClientId, playerName);

            startButtonUIPanel.SetActive(false);
            // readyButtonUIPanel.SetActive(true);
        }

        // NetworkManager.Singleton.OnClientDisconnectCallback += UpdateDisconnectedPlayer;

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;

        lobbyCode.text = "Lobby Code : " + GameNetworkManager.Instance.GetLobbyCode();

        // 자기 자리 찾기 -> 자기가 지금 접속한 사람 중에 몇 등인지를 체크.
        // 접속한 순서를 따라 로비 캐릭터 자리 4개 중 어느 것이 내 자리인지 체크 후 캐릭터를 활성화한다.
        GenerateLobbyCharacterServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    // 우선 접속 순서를 맞추기 위해 클라이언트 번호만 기록한다.
    private void OnClientConnectedCallback(ulong clientID)
    {
        if (IsHost)
            DataBinder.instance.AddConnectedPlayer(clientID);

    }

    // 호스트가 연결을 끊었을 때
    // 호스트 
    // 곧바로 부팅 씬으로 나간다. -> 이거는 버튼 차원에서 선행 됨.
    // 클라이언트 
    // 방이 폭파되었다는 팝업창이 뜨며 확인을 누르면 부팅씬으로 나가게 된다.
    // 클라이언트가 연결을 끊었을 때
    // 호스트  
    // 해당 클라이언트의 로비 캐릭터를 비활성화하는 RPC를 호출한다.
    // 채팅창에 해당 클라이언트의 닉네임을 표시해주며 접속이 끊어졌다고 알린다.
    // 
    // 위 두 사례를 다르게 처리해야 한다.
    private void OnClientDisconnectCallback(ulong clientID)
    {
        Debug.Log($"클라이언트 연결 끊김 {clientID}");
        if (clientID == NetworkManager.ServerClientId)
        {
            if (!IsHost)
            {

                // 호스트가 나갔을 때
                Debug.Log("호스트가 나갔습니다.");
                // 다른 UI들 죄다 비활성화
                characterInfoUIPanel.SetActive(false);
                classSelectUIPanel.SetActive(false);
                startButtonUIPanel.SetActive(false);
                backButtonUIPanel.SetActive(false);
                chattingUIPanel.SetActive(false);

                // 호스트가 팀을 해체했다는 팝업창 띄우기.
                LobbyExpiredPopUpUIPanel.SetActive(true);
            }
            // SceneLoadManager.Instance.LoadRegularScene("Scene_Booting");
        }
        else
        {
            // 클라이언트가 나갔을 때
            if (IsHost)
            {

                // 해당 클라이언트가 나갔다는 메세지 송출 RPC 실행

                // 해당 클라이언트의 캐릭터를 비활성화해주는 RPC 실행
                lobbyCharacterContoller.RemoveLobbyCharacter(clientID);
                // 현재 접속한 리스트에서 clientID 삭제제
                DataBinder.instance.RemoveConnectedPlayer(clientID);
                if (GameManager.Instance != null)
                {
                    if (GameManager.Instance.connectedPlayerList.Count > 0)
                    {
                        Debug.Log("여기까진 왔다 2");
                        for (int i = GameManager.Instance.connectedPlayerList.Count - 1; i >= 0; i--)
                        {
                            if (GameManager.Instance.connectedPlayerList[i].OwnerClientId == clientID)
                            {
                                Debug.Log("여기까진 왔다 3");
                                GameManager.Instance.connectedPlayerList.RemoveAt(i);
                            }
                        }
                    }
                }
            }
            // 
            // UpdateDisconnectedPlayer(clientID);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void GenerateLobbyCharacterServerRpc(ulong clientID)
    {
        int spawnIndex = DataBinder.instance.GetOrderOfPlayer(clientID);
        lobbyCharacterContoller.GenerateLobbyCharacter(clientID, spawnIndex);
    }



    #region 플레이어 연결이 끊어질 때의 콜백 함수

    private void UpdateDisconnectedPlayer(ulong clientID)
    {
        if (IsHost)
            UpdateDisconnectedPlayerClientRpc(clientID);
    }

    [ClientRpc]
    private void UpdateDisconnectedPlayerClientRpc(ulong clientID)
    {
        foreach (Transform item in connectedPlayerUIPanel.transform)
        {
            if (item.GetComponent<PlayerInfoUI>().clientID == clientID)
            {
                Destroy(item);
                break;
            }
        }
    }
    #endregion

    #region 플레이어 연결이 이어질 때의 콜백 함수
    [ServerRpc(RequireOwnership = false)] // 서버에 본인의 이름을 전파하고 UI 목록을 받아온다.
    private void UpdateConnectedPlayerServerRpc(ulong clientID, string playerName)
    {
        // 서버에 접속 기록을 남김.
        for (int i = 0; i < 4; i++)
        {
            if (positionCheckArray[i] == false)
            {
                positionCheckArray[i] = true;
                DataBinder.instance.AddConnectedPlayer(clientID, playerName, i);
                break;
            }
        }

        // 클라이언트한테 현재 있는 Player List를 싹 다 날리라고 명령한다.
        // ClearConnectedPlayerInfoUIListClientRpc(clientID);
        foreach (var playerData in DataBinder.instance.GetCopiedPlayerList())
        {
            // UpdateConnectedPlayerClientRpc(playerData.clientID, playerData.playerName, playerData.characterClass);

            lobbyCharacterContoller.EnableCharacter(playerData.clientID, (PlayerClass)playerData.characterClass);
        }

        // GameObject connectedPlayerInfoUI = Instantiate(connectedPlayerUIPrefab, connectedPlayerUIPanel.transform);
        // connectedPlayerInfoUI.GetComponent<TextMeshProUGUI>().text = playerName;
        // connectedPlayerInfoList.Add(clientID, connectedPlayerInfoUI);
    }

    [ClientRpc]
    private void ClearConnectedPlayerInfoUIListClientRpc(ulong receiverID)
    {
        // if (NetworkManager.Singleton.LocalClientId != receiverID)
        //     return;
        foreach (Transform child in connectedPlayerUIPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    [ClientRpc]
    private void UpdateConnectedPlayerClientRpc(ulong clientID, string playerName, int characterClass)
    {
        Debug.Log("이미지 값 " + characterClass);
        GameObject connectedPlayerInfoUI = Instantiate(connectedPlayerUIPrefab, connectedPlayerUIPanel.transform);
        connectedPlayerInfoUI.GetComponent<PlayerInfoUI>().clientID = clientID;
        connectedPlayerInfoUI.GetComponentInChildren<TextMeshProUGUI>().text = playerName;

        if (characterClass == -1)
        {
            characterClass = (int)PlayerClass.Selecting;
        }
        Debug.Log(characterClass);
        connectedPlayerInfoUI.GetComponent<PlayerInfoUI>().SetIconPerClass((PlayerClass)characterClass);
    }
    #endregion

    // 서버에 이 캐릭터가 해당 클래스를 선택했음을 알리고 선택이 가능하다면 서버에서 그에 맞는 응답을 줘야 한다. 
    // 캐릭터 선택이 유효하다면 서버에 캐릭터를 스폰하라고 말해줘야 함.
    public void SelectCharacterClass(PlayerClass characterClass, bool selected)
    {
        if (selected == true)
        {
            RespondToClassSelectUpdateServerRpc(NetworkManager.Singleton.LocalClientId, (int)characterClass);

            // SpawnSittingIdleCharacterServerRpc(NetworkManager.Singleton.LocalClientId, spawnIndex, (int)characterClass);
        }
        else
        {
            RespondToClassUnselectUpdateServerRpc(NetworkManager.Singleton.LocalClientId, (int)characterClass);
        }
        // characterSelectSlotUI.GetReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RespondToClassSelectUpdateServerRpc(ulong clientID, int characterClassIndex)
    {
        // 아직 해당 클래스를 사용하는 사람이 없다면
        // NOTE : 테스트용으로 캐릭 중복 허용
        // if (!DataBinder.instance.IfClassExistsInPlayers(characterClassIndex))
        {
            DataBinder.instance.RespondToClassSelect(clientID, characterClassIndex);

            // 얘도 처리해야 함. 
            lobbyCharacterContoller.EnableCharacter(clientID, (PlayerClass)characterClassIndex);
            // lobbyCharacterContoller.SetEnable();

            UpdatePlayerInfoUIClientRpc(clientID, characterClassIndex, true);
        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void RespondToClassUnselectUpdateServerRpc(ulong clientID, int characterClassIndex)
    {
        // 해당 클래스를 사용하는 사람이 있다면
        if (DataBinder.instance.IfClassExistsInPlayers(characterClassIndex))
        {
            DataBinder.instance.RevertClassSelected(clientID);

            lobbyCharacterContoller.DisableCharacter(clientID);

            UpdatePlayerInfoUIClientRpc(clientID, characterClassIndex, false);
        }
        else // 없다면
        {
        }
    }

    [ClientRpc] // 해당 클라이언트의 선택된 클래스를 수정한다. insert 값에 따라 빈 칸으로 표시할지, 클래스를 확정지을지 결정남.
    private void UpdatePlayerInfoUIClientRpc(ulong clientID, int characterClassIndex, bool insert)
    {
        foreach (Transform uiObject in connectedPlayerUIPanel.transform)
        {
            if (uiObject.GetComponent<PlayerInfoUI>().clientID == clientID)
            {
                if (insert)
                {
                    uiObject.GetComponent<PlayerInfoUI>().SetIconPerClass((PlayerClass)characterClassIndex);
                    break;
                }
                else
                {
                    uiObject.GetComponent<PlayerInfoUI>().SetIconPerClass(PlayerClass.Selecting);
                    break;
                }
            }
        }
    }



    public void LeftNext()
    {
        throw new NotImplementedException();
    }

    public void rightNext()
    {
        throw new NotImplementedException();
    }


    public void OnLobbyOffMusic()
    {
        LobbyMusicOffServerRpc();
    }

    [ServerRpc]
    public void LobbyMusicOffServerRpc()
    {
        LobbyMusicOffClientRpc();
    }

    [ClientRpc]
    public void LobbyMusicOffClientRpc()
    {
        // lobbyManager.OffLobbyMusic();
    }

    [ServerRpc]
    public void FadeOutServerRpc(float time)
    {
        FadeOutClientRpc(time);
    }

    [ClientRpc]
    private void FadeOutClientRpc(float time)
    {
        StartCoroutine(SceneLoadManager.Instance.FadeOut(time));
    }
}
