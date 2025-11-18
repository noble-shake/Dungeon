using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Unity.Behavior;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

// 1. Dictionary
// 2. Prevent Repeat Priority

// 해당 클래스는 AI들에게 장착되어 Host에서만 실행됨.
public class AIAggroManager : MonoBehaviour
{
    // 어그로 가중치
    [SerializeField] float AWeight;
    // 캐릭터들마다 어그로 가중치 딕셔너리

    private List<ulong> alivePlayerList = new();
    private Dictionary<ulong, float> AggroCollection;
    // 캐릭터들마다 어그로 할당된 횟수 기록용 딕셔너리
    private Dictionary<ulong, int> AggroCounter;
    private ulong prevID;
    private ulong pprevID;
    void Start()

    {
        agent = GetComponent<BehaviorGraphAgent>();
        AggroCollection = new Dictionary<ulong, float>();
        AggroCounter = new Dictionary<ulong, int>();
        // PlayerCheck();
    }

    private BehaviorGraphAgent agent;

    public void SetAggro(ulong _PlayerID, float _value)
    {
        // SetTarget(_PlayerID);
        AggroCollection[_PlayerID] += _value;
    }

    private void AggroClear()
    {
        Dictionary<ulong, float> temp = new Dictionary<ulong, float>(AggroCollection);
        Dictionary<ulong, int> tempCnt = new Dictionary<ulong, int>(AggroCounter);
        foreach (ulong comp in temp.Keys)
        {
            temp[comp] = 0f;
            tempCnt[comp] = 0;
        }
        AggroCollection = temp;
        AggroCounter = tempCnt;
    }

    private bool JudgeTargetable(ulong _playerID)
    {
        if (Mathf.Abs(AggroCounter[_playerID] - AggroCounter.Values.Min()) > 2)
        {
            return false;
        }

        return true;
    }
    private bool IsTargetable(ulong _playerID)
    {
        if (Mathf.Abs(AggroCounter[_playerID] - AggroCounter.Values.Min()) > 2)
        {
            return false;
        }

        return true;
    }


    public ulong GetTarget()
    {
        // 현재 살아있는 대상만 리스트에 추가한다. 
        PlayerCheck();

        switch (alivePlayerList.Count)
        {
            case 0:
                Debug.Log("No Target Exists.");
                return 123456789;
            // 혼자서 플레이할 때
            case 1:
                Debug.Log("Only One Target Exists.");
                return alivePlayerList[0];
            // 플레이어는 여러 명인데 처음 어그로를 계산할 때의 케이스
            case 2:
            case 3:
            case 4:
                // 현재 까지의 아군 어그로 값들을 리스트로 치환
                List<Tuple<int, float>> distancePerPlayer = new List<Tuple<int, float>>();
                // 죽었으면 어그로 리스트에서 삭제
                for (int idx = alivePlayerList.Count - 1; idx >= 0; idx--)
                {
                    PlayerManager player = NetworkManager.Singleton.SpawnManager.SpawnedObjects[alivePlayerList[idx]].GetComponent<PlayerManager>();
                    if (player.isDead.Value == true)
                    {
                        alivePlayerList.RemoveAt(idx);
                        continue;
                    }
                    distancePerPlayer.Add(new Tuple<int, float>(idx, Vector3.Distance(transform.position, player.transform.position)));
                }

                // 현재는 최단 거리의 루트를 찾아감. 
                distancePerPlayer.Sort((a, b) => a.Item2.CompareTo(b.Item2));
                int closestPlayerID = distancePerPlayer[0].Item1;

                // 랜덤으로 하나 고르고 얘가 합당하면 반환, 그렇지 않으면 재선택
                // int tossPlayerID = Random.Range(0, AggroPlayers.Count);
                // while (IsTargetable(AggroPlayers[tossPlayerID]) == false)
                // {
                //     Debug.Log("플레이어 재선택 중입니다.");
                //     tossPlayerID = Random.Range(0, AggroPlayers.Count);
                // }
                return alivePlayerList[closestPlayerID];

            default:
                Debug.LogError("인원수가 0명 미만, 4명 초과라는 것은 말이 안 됩니다.");
                return 123456789;

        }


    }

    // 공격해야 되는 순간의 네트워크에 접속한 유저들을 딕셔너리에 추가한다.
    // 이미 등록되어 있는 캐릭터들은 영향이 없다.
    private void PlayerCheck()
    {
        alivePlayerList.Clear();

        foreach (ulong playerObjectID in NetworkManager.Singleton.SpawnManager.SpawnedObjects.Keys)
        {
            NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerObjectID].TryGetComponent(out PlayerManager player);
            if (player != null && player.isDead.Value != true)
            {
                alivePlayerList.Add(player.NetworkObjectId);
            }

        }
    }
}
