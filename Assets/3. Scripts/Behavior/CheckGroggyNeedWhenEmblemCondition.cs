using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckGroggyNeedWhenEmblem", story: "[Boss] Plays Groggy When Emblem Pattern", category: "Conditions", id: "1acba788e21129079b83751ef7487edc")]
public partial class CheckGroggyNeedWhenEmblemCondition : Condition
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;

    public override bool IsTrue()
    {
        if (Boss.Value.emblemPattern.isGroggyTime)
        {
            if (Boss.Value.emblemPattern.patternChance == 0)
            {
                Boss.Value.emblemPattern.isGroggyTime = false;
                return false;
            }
            Boss.Value.emblemPattern.isGroggyTime = false;
            return true;
        }
        return false;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
