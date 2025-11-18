using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "StopAISlowly", story: "[AI] Stops Slowly", category: "Action", id: "297d3fa7984eb59a6bf88cec32f81373")]
public partial class StopAiSlowlyAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        AI.Value.aiCharacterAnimatorManager.UpdateAnimatorMovementParameters(0f, 0f, false);
        AI.Value.aiCharacterNetworkManager.animatorVerticalParameter.Value = AI.Value.animator.GetFloat("Vertical");
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

