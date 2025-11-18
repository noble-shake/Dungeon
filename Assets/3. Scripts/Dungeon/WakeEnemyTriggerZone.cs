using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WakeEnemyTriggerZone : NetworkBehaviour
{
    [SerializeField] private AICharacterSpawner[] spawnerList;
    [SerializeField] private List<AICharacterManager> enemyList = new();

    private void Start()
    {
        StartCoroutine(WaitAndGetInstantiatedObject());
    }

    private IEnumerator WaitAndGetInstantiatedObject()
    {
        foreach (var spawner in spawnerList)
        {
            while (true)
            {
                if (spawner.GetInstantiatedObject() == null)
                    yield return null;
                enemyList.Add(spawner.GetInstantiatedObject().GetComponent<AICharacterManager>());
                break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 서버를 호출해서 들어온 사람을 적으로 인식하게끔 한다. 

        // 서버RPC 호출해서 인식해야 할 에너미 리스트를 보냄.
        // 서버에서 강제로 걔네의 타겟을 영역에 발을 들인 플레이어로 인식하게끔 수정.
        // 이 영역은 한번 사용되면 두번 다시 사용 되면 안 됨.
        // 기존 AI는 자기 자리 개념이 있음. 멀리 벗어나면 돌아감. -> behavior 트리 나중에 추가? 
        NetworkObject player = other.GetComponent<NetworkObject>();
        if (player != null && player.IsLocalPlayer && player.gameObject.tag == "Player")
        {
            ulong[] enemyIds = new ulong[enemyList.Count];
            for (int i = 0; i < enemyList.Count; i++)
            {
                enemyIds[i] = enemyList[i].NetworkObjectId;
            }
            WakeUpEnemyAIServerRpc(enemyIds, player.NetworkObjectId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void WakeUpEnemyAIServerRpc(ulong[] enemyIds, ulong networkObjectId)
    {
        PlayerManager player = FindNetworkObjectById(networkObjectId).GetComponent<PlayerManager>();
        foreach (ulong enemyId in enemyIds)
        {
            NetworkObject enemyNetworkObject = FindNetworkObjectById(enemyId);
            if (enemyNetworkObject != null)
            {
                AICharacterManager enemy = enemyNetworkObject.GetComponent<AICharacterManager>();
                if (enemy != null)
                {
                    // 타겟을 플레이어로 설정. 그런데 이 땐 그럼 시야 사거리를 없애줘야 하지 않나? 
                    // 여기서 그럼 문제 발생. 한번 방에 들어온 사람은 못 나가게 해주나? 
                    // enemy.WakeUpAndSetTarget(player);
                    enemy.aiCharacterCombatManager.currentTarget = player;
                }
            }
        }
    }

    private NetworkObject FindNetworkObjectById(ulong networkObjectId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject networkObject))
        {
            return networkObject;
        }
        return null;
    }

}
