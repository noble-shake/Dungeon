using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Boss places Self and Players", story: "[Boss] Places Self And Players", category: "Action", id: "ee2382132130785ccfd54c927c16f047")]
public partial class BossPlacesSelfAndPlayersAction : Action
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;

    protected override Status OnStart()
    {
        // 보스가 벽으로 점프한다.
        Boss.Value.tornadoPattern.JumpToWallServerRpc(Boss.Value.transform.position);
        // 플레이어를 벽으로 밀쳐낸다.
        // Boss.Value.tornadoPattern.PlacePlayersTowardsWallServerRpc();
        return Status.Success;
    }
}

