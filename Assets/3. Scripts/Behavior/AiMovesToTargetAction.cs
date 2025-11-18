using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.Services.Lobbies.Models;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AI Moves To Target", story: "[AI] Moves [IsRunning] To [TargetPlayer] With [RotationSpeed]", category: "Action", id: "1ed052f462b4d53587880222487906a9")]
public partial class AiMovesToTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    [SerializeReference] public BlackboardVariable<PlayerManager> TargetPlayer;
    [SerializeReference] public BlackboardVariable<float> RotationSpeed;
    [SerializeReference] public BlackboardVariable<bool> IsRunning;
    int vertical = Animator.StringToHash("Vertical");

    protected override Status OnStart()
    {
        AI.Value.aiCharacterNetworkManager.isMoving.Value = true;

        return Status.Running;
    }


    protected override Status OnUpdate()
    {
        if (AI.Value.isPerformingAction == true)
        {
            //     AI.Value.animator.SetFloat("Vertical", 0f);
            return Status.Running;
        }
        // 보스 몹을 회전시키는 코드 
        Vector3 AIPosition = AI.Value.transform.position;
        Vector3 TargetPosition = TargetPlayer.Value.transform.position;

        Vector3 dir = TargetPosition - AIPosition;
        dir.y = 0;
        dir.Normalize();
        Quaternion rotation = Quaternion.LookRotation(dir);
        AI.Value.transform.rotation = Quaternion.Slerp(AI.Value.transform.rotation, rotation, RotationSpeed.Value * Time.deltaTime);

        if (IsRunning.Value)
        {
            AI.Value.aiCharacterAnimatorManager.UpdateAnimatorMovementParameters(0f, 1f, false);
        }
        else
        {
            AI.Value.aiCharacterAnimatorManager.UpdateAnimatorMovementParameters(0f, 0.5f, false);
        }

        AI.Value.aiCharacterNetworkManager.animatorVerticalParameter.Value = AI.Value.animator.GetFloat(vertical);
        return Status.Running;
    }

    protected override void OnEnd()
    {
        AI.Value.aiCharacterAnimatorManager.UpdateAnimatorMovementParameters(0f, 0.0f, false, 0f);
        AI.Value.aiCharacterNetworkManager.animatorVerticalParameter.Value = AI.Value.animator.GetFloat(vertical);

        IsRunning.Value = false;
    }
}

