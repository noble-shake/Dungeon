using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "IsTargetPlayerAliveAndNotNull", story: "[TargetPlayer] Is Valid", category: "Conditions", id: "b026487b706718b118791583f8e2bbc5")]
public partial class IsTargetPlayerAliveAndNotNullCondition : Condition
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
