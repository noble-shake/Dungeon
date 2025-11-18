using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "BreakCondition", story: "[BreakState] is [True]", category: "Conditions", id: "9b79c6da1244b20437f55c6faa72e705")]
public partial class BreakCondition : Condition
{
    [SerializeReference] public BlackboardVariable<bool> BreakState;
    [SerializeReference] public BlackboardVariable<bool> True;

    public override bool IsTrue()
    {
        return (BreakState == True);
    }
}
