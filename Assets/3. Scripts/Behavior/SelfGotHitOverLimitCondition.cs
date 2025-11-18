using BehaviorDesigner.Runtime.Tasks;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Self Got Hit Over Limit", story: "[Boss] Got Hit Over Limit", category: "Conditions", id: "64a8deaa31196f49d7b0eb5bf41fd1af")]
public partial class SelfGotHitOverLimitCondition : Condition
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;

    public override bool IsTrue()
    {
        int count = Boss.Value.bossNetworkManager.gimmickHitCount.Value;
        int threshold = Boss.Value.swordWavePattern.ParryiedWaveHitCountThreshold;
        if (count >= threshold)
            return true;
        return false;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
