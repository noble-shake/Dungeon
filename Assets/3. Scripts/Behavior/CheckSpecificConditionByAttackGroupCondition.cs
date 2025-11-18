using System;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Netcode;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckSpecificConditionByAttackGroup", story: "[AI] , [Target] Conditions Are Met When [AttackGroupValue]", category: "Conditions", id: "405201df27cde8017eb540f25a5f13b0")]
public partial class CheckSpecificConditionByAttackGroupCondition : Condition
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    [SerializeReference] public BlackboardVariable<AttackGroup> AttackGroupValue;
    [SerializeReference] public BlackboardVariable<CharacterManager> Target;

    private List<PlayerManager> playerList = new List<PlayerManager>();
    private int randomIndex = -1;

    public override bool IsTrue()
    {
        switch (AttackGroupValue.Value)
        {
            case AttackGroup.None:
                return false;
            // 그냥 조건 없이 살아있는 플레이어를 랜덤으로 지정한다.
            // 거리가 6 이하인 플레이어를 하나를 랜덤으로 지정한다.
            case AttackGroup.AGroup:
                playerList.Clear();
                foreach (var networkObject in NetworkManager.Singleton.SpawnManager.SpawnedObjects)
                {
                    if (networkObject.Value.TryGetComponent(out PlayerManager player))
                    {
                        if (player.isDead.Value) continue;
                        float distacne = Vector3.Distance(player.transform.position, AI.Value.transform.position);
                        if (distacne <= 8f)
                        {
                            playerList.Add(player);
                        }
                    }
                }
                // foreach (PlayerManager player in GameManager.Instance.connectedPlayerList)
                // {
                //     if (player.isDead.Value)
                //         continue;
                //     float distacne = Vector3.Distance(player.transform.position, AI.Value.transform.position);
                //     if (distacne <= 8f)
                //     {
                //         playerList.Add(player);
                //     }
                // }
                if (playerList.Count == 0)
                    return false;
                else
                {
                    randomIndex = UnityEngine.Random.Range(0, playerList.Count);
                    Target.Value = playerList[randomIndex];
                    AI.Value.aiCharacterCombatManager.currentTarget = Target.Value;
                    return true;
                }
            // AI 뒤 쪽 일정거리 이내에 살아있는 플레이어들 중 하나를 랜덤으로 지정한다.
            case AttackGroup.BGroup:
                playerList.Clear();
                foreach (var networkObject in NetworkManager.Singleton.SpawnManager.SpawnedObjects)
                {
                    if (networkObject.Value.TryGetComponent(out PlayerManager player))
                    {
                        if (player.isDead.Value) continue;
                        float distacne = Vector3.Distance(player.transform.position, AI.Value.transform.position);
                        if (distacne < 3f)
                        {
                            Vector3 dir = (player.transform.position - AI.Value.transform.position).normalized;
                            float dot = Vector3.Dot(AI.Value.transform.forward, dir);
                            if (dot < 0f)
                            {
                                playerList.Add(player);
                            }
                        }
                    }
                }
                // foreach (PlayerManager player in GameManager.Instance.connectedPlayerList)
                // {
                //     if (player.isDead.Value)
                //         continue;
                //     float distacne = Vector3.Distance(player.transform.position, AI.Value.transform.position);
                //     if (distacne < 3f)
                //     {
                //         Vector3 dir = (player.transform.position - AI.Value.transform.position).normalized;
                //         float dot = Vector3.Dot(AI.Value.transform.forward, dir);
                //         if (dot < 0f)
                //         {
                //             playerList.Add(player);
                //         }
                //     }
                // }
                if (playerList.Count == 0)
                    return false;
                else
                {
                    randomIndex = UnityEngine.Random.Range(0, playerList.Count);
                    Target.Value = playerList[randomIndex];
                    AI.Value.aiCharacterCombatManager.currentTarget = Target.Value;

                    return true;
                }
            // 거리가 4 초과인인 플레이어들 중 하나를 랜덤으로 지정한다.
            case AttackGroup.DGroup:
                playerList.Clear();
                foreach (var networkObject in NetworkManager.Singleton.SpawnManager.SpawnedObjects)
                {
                    if (networkObject.Value.TryGetComponent(out PlayerManager player))
                    {
                        if (player.isDead.Value) continue;
                        float distacne = Vector3.Distance(player.transform.position, AI.Value.transform.position);
                        if (8f < distacne)
                        {
                            playerList.Add(player);
                        }
                    }
                }
                // foreach (PlayerManager player in GameManager.Instance.connectedPlayerList)
                // {
                //     if (player.isDead.Value)
                //         continue;
                //     float distacne = Vector3.Distance(player.transform.position, AI.Value.transform.position);
                //     if (8f < distacne)
                //     {
                //         playerList.Add(player);
                //     }
                //     playerList.Add(player);
                // }
                if (playerList.Count == 0)
                    return false;
                else
                {
                    randomIndex = UnityEngine.Random.Range(0, playerList.Count);
                    Target.Value = playerList[randomIndex];
                    AI.Value.aiCharacterCombatManager.currentTarget = Target.Value;

                    return true;
                }
            // 일정 거리 이상에 플레이어가 존재한다.
            case AttackGroup.RGroup:
            case AttackGroup.Dash:
                playerList.Clear();
                foreach (var networkObject in NetworkManager.Singleton.SpawnManager.SpawnedObjects)
                {
                    if (networkObject.Value.TryGetComponent(out PlayerManager player))
                    {
                        if (player.isDead.Value) continue;
                        float distacne = Vector3.Distance(player.transform.position, AI.Value.transform.position);
                        if (distacne > 10f)
                        {
                            playerList.Add(player);
                        }
                    }
                }
                // foreach (PlayerManager player in GameManager.Instance.connectedPlayerList)
                // {
                //     if (player.isDead.Value)
                //         continue;
                //     float distacne = Vector3.Distance(player.transform.position, AI.Value.transform.position);
                //     if (distacne > 10f)
                //     {
                //         playerList.Add(player);
                //     }
                // }
                if (playerList.Count == 0)
                    return false;
                else
                {
                    randomIndex = UnityEngine.Random.Range(0, playerList.Count);
                    Target.Value = playerList[randomIndex];
                    AI.Value.aiCharacterCombatManager.currentTarget = Target.Value;
                    return true;
                }
        }
        return false;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
