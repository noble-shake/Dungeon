using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveAi", story: "[AI] Prepares To Move To [TargetPlayer]", category: "Action", id: "3ddd0a150c862ff3ec9dafc15efee026")]
public partial class MoveAiAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    [SerializeReference] public BlackboardVariable<PlayerManager> TargetPlayer;

    protected override Status OnStart()
    {
        // AI.Value.GetComponent<NavMeshAgent>().enabled = true;
        AI.Value.aiCharacterNetworkManager.isMoving.Value = true;
        // AI.Value.GetComponent<NavMeshAgent>().SetDestination()
        return Status.Running;
    }
}

