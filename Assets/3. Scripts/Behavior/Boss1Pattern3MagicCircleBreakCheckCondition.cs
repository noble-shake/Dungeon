using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Boss1Pattern3MagicCircleBreakCheck", story: "[Self] Check MagicCircle Break", category: "Conditions", id: "93ae1af64af11006d64957a8599e6557")]
public partial class Boss1Pattern3MagicCircleBreakCheckCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    public override bool IsTrue()
    {
        if (Self.Value.GetComponent<WarriorGolemPattern3Component>().isMagicCirclePatternPlaying == false)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
