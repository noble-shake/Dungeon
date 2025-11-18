using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RotateAI", story: "[AI] Rotates Towards [TargetPlayer]", category: "Action", id: "e42417d6613f4175e58481136a90bbde")]
public partial class RotateAiAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    [SerializeReference] public BlackboardVariable<PlayerManager> TargetPlayer;
    protected override Status OnStart()
    {
        if (AI.Value.aiCharacterCombatManager.currentTarget == null) return Status.Success;
        Vector3 targetDirection = AI.Value.GetComponent<AICharacterManager>().aiCharacterCombatManager.currentTarget.transform.position - AI.Value.GetComponent<AICharacterManager>().transform.position;
        targetDirection.y = 0f;
        targetDirection.Normalize();

        if (targetDirection == Vector3.zero) targetDirection = AI.Value.GetComponent<AICharacterManager>().transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        // 왜 여기서 캐릭터를 뚫어버리지? 
        AI.Value.transform.rotation = Quaternion.Slerp(AI.Value.transform.rotation, targetRotation, AI.Value.aiCharacterCombatManager.attackRotationSpeed * Time.deltaTime);

        return Status.Success;
    }
}

