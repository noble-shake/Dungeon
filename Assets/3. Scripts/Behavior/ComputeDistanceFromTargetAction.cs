using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ComputeDistanceFromTarget", story: "[AI] Computes Distance From [TargetPlayer]", category: "Action", id: "bb244f4e32630eb89b0b5b4727e3dec5")]
public partial class ComputeDistanceFromTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    [SerializeReference] public BlackboardVariable<PlayerManager> TargetPlayer;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (AI.Value.aiCharacterCombatManager.currentTarget != null)
        {
            AI.Value.aiCharacterCombatManager.targetDirection = AI.Value.aiCharacterCombatManager.currentTarget.transform.position - AI.Value.transform.position;
            AI.Value.aiCharacterCombatManager.viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(AI.Value.transform, AI.Value.aiCharacterCombatManager.targetDirection);
            AI.Value.aiCharacterCombatManager.distanceFromTarget = Vector3.Distance(AI.Value.transform.position, AI.Value.aiCharacterCombatManager.currentTarget.transform.position);
            return Status.Running;
        }
        else
        {
            AI.Value.aiCharacterCombatManager.targetDirection = Vector3.zero;
            AI.Value.aiCharacterCombatManager.viewableAngle = 0f;
            AI.Value.aiCharacterCombatManager.distanceFromTarget = 0f;
            return Status.Running;
        }

        // ProcessStateMachine();
    }

}

