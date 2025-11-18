using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Boss1Pattern3PrisionBreakCheck", story: "[Self] Check Prison Break", category: "Conditions", id: "1b7a93a2bd777321d1b3e3dca9bc516d")]
public partial class Boss1Pattern3PrisionBreakCheckCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    public override bool IsTrue()
    {
        return true;
    }
}
