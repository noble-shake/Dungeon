using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CheckGimmickEndCondition", story: "[Boss] Checks PatternSuccess", category: "Action", id: "d3d0bf46712e1ac6bd20a1c854c49bb1")]
public partial class CheckGimmickEndConditionAction : Action
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;
    // [SerializeReference] public BlackboardVariable<bool> PatternSuccess;
    protected override Status OnUpdate()
    {
        int gimmickHitCount = Boss.Value.bossNetworkManager.gimmickHitCount.Value;
        if (gimmickHitCount >= Boss.Value.swordWavePattern.ParryiedWaveHitCountThreshold)
        {
            // PatternSuccess.Value = true;
            return Status.Success;
        }
        // PatternSuccess.Value = false;
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

