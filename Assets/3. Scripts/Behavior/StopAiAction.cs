using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "StopAI", story: "[AI] Stops while Idle State", category: "Action", id: "b7d5a4e941337a91adfbb59fd8d9ad4c")]
public partial class StopAiAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    protected override Status OnStart()
    {
        // AI.Value.GetComponent<NavMeshAgent>().enabled = false;
        // AI.Value.GetComponent<NavMeshAgent>().ResetPath();
        // AI.Value.aiCharacterNetworkManager.isMoving.Value = false;

        return Status.Success;
    }
}

