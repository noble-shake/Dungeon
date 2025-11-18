using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using Unity.Behavior;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;


// 클라이언트의 캐릭터가 AI를 공격할 땐 어떻게 흘러가는가?

// 반대의 상황에서는 어떻게 흘러가는가?

// 


public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] GameObject[] characterList;

    private int[] dx = new int[4] { -1, -1, 1, 1 };
    private int[] dz = new int[4] { -1, 1, -1, 1 };

    [SerializeField] private AIBossCharacterManager currentBoss;

    public Transform[] spawnPointList;
    public GameObject cursePrefab;
    public GameObject curseExplosionPrefab;
    [SerializeField] private Transform bossRoomCenter;
    [SerializeField] private List<Transform> bossRoomMagicCirclePositions;
    [SerializeField] private List<Transform> bossRoomPrisonViewPositions;

    [HideInInspector] public CameraChangeWhenDead cameraChange;

    private ReactiveProperty<GameState> currentGameState = new ReactiveProperty<GameState>(GameState.Idle);

    public List<PlayerManager> connectedPlayerList = new();




    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            cameraChange = GetComponent<CameraChangeWhenDead>();
            NetworkManager.Singleton.OnClientDisconnectCallback += RemovePlayer;
        }
        else
        {
            Destroy(this);
        }
    }

    // 이상하게 이 함수가 안 먹힘; 
    private void RemovePlayer(ulong clientId)
    {
        Debug.Log("여기까진 왔다 1");
        if (IsHost)
        {
            Debug.Log("여기까진 왔다 2");
            for (int i = connectedPlayerList.Count - 1; i >= 0; i--)
            {
                if (connectedPlayerList[i].OwnerClientId == clientId)
                {
                    Debug.Log("여기까진 왔다 3");
                    connectedPlayerList.RemoveAt(i);
                }
            }
        }
    }




    // Start is called before the first frame update
    private async void Start()
    {
        await WaitForBeingSpawned();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        InputManager.instance.gameManager = this;
        SpawnCharacterServerRpc(NetworkManager.Singleton.LocalClientId);

        if (IsHost)
        {
            CheckAllPlayerDead().Forget();
        }
        else
        {
        }
    }

    private async UniTaskVoid CheckAllPlayerDead()
    {
        bool gameEnd = false;
        while (true)
        {
            await UniTask.WaitForSeconds(1);
            gameEnd = true;
            // 모든 플레이어가 죽었는지 체크한다.
            foreach (var networkObject in NetworkManager.Singleton.SpawnManager.SpawnedObjects)
            {
                PlayerManager player = networkObject.Value.GetComponent<PlayerManager>();
                if (player != null && player.isDead.Value == false)
                {
                    // Debug.Log("아직 살아있는 플레이어가 있습니다.");
                    gameEnd = false;
                    break;
                }
            }


            if (gameEnd)
            {
                // Debug.Log("모든 플레이어가 죽었습니다.");
                currentBoss.GetComponent<NetworkObject>().Despawn(true);
                DOVirtual.DelayedCall(1f, () => SendMessageServerRpc("이 노력이 의미 있을지는 아무도 모른다."));
                DOVirtual.DelayedCall(7f, () => SceneLoadManager.Instance.LoadNetworkScene(SceneLoadManager.Instance.Scene_Lobby));
                return;
            }

        }
    }

    // 플레이어가 

    [ServerRpc(RequireOwnership = false)]
    private void SpawnCharacterServerRpc(ulong clientID)
    {
        int characterClass = -1;
        // 해당 클라이언트의 클래스대로 캐릭터를 생성하고 소유권을 준다.
        foreach (var localClient in DataBinder.instance.GetCopiedPlayerClassList())
        {
            if (localClient.Key == clientID)
            {
                characterClass = localClient.Value;
                break;
            }
        }
        if (characterClass == -1)
        {
            Debug.LogError("데이터 바인더에 클래스 데이터가 없습니다.");
            characterClass = 0;
        }
        Vector3 positionToSpawn = GetSpawnPositionOnStart(clientID);
        GameObject characterToSpawn = Instantiate(characterList[characterClass], positionToSpawn, spawnPointList[clientID].rotation);
        characterToSpawn.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID);
        connectedPlayerList.Add(characterToSpawn.GetComponent<PlayerManager>());

        // 클라이언트 rpc 호출.
        // 클라이언트에게 지금까지 접속하고 스폰된 캐릭터들의 networkobject id를 넘겨준다.
        // 클라이언트는 이걸 받은 뒤 현재의 UI 리스트와 비교 후 UI를 업데이트 해준다.
        ulong[] ulongList = new ulong[connectedPlayerList.Count];
        for (int i = 0; i < connectedPlayerList.Count; i++)
        {
            ulongList[i] = connectedPlayerList[i].NetworkObjectId;
        }

        // 우선 게임 자체는 중간 참여가 불가능하게 하자고.
        // 어차피 네트워크 밸류가 있으니까 clientID에 맞춰서 순서대로 할당해주면 되지 않을까 싶음. 
        // 누구 나가면 하나 지워주고? 로비 때랑 마찬가지로 하면 되지 않을까? 
        // 그럼 내가 현재 쓸 수 있는 데이터는?
        // 클라이언트 ID 딕셔너리 ? 
        // 그럼 본인 제외하고 걍 로컬이 편한대로 ID 빠른 순서대로 

        // 그럼 어느 쪽에서 제어해줘야 하지? 로비 로직을 살펴보자. 

        Debug.Log("캐릭터 스폰 및 할당이 완료되었습니다.");
    }



    // 스폰 위치는 항상 spanwPoint를 기준으로 네 방향. 
    private Vector3 GetSpawnPositionOnStart(ulong clientID)
    {
        int order = DataBinder.instance.GetOrderOfPlayer(clientID);
        return spawnPointList[order].position;
    }

    private Vector3 GetPositionInMiddle()
    {
        return Vector3.zero;
    }

    private async UniTask WaitForBeingSpawned()
    {
        await UniTask.WaitUntil(() => IsSpawned);
    }

    [ServerRpc]
    public void SendMessageServerRpc(string message)
    {
        SendMessageClientRpc(message);
    }

    [ClientRpc]
    private void SendMessageClientRpc(string message)
    {
        HUD_UIManager.instance.popUpUIManager.SendRpcMessagePopUp(message);
    }

    // 봉인된 애는 제외
    public PlayerManager GetRandomPlayer(int number = 1)
    {
        List<NetworkObject> randomPlayerList = new();
        foreach (var networkobject in NetworkManager.Singleton.SpawnManager.SpawnedObjects)
        {
            PlayerManager player = networkobject.Value.GetComponent<PlayerManager>();
            if (player != null && !player.isDead.Value)
            {
                randomPlayerList.Add(networkobject.Value);
            }
        }
        int randomIndex = Random.Range(0, randomPlayerList.Count);
        return randomPlayerList[randomIndex].GetComponent<PlayerManager>();
    }

    public PlayerManager GetRandomPlayer(ulong sealedPlayerObjectID)
    {
        List<NetworkObject> randomPlayerList = new();
        foreach (var networkobject in NetworkManager.Singleton.SpawnManager.SpawnedObjects)
        {
            if (networkobject.Value.NetworkObjectId == sealedPlayerObjectID)
            {
                continue;
            }
            PlayerManager player = networkobject.Value.GetComponent<PlayerManager>();
            if (player != null && !player.isDead.Value)
            {
                randomPlayerList.Add(networkobject.Value);
            }
        }
        int randomIndex = Random.Range(0, randomPlayerList.Count);
        return randomPlayerList[randomIndex].GetComponent<PlayerManager>();
    }

    public void SetCurseOnAll()
    {
        SetCurseOnAllServerRpc();
    }
    [ServerRpc]
    private void SetCurseOnAllServerRpc()
    {
        SetCurseOnAllClientRpc();

    }
    [ClientRpc]
    private void SetCurseOnAllClientRpc()
    {
        foreach (var networkobject in NetworkManager.Singleton.SpawnManager.SpawnedObjects)
        {
            if (networkobject.Value.GetComponent<PlayerManager>() != null)
            {
                GameObject curseObject = ObjectPoolManager.Singleton.GetObject(cursePrefab);
                curseObject.transform.SetParent(networkobject.Value.transform);
                curseObject.transform.localPosition = new Vector3(0, 2.3f, 0);
            }
        }
    }

    public void DispellCurseExceptForTarget(ulong networkObjectId)
    {
        DispellCurseExceptForTargetServerRpc(networkObjectId);
    }

    [ServerRpc]
    private void DispellCurseExceptForTargetServerRpc(ulong networkObjectId)
    {
        DispellCurseExceptForTargetClientRpc(networkObjectId);
    }

    [ClientRpc]
    private void DispellCurseExceptForTargetClientRpc(ulong networkObjectId)
    {
        foreach (var networkobject in NetworkManager.Singleton.SpawnManager.SpawnedObjects)
        {
            if (networkobject.Value.GetComponent<PlayerManager>() != null && networkobject.Value.NetworkObjectId != networkObjectId)
            {
                GameObject curse = networkobject.Value.transform.GetComponentInChildren<ParticleSystem>().gameObject;
                ObjectPoolManager.Singleton.ReturnObject(curse, cursePrefab);
                // 여기서 추가로 폭발 이펙트 추가해야 됨. 
                GameObject curseExplosion = ObjectPoolManager.Singleton.GetObject(curseExplosionPrefab);
                curseExplosion.transform.SetParent(networkobject.Value.transform);
                curseExplosion.transform.localPosition = new Vector3(0, 2.3f, 0);
                curseExplosion.GetComponent<ParticleSystem>().Play();
            }
        }
    }

    public void SetCurrentBoss(AIBossCharacterManager boss)
    {
        currentBoss = boss;
        // currentBoss.
    }

    public Vector3 GetCenterOfBossRoom()
    {
        return bossRoomCenter.position;
    }

    public List<Transform> GetMagicCircleOfBossRoom()
    {
        return bossRoomMagicCirclePositions;
    }

    public void SetLimitOnMaxHpPenalty(float limit)
    {
        LimitMaxHpServerRpc(limit);
    }

    [ServerRpc]
    private void LimitMaxHpServerRpc(float limit)
    {
        LimitMaxHpClientRpc(limit);
    }

    [ClientRpc]
    private void LimitMaxHpClientRpc(float limit)
    {
        PlayerManager player = InputManager.instance.player;
        player.playerNetworkManager.maxHp.Value = (int)(limit * player.playerNetworkManager.maxHp.Value);
    }

    public List<Transform> GetPrisonViewerOfBossRoom()
    {
        return bossRoomPrisonViewPositions;
    }

    // 호스트에 의해서만 실행 되어야 함. 
    // 애초에 비해이비어 그래프에서 실행되므로 호스트에 의해서만 실행 되겠지만.
    public void GoToLobbyScene()
    {
        if (!IsHost) return;
        SceneLoadManager.Instance.LoadNetworkScene(SceneLoadManager.Instance.Scene_Lobby);
    }

    public void GameEnd()
    {
        //currentBoss.GetComponent<NetworkObject>().Despawn(true);
        DOVirtual.DelayedCall(5f, () => currentBoss.GetComponent<NetworkObject>().Despawn(true));
        DOVirtual.DelayedCall(2f, () => SendMessageServerRpc("이 노력이 의미 있을지는 아무도 모른다."));
        DOVirtual.DelayedCall(7f, () => SceneLoadManager.Instance.LoadNetworkScene(SceneLoadManager.Instance.Scene_Lobby));
    }

    // TODO : 현재 보스에 사정거리 관계 없이 또는 락온 여부 관계없이 스킬이 명중되는데 나중가면 이 점을 좀 고려해봐야 할 듯.
    // 현재는 아래 두 함수 안 씀. 직접 궁극기 스킬에서 호출해줌.
    public void StopBossAnimation()
    {
        currentBoss.bossAnimatorManager.StopAttackAnimationServerRpc();
    }

    public void SetBossAggro(PlayerManager player, float interceptAggroTime)
    {
        currentBoss.GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("FixedTarget", player);
        currentBoss.bossCombatManager.SetAggroFixedToplayer(player, interceptAggroTime).Forget();
    }

    public void InitializeBoss()
    {
        currentBoss.bossCombatManager.SetHpAndGroggyGaugeByPlayerCount(connectedPlayerList.Count);
    }
}
