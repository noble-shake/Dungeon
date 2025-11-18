using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "TargetPlayerSet", story: "[TargetPlayer] Is Set", category: "Conditions", id: "942b38f3e259e8316d06710fffb9f516")]
public partial class TargetPlayerSetCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> TargetPlayer;

    public override bool IsTrue()
    {
        if (TargetPlayer.Value != null)
        {
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
