using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.Netcode;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RushTowardsPlayer", story: "[AI] Rushes Towards [TargetPlayer]", category: "Action", id: "70cf7d0827d77348a41942f35fce42ba")]
public partial class RushTowardsPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    [SerializeReference] public BlackboardVariable<PlayerManager> TargetPlayer;

    protected override Status OnStart()
    {
        // 얘가 RPC로 실행되어야 함. 
        AI.Value.GetComponent<RushUntilCollision>().BossRushServerRpc(TargetPlayer.Value.transform.position);
        return Status.Success;
    }

}

