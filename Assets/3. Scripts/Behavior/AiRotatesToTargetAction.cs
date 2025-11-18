using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using TeleportFX;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AI Rotates To Target", story: "[AI] Rotates To [Target] For [Seconds] Seconds", category: "Action", id: "a1ed09c34c3c42ee15d4cdde442e6331")]
public partial class AiRotatesToTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    [SerializeReference] public BlackboardVariable<Transform> Target;
    [SerializeReference] public BlackboardVariable<float> Seconds;
    private float elapsedTime = 0f;
    private FirstBossGimmickFunc rushGimmick;
    protected override Status OnStart()
    {
        // 맨 처음에 각도에 따라 회전 애니메이션 실행 
        rushGimmick = AI.Value.GetComponent<FirstBossGimmickFunc>();
        rushGimmick.RotateByAngle(Target.Value.gameObject);
        // rushGimmick.indicatorActive.Value = true;
        // rushGimmick.indicator.FillProgress = 0;
        // rushGimmick.indicatorFillProgress.Value = rushGimmick.indicator.FillProgress;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // 애니메이션 실행 이후 계속 각도 맞춰주기. 언제까지? 
        if (AI.Value.isPerformingAction == true)
        {
            return Status.Running;
        }
        else
        {
            if (elapsedTime < Seconds.Value)
            {
                // rushGimmick.indicator.FillProgress = elapsedTime / Seconds.Value;
                // rushGimmick.indicatorFillProgress.Value = rushGimmick.indicator.FillProgress;
                elapsedTime += Time.deltaTime;
                Vector3 targetDirection = Target.Value.transform.position - AI.Value.transform.position;
                targetDirection.y = 0f;
                targetDirection.Normalize();
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                AI.Value.transform.rotation = Quaternion.Slerp(AI.Value.transform.rotation, targetRotation, 10 * Time.deltaTime);

                AI.Value.GetComponent<FirstBossGimmickFunc>().targetTransform.Value = Target.Value.transform.position;

                return Status.Running;
            }
            else
            {
                // rushGimmick.indicator.FillProgress = 1f;
                // rushGimmick.indicatorFillProgress.Value = rushGimmick.indicator.FillProgress;

                elapsedTime = 0f;
                return Status.Success;
            }
        }
    }

    protected override void OnEnd()
    {

    }
}

