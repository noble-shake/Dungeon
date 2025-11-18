using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "EdgeCounterCheck", story: "[EdgeCounter] Lower Than [Value]", category: "Conditions", id: "b2b5cee9b68a05fdc88a040d5317faa1")]
public partial class EdgeCounterCheckCondition : Condition
{
    [SerializeReference] public BlackboardVariable<int> EdgeCounter;
    [SerializeReference] public BlackboardVariable<int> Value;

    public override bool IsTrue()
    {
        return EdgeCounter < Value.Value;
    }
}
