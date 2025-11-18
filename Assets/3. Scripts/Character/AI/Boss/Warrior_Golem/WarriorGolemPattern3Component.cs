using System.Collections.Generic;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using Unity.Behavior;
using System;
using System.Linq;

public class WarriorGolemPattern3Component : NetworkBehaviour
{
    [HideInInspector] private AIWarriorGolemCharacterManager characterManager;
    [HideInInspector] public NetworkVariable<bool> indicatorActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [HideInInspector] public NetworkVariable<float> indicatorFillProgress = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [HideInInspector] public NetworkVariable<int> NumbOfCircleObject = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [HideInInspector] public int NumbOfPlayer;

    [Header("Force Pull")]
    [SerializeField] public GameObject WavePrefab;
    [SerializeField] public WarriorGolemPattern3WaveIndicator IndicatorWave;
    [SerializeField] int WaveDamage;

    [Space]
    [Header("Magic Circle")]
    public bool isMagicCirclePatternPlaying;
    [SerializeField] public GameObject MagicCirclePrefab;
    [SerializeField] public GameObject CircleObjectPrefab;
    [SerializeField] public List<WarriorGolemPattern3MagicCircle> MagicCircles;
    [SerializeField] public List<WarriorGolemPattern3CircleObject> MagicCircleObjects;
    [SerializeField] public Queue<WarriorGolemPattern3CircleObject> CircleObjectQueue;
    
    [SerializeField] public List<int> MagicCircleID;
    [SerializeField] public Dictionary<int, ulong> CircleObjectOwnCheck;
    // [HideInInspector] public int NumbOfCircleObject;

    [Space]
    [HideInInspector] public Action<bool> BuffAction;
    [SerializeField] public bool isBuffActivated;
    [SerializeField] int BuffCount;

