using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckTargetPlayerValid", story: "[TargetPlayer] Is InValid", category: "Conditions", id: "bf3c27a01c83297c0c5cd23ad23832d2")]
public partial class CheckTargetPlayerValidCondition : Condition
{
    [SerializeReference] public BlackboardVariable<PlayerManager> TargetPlayer;

    public override bool IsTrue()
    {
        if (TargetPlayer.Value == null) return true;
        if (TargetPlayer.Value.isDead.Value)
        {
            TargetPlayer = null;
            return true;
        }
        else
            return false;
    }

}
