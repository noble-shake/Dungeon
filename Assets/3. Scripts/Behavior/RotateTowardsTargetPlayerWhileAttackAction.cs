using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RotateTowardsTargetPlayerWhileAttack", story: "[AI] Rotates Towards [TargetPlayer]", category: "Action", id: "2361422edea5be325d8b60446c413263")]
public partial class RotateTowardsTargetPlayerWhileAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    [SerializeReference] public BlackboardVariable<PlayerManager> TargetPlayer;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Vector3 targetDirection = AI.Value.aiCharacterCombatManager.currentTarget.transform.position - AI.Value.transform.position;
        targetDirection.y = 0f;
        targetDirection.Normalize();

        if (targetDirection == Vector3.zero) targetDirection = AI.Value.transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        AI.Value.transform.rotation = Quaternion.Slerp(AI.Value.transform.rotation, targetRotation, AI.Value.aiCharacterCombatManager.attackRotationSpeed * Time.deltaTime);
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