    [Space]
    [Header("Prison Wall")]
    public bool PrisonPatternPlaying;
    public bool PrisonPatternBreaked;
    [HideInInspector] public NetworkVariable<float> PatternTimer = new NetworkVariable<float>(30f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public float CurPatternTimer;
    [SerializeField] public GameObject PrisonBeginPrefab;
    [SerializeField] public GameObject PrisonCastPrefab;
    [SerializeField] public GameObject PrisonPrefab;
    [SerializeField] public GameObject PrisonChainPrefab;
    [SerializeField] public PlayerManager CurDualTarget;
    [SerializeField] int PrisonWallHP = 400;
    [SerializeField] List<WarriorGolemPattern3PrisonChain> Chainings;
    [HideInInspector] public WarriorGolemPattern3Prison PrisonObject;
    [HideInInspector] public GameObject PrisonBeginEffect;
    public List<ulong> PlayerIDList;
    public List<Vector3> playerPoses;



    [Space]
    [Header("Break")]
    [SerializeField] public bool isBreakTime;
    [HideInInspector] public NetworkVariable<int> BreakCount = new NetworkVariable<int>(4, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Start()
    {
        BuffAction += OnPattern3BuffOnPattern3Buff;
        CircleObjectOwnCheck = new Dictionary<int, ulong>();
        MagicCircles = new List<WarriorGolemPattern3MagicCircle>();
        MagicCircleObjects = new List<WarriorGolemPattern3CircleObject>();
        characterManager = GetComponent<AIWarriorGolemCharacterManager>();
        indicatorActive.OnValueChanged += OnIndicatorActive;
        indicatorFillProgress.OnValueChanged += OnIndicatorFillProgressChanged;
        PatternTimer.OnValueChanged += TimerFlow;
        // MagicCircle Instantiate.
        
    }

    private void Update()
    {
        if (PrisonPatternPlaying)
        {
            PatternTimer.Value -= Time.deltaTime;

        }
    }

    #region Force Pull
    public void OnForcePull()
    {
        if (IsOwner) PropagationNumbPlayerServerRpc();

        GameObject waver = ObjectPoolManager.Singleton.GetObject(WavePrefab);
        IndicatorWave = waver.GetComponent<WarriorGolemPattern3WaveIndicator>();
        waver.gameObject.SetActive(true);
        waver.GetComponent<WarriorGolemPattern3WaveIndicator>().Init(this.gameObject);
        waver.transform.position = GameManager.Instance.GetCenterOfBossRoom();
        StartCoroutine(ForcePullIEnumerator());
    }

    IEnumerator ForcePullIEnumerator()
    {
        List<PlayerManager> targetPlayers = new List<PlayerManager>();
        List<Vector3> targetPlayersPos = new List<Vector3>();
        foreach (ulong playerObjectID in NetworkManager.Singleton.SpawnManager.SpawnedObjects.Keys)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerObjectID].GetComponent<PlayerManager>() != null)
            {
                PlayerManager temp = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerObjectID].GetComponent<PlayerManager>();
                targetPlayers.Add(temp);
                targetPlayersPos.Add(temp.transform.position);
            }
        }

        yield return null;

        Vector3 BossPos = transform.position;
        float curTime = 0f;
        while (curTime < 1f)
        {
            if (curTime < 0.5f)
            {
                for (int idx = 0; idx < targetPlayers.Count; idx++)
                {
                    PlayerManager targetPlayer = targetPlayers[idx];
                    Vector3 targetPos = targetPlayersPos[idx];
                    if (targetPlayer.isDead.Value == false)
                    {
                        CharacterController controller = targetPlayer.GetComponent<CharacterController>();
                        controller.Move(Vector3.Slerp(targetPos, Vector3.Lerp(targetPos, BossPos + Vector3.up * 2 , 0.5f), curTime * 2f) - targetPlayer.transform.position);


                    }
                }
            }
            else
            {
                for (int idx = 0; idx < targetPlayers.Count; idx++)
                {
                    PlayerManager targetPlayer = targetPlayers[idx];
                    Vector3 targetPos = targetPlayersPos[idx];
                    if (targetPlayer.isDead.Value == false)
                    {
                        CharacterController controller = targetPlayer.GetComponent<CharacterController>();
                        controller.Move(Vector3.Slerp(Vector3.Lerp(targetPos, BossPos + Vector3.up * 2 , 0.5f), BossPos, (curTime -0.5f) * 2f) - targetPlayer.transform.position);
                    }
                }
            }

            curTime += Time.deltaTime * 2; // 1 sec
            yield return null;
        }

        if(IsOwner) GetComponent<AIBossCharacterAnimatorManager>().PlayAnimationOnNetwork(AnimationType.Attack, "ForcePullCastDone", true);
    }

    public void OnWaveIndicatorWave()
    {
        StartCoroutine(IndicatorEffect());
    }

    IEnumerator IndicatorEffect()
    {
        if(IndicatorWave.Explosion.isPlaying) IndicatorWave.Explosion.Stop();
        IndicatorWave.Aura.gameObject.SetActive(false);
        IndicatorWave.Indicator.gameObject.SetActive(false);
        IndicatorWave.Explosion.gameObject.SetActive(true);
        IndicatorWave.Explosion.Play();
        yield return null;

        while (IndicatorWave.Explosion.isPlaying)
        {
            yield return null;
        }

        IndicatorWave.Explosion.gameObject.SetActive(false);

        ObjectPoolManager.Singleton.ReturnObject(IndicatorWave.gameObject, WavePrefab);
    }

    public void OnIndicatorHit()
    {
        IndicatorWave.HitTargets();
    }

    private void OnIndicatorActive(bool previousValue, bool newValue)
    {
        if (newValue == true)
        {
            IndicatorWave.Indicator.gameObject.SetActive(true);
        }
        else
        {
            IndicatorWave.Indicator.gameObject.SetActive(false);
        }
    }

    private void OnIndicatorFillProgressChanged(float oldValue, float newValue)
    {
        if (!IsOwner)
        {
            IndicatorWave.FillProgress = newValue;
        }
    }
    #endregion

    #region Buff

    public void OnPattern3BuffOnPattern3Buff(bool isOn)
    {
        // SubGraph라 아마 안먹힐 가능성이 매웅ㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇ 높다. 그래서 변수 하나 박아둠
        // Can't Access to Dynamic SubGraph. use bool
        isBuffActivated = isOn;
        GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("OnBuff", isOn); // not work.
    }

    #endregion


    public void TimerFlow(float oldValue, float newValue)
    {
        PatternTimer.Value = newValue;

        if (PatternTimer.Value <= 0f)
        {
            PatternTimer.Value = 0f;
        }
    }

    #region  MagicCircle

    public void OnSummonMagicCircle()
    {
        isMagicCirclePatternPlaying = true;

        if (!IsOwner) return;

        

        List<int> tossList = new List<int>();

        Debug.Log($"Summon MagicCircle by NumbPlayer {NumbOfPlayer}");
        switch (NumbOfPlayer)
        {
            default:
            case 1:
            case 2:
                while (tossList.Count < 2)
                {
                    int toss = UnityEngine.Random.Range(0, 4);
                    if (tossList.Contains(toss) == false)
                    {
                        tossList.Add(toss);
                    }
                }
                for (int idx = 0; idx < 2; idx++)
                {
                    MagicCircles.Add(Instantiate(MagicCirclePrefab).GetComponent<WarriorGolemPattern3MagicCircle>());
                    MagicCircleObjects.Add(Instantiate(CircleObjectPrefab).GetComponent<WarriorGolemPattern3CircleObject>());
                }
                NumbOfCircleObject.Value = 2;
                break;
            case 3:
                while (tossList.Count < 3)
                {
                    int toss = UnityEngine.Random.Range(0, 4);
                    if (tossList.Contains(toss) == false)
                    {
                        tossList.Add(toss);
                    }
                }
                for (int idx = 0; idx < 3; idx++)
                {
                    MagicCircles.Add(Instantiate(MagicCirclePrefab).GetComponent<WarriorGolemPattern3MagicCircle>());
                    MagicCircleObjects.Add(Instantiate(CircleObjectPrefab).GetComponent<WarriorGolemPattern3CircleObject>());
                }
                NumbOfCircleObject.Value = 3;
                break;
            case 4:
                while (tossList.Count < 4)
                {
                    int toss = UnityEngine.Random.Range(0, 4);
                    if (tossList.Contains(toss) == false)
                    {
                        tossList.Add(toss);
                    }
                }
                for (int idx = 0; idx < 4; idx++)
                {
                    MagicCircles.Add(Instantiate(MagicCirclePrefab).GetComponent<WarriorGolemPattern3MagicCircle>());
                    MagicCircleObjects.Add(Instantiate(CircleObjectPrefab).GetComponent<WarriorGolemPattern3CircleObject>());
                }
                NumbOfCircleObject.Value = 4;
                break;
        }

        // Circle Position Random Toss
        // Network

        HashSet<int> tossPosHashSet = new HashSet<int>();
        while (tossPosHashSet.Count < MagicCircles.Count)
        {
            int tossVal = UnityEngine.Random.Range(0, 4);
            tossPosHashSet.Add(tossVal);
        }
        List<int> tossPosList = tossPosHashSet.ToList();
        List<Transform> circlePositions = GameManager.Instance.GetMagicCircleOfBossRoom();

        for (int idx = 0; idx < MagicCircles.Count; idx++)
        {
            int targetPos = tossList[idx];
            MagicCircles[idx].GetComponent<NetworkObject>().Spawn();
            SetBossCompServerRpc(idx);
            MagicCircles[idx].OnMagicCircleSetServerRpc(idx, targetPos, circlePositions[tossPosList[idx]].position);
            MagicCircleObjects[idx].SetObject(targetPos);
            MagicCircleObjects[idx].ownerIndex = idx;
        }

        CircleObjectQueue = new Queue<WarriorGolemPattern3CircleObject>();
        foreach (WarriorGolemPattern3CircleObject circle in MagicCircleObjects)
        {
            var circleObject = Instantiate(circle.gameObject).GetComponent<WarriorGolemPattern3CircleObject>();
            circleObject.SetObject((int)circle.CurCircleType);
            circleObject.ownerIndex = circle.ownerIndex;
            CircleObjectQueue.Enqueue(circleObject);
        }
        StartCoroutine(SummonMagicCircleEffect());
    }

    [ServerRpc(RequireOwnership = false)]
    public void DropServerRpc(Vector3 pos)
    {
        DropClientRpc(pos);
    }

    [ClientRpc]
    public void DropClientRpc(Vector3 pos)
    {
        if (IsOwner)
        {
            if (CircleObjectQueue.Count > 0)
            {
                WarriorGolemPattern3CircleObject circleObject = CircleObjectQueue.Dequeue();
                

                circleObject.GetComponent<NetworkObject>().Spawn();
                circleObject.CircleObjectPosSetServerRpc(circleObject.CircleOrbsIdx, pos);
                NumbOfCircleObject.Value = CircleObjectQueue.Count;
            }
            else
            {
                return;
            }
        }
    }

    public void ReproduceObject(int ListIndex)
    {
        var circleObject = Instantiate(MagicCircleObjects[ListIndex].gameObject).GetComponent<WarriorGolemPattern3CircleObject>();
        circleObject.SetObject((int)MagicCircleObjects[ListIndex].CurCircleType);
        circleObject.ownerIndex = MagicCircleObjects[ListIndex].ownerIndex;
        CircleObjectQueue.Enqueue(circleObject);
    }

    [ServerRpc]
    public void SetBossCompServerRpc(int idx)
    {
        SetBossCompClientRpc(idx);
    }

    [ClientRpc]
    public void SetBossCompClientRpc(int idx)
    {
        if(IsOwner)MagicCircles[idx].CircleManager = this;
    }

    IEnumerator SummonMagicCircleEffect()
    {

        if (IsOwner) GameManager.Instance.SendMessageServerRpc("나는 점점 강해진다!");

        yield return new WaitForSeconds(1.2f);

        BuffAction.Invoke(true);
        if(IsOwner) GetComponent<AIBossCharacterAnimatorManager>().PlayAnimationOnNetwork(AnimationType.Attack, "Boss1_Summon_MagicCircle_Execute", true);
    }



    public void MagicCircleClearCheck()
    {
        bool isActivatedAll = true;
        foreach (WarriorGolemPattern3MagicCircle circle in MagicCircles)
        {
            if (circle.isActivated == false) isActivatedAll = false;
        }

        if (isActivatedAll)
        {
            Debug.Log("MagicCircle Clear");
            StartCoroutine(CircleClearSeqeunce());
            isMagicCirclePatternPlaying = false;
        }
    }

    IEnumerator CircleClearSeqeunce()
    {
        yield return new WaitForSeconds(2f);

        //Variable: PatternPhase
        //GUID Parts: 4398231458999220924, 777790602256171328
        //GUID String: bcba988935a8093d40c11d8d6e44cb0a

       
        ClearAllCircles();
    }

    public void ClearAllCircles()
    {
        foreach (WarriorGolemPattern3MagicCircle circle in MagicCircles)
        {
            ObjectPoolManager.Singleton.ReturnObject(circle.gameObject, MagicCirclePrefab);
            if (IsOwner) circle.GetComponent<NetworkObject>().Despawn();
        }
        MagicCircles = new();
    }

    #endregion

    #region Prison

    [ServerRpc]
    public void SetPlayerListForPrisonOrderingServerRpc()
    {
        //PlayerIDList
        foreach (ulong playerObjectID in NetworkManager.Singleton.SpawnManager.SpawnedObjects.Keys)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerObjectID].GetComponent<PlayerManager>() != null)
            {
                PlayerIDList.Add(playerObjectID);
            }
        }

        SetPlayerListForPrisonOrderingClientRpc(PlayerIDList.ToArray());
    }

    [ClientRpc]
    public void SetPlayerListForPrisonOrderingClientRpc(ulong[] order)
    {

        PlayerIDList = order.ToList();
        playerPoses = new List<Vector3>();

        foreach (ulong playerObjectID in PlayerIDList)
        {
            playerPoses.Add(NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerObjectID].transform.position);
        }
    }

    public void OnSummonPrison()
    {
        isMagicCirclePatternPlaying = false;
        if (IsOwner)
        {
            PrisonObject = Instantiate(PrisonPrefab).GetComponent<WarriorGolemPattern3Prison>();
            PrisonObject.GetComponent<NetworkObject>().Spawn();
            PrisonSetBossCompServerRpc();

            GameManager.Instance.SendMessageServerRpc("이건 감옥 패턴이다. 할 말을 정해라!");

            Chainings = new List<WarriorGolemPattern3PrisonChain>();
            PlayerIDList = new List<ulong>();

            SetPlayerListForPrisonOrderingServerRpc();

            foreach (ulong playerObjectID in PlayerIDList)
            {

                GameObject chain = Instantiate(PrisonChainPrefab);
                chain.GetComponent<NetworkObject>().Spawn();
                chain.GetComponent<WarriorGolemPattern3PrisonChain>().GrabPlayerServerRpc(playerObjectID);
                Chainings.Add(chain.GetComponent<WarriorGolemPattern3PrisonChain>());
            }
            GravityOffServerRpc();
        }

        StartCoroutine(LockEffect());
    }



    IEnumerator LockEffect()
    {
        yield return new WaitForSeconds(1f);
        // summon chain
        List<Transform> prisonPoses = GameManager.Instance.GetPrisonViewerOfBossRoom();

        List<Vector3> playerPoses = new List<Vector3>();

        foreach (ulong playerObjectID in PlayerIDList)
        {
            playerPoses.Add(NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerObjectID].transform.position);
        }

        float curTime = 0f;
        while (curTime < 1f)
        {
            curTime += (Time.deltaTime) * 2f; // 1 sec

            int count = 0;
            for (int idx = 0; idx < PlayerIDList.Count; idx++)
            {
                PlayerManager player = NetworkManager.Singleton.SpawnManager.SpawnedObjects[PlayerIDList[idx]].GetComponent<PlayerManager>();
                CharacterController controller = player.GetComponent<CharacterController>();
                controller.Move(Vector3.Lerp(playerPoses[idx], prisonPoses[idx].position
                    + Vector3.up * 3f, curTime) - player.transform.position);

            }
            yield return null;
        }

        if (IsOwner)
        {
            for (int idx = 0; idx < PlayerIDList.Count; idx++)
            {
                Chainings[idx].ChainingServerRpc(prisonPoses[idx].position);
            }

            GetComponent<AIBossCharacterNetworkManager>().currentHp.Value = (int)(GetComponent<AIBossCharacterNetworkManager>().maxHp.Value * 0.1f);

            yield return new WaitForSeconds(2f);
            PrisonObject.PrisonTurnOnServerRpc();
            // Animation
            PrisonPatternPlaying = true;

            yield return new WaitForSeconds(2f);

            GetComponent<AIBossCharacterAnimatorManager>().PlayAnimationOnNetwork(AnimationType.Attack, "Boss1_Summon_Prison_Execute", true);
        }
    }

    public void OnDual()
    {
        // SetDualTargetServerRpc();
        if (IsOwner)
        {
            List<ulong> playerList = new List<ulong>();
            List<int> ChainingIdx = new List<int>();
            for (int idx = 0; idx < PlayerIDList.Count; idx++)
            {
                if (NetworkManager.Singleton.SpawnManager.SpawnedObjects[PlayerIDList[idx]].GetComponent<PlayerManager>() != null)
                {
                    if (NetworkManager.Singleton.SpawnManager.SpawnedObjects[PlayerIDList[idx]].GetComponent<PlayerManager>().isDead.Value == false)
                    {
                        playerList.Add(PlayerIDList[idx]);
                        ChainingIdx.Add(idx);
                    } 
                }
            }

            int tossID = UnityEngine.Random.Range(0, playerList.Count);

            CurDualTarget = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerList[tossID]].GetComponent<PlayerManager>();
            GetComponent<AIBossCharacterCombatManager>().currentTarget = CurDualTarget;

            SetDualTargetServerRpc(ChainingIdx[tossID], playerList[tossID]);
        }
    }

    IEnumerator DualEffect(int tossID)
    {
        
        Vector3 BossPos = transform.position;
        float curTime = 0f;
        Vector3 TargetPos = CurDualTarget.transform.position;
        Vector3 PulledPos = transform.position + transform.forward;
        yield return new WaitForSeconds(3f);
        if (IsOwner) ChainingDespawn(tossID);

        yield return null;

        // RPC로 수정
        CurDualTarget.transform.position = PulledPos;

        yield return null;
        if(IsOwner) PrisonObject.PrisonTurnOnServerRpc();
        if(CurDualTarget.IsOwner) CurDualTarget.AppearEffectServerRpc();
        yield return new WaitForSeconds(5f);

        if(IsOwner) GetComponent<AIBossCharacterAnimatorManager>().PlayAnimationOnNetwork(AnimationType.Attack, "Boss1_DualBegin_Execute", true);
    }

    public void OnDualStart()
    {
        if (IsOwner)
        {
            // Animation
            PrisonPatternPlaying = true;
        }
    }


    [ServerRpc]
    public void SetDualTargetServerRpc(int idx, ulong tossID)
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects[tossID].GetComponent<PlayerManager>().DisappearEffectServerRpc();
        GravityOnWithIDServerRpc(CurDualTarget.PlayerID);
        SetDualTargetClientRpc(idx, tossID);
    }

    [ClientRpc]
    public void SetDualTargetClientRpc(int idx,ulong tossID)
    {

        CurDualTarget = NetworkManager.Singleton.SpawnManager.SpawnedObjects[tossID].GetComponent<PlayerManager>();
        GetComponent<AIBossCharacterCombatManager>().currentTarget = CurDualTarget;
        StartCoroutine(DualEffect(idx));
        
    }

    public void ChainingDespawn(int idx)
    {
        Chainings[idx].GetComponent<NetworkObject>().Despawn();
    }

    #endregion

    #region Prison Pattern RPCs

    [ServerRpc]
    public void PrisonBeginOffServerRpc()
    {
        PrisonBeginOffClientRpc();
    }

    [ClientRpc]
    public void PrisonBeginOffClientRpc()
    {

    }

    [ServerRpc]
    public void GravityOffServerRpc()
    {
        GravityOffClientRpc();
    }

    [ClientRpc]
    public void GravityOffClientRpc()
    {
        foreach (ulong playerObjectID in NetworkManager.Singleton.SpawnManager.SpawnedObjects.Keys)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerObjectID].GetComponent<PlayerManager>() != null)
            {
                NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerObjectID].GetComponent<PlayerLocomotionManager>().DisableGravitySettings();
                NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerObjectID].GetComponent<PlayerLocomotionManager>();
            }
        }
    }

    [ServerRpc]
    public void GravityOnWithIDServerRpc(ulong playerID)
    {
        GravityOnWithIDClientRpc(playerID);
    }

    [ClientRpc]
    public void GravityOnWithIDClientRpc(ulong playerID)
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerID].GetComponent<PlayerLocomotionManager>().EnableGravitySettings();
    }


    [ServerRpc]
    public void PrisonSetBossCompServerRpc()
    {
        PrisonSetBossCompClientRpc();
    }

    [ClientRpc]
    public void PrisonSetBossCompClientRpc()
    {
        PrisonObject.owner = this;
    }

    #endregion

    #region GroggyPattern

    public void OnGroggyStart()
    {
        if (!IsOwner) return;
        
        GameManager.Instance.SendMessageServerRpc("여기에 플레이어 수 * 4 만큼, 때려야 하는걸 알려줘야 함");
        int numbOfP = NetworkManager.Singleton.ConnectedClients.Count;
        BreakCount.Value = numbOfP * 4;
    }

    public void GroggyHit(int oldValue, int newValue)
    { 
        BreakCount.Value = newValue;

        if (BreakCount.Value <= 0)
        {
            BreakCount.Value = 0;
            GetComponent<AIBossCharacterNetworkManager>().currentHp.Value = 0; // DEAD
        }
    }

    #endregion

    #region Clean Codes

    public void ResetAll()
    {
        StartCoroutine(ResetFrameDelay());
    }

    IEnumerator ResetFrameDelay()
    {
        yield return null;

        PrisonObject.CleanRemained();
        PrisonReturnServerRpc();

        MagicCircles = new();
        MagicCircleObjects = new();
        CircleObjectQueue = new();
    }

    public void ResetState()
    {
        isMagicCirclePatternPlaying = false;
        PrisonPatternBreaked = false;
        PrisonPatternPlaying = false;
    }

    [ServerRpc]
    public void PrisonReturnServerRpc()
    {
        PrisonReturnClientRpc();
        PrisonObject.GetComponent<NetworkObject>().Despawn();
    }

    [ClientRpc]
    public void PrisonReturnClientRpc()
    {
        Destroy(PrisonObject);
    }

    #endregion

    // Propagate Number of Players.
    [ServerRpc]
    public void PropagationNumbPlayerServerRpc()
    {
        int connectedPlayerNumber = NetworkManager.Singleton.ConnectedClients.Count;

        PropagationNumbPlayerClientRpc(connectedPlayerNumber);
    }

    [ClientRpc]
    public void PropagationNumbPlayerClientRpc(int connectedPlayerNumber)
    {
        NumbOfPlayer = connectedPlayerNumber;
    }

    [ServerRpc(RequireOwnership =false)]
    public void CircleObjectReproduceServerRpc(int idx)
    {
        //if (IsOwner) 
        //{
        //    var circleObj = Instantiate(CircleObjectPrefab).GetComponent<WarriorGolemPattern3CircleObject>();
        //    circleObj.tossValueIdx = idx;
        //    CircleObjectQueue.Enqueue(circleObj);
        //}

        CircleObjectReproduceClientRpc(idx);
    }

    [ClientRpc]
    public void CircleObjectReproduceClientRpc(int idx)
    {
        var circleObj = ObjectPoolManager.Singleton.GetObject(CircleObjectPrefab).GetComponent<WarriorGolemPattern3CircleObject>();
        if (IsOwner)
        {
            circleObj.tossValueIdx = idx;
            CircleObjectQueue.Enqueue(circleObj);
        }
    }


}
