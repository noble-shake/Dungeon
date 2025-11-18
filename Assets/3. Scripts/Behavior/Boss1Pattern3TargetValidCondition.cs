using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Boss1Pattern3TargetValid", story: "[TargetPlayer] is Valid in Pattern3", category: "Conditions", id: "3670b6db1a198fc12bd84ad37d97b453")]
public partial class Boss1Pattern3TargetValidCondition : Condition
{
    [SerializeReference] public BlackboardVariable<PlayerManager> TargetPlayer;

    public override bool IsTrue()
    {
        if (TargetPlayer.Value != null)
        {
            return !TargetPlayer.Value.isDead.Value;

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
