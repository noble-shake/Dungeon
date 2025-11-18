using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckEmblemPlayerDead", story: "[Boss] Watches Selected Player Dead", category: "Conditions", id: "b79c1da0aa5b7dd099ff2fc1ce7b77f9")]
public partial class CheckEmblemPlayerDeadCondition : Condition
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;

    public override bool IsTrue()
    {
        if (Boss.Value.emblemPattern.selectedPlayer.isDead.Value)
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
