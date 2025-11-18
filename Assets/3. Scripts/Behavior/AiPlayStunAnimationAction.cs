using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AIPlayStunAnimation", story: "[AI] Plays Stun Animation", category: "Action", id: "b90295cd4a5838b849e0d29eb366c600")]
public partial class AiPlayStunAnimationAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;

    protected override Status OnStart()
    {
        Debug.Log("얘가 매번 실행 된다고?");
        AI.Value.aiCharacterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Hit, "stun", true);
        return Status.Success;
    }

}

