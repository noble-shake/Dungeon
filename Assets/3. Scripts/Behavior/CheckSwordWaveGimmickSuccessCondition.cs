using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckSwordWaveGimmickSuccess", story: "[Boss] Decides Gimmick Success With [PatternSuccess]", category: "Conditions", id: "1ac8c7eec91661bbb8b319bbfe37d306")]
public partial class CheckSwordWaveGimmickSuccessCondition : Condition
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;
    [SerializeReference] public BlackboardVariable<bool> PatternSuccess;

    public override bool IsTrue()
    {
        // int count = Boss.Value.bossNetworkManager.gimmickHitCount.Value;
        // int threshold = Boss.Value.swordWavePattern.ParryiedWaveHitCountThreshold;
        // if (count >= threshold)
        //     return true;
        // return false;

        return PatternSuccess.Value;

    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
