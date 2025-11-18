using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Boss1Pattern3PrisonBreakCheck", story: "[Self] Check Prison Breaked", category: "Conditions", id: "a1bb58ebc9ea26f88a8da3b3fc4fe88e")]
public partial class Boss1Pattern3PrisonBreakCheckCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    public override bool IsTrue()
    {
        if (Self.Value.GetComponent<WarriorGolemPattern3Component>().PrisonPatternBreaked == true)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
