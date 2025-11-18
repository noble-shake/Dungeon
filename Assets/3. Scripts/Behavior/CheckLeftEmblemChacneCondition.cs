using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckLeftEmblemChacne", story: "[Boss] Has Chance To Execute Emblem Pattern", category: "Conditions", id: "1a9912ea2b1da8cb40f33f7869aebdf6")]
public partial class CheckLeftEmblemChacneCondition : Condition
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;

    public override bool IsTrue()
    {
        return Boss.Value.emblemPattern.CheckLeftChance();
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
