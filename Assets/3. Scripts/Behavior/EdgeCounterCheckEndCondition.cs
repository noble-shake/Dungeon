using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "EdgeCounterCheckEndCondition", story: "[EdgeCounter] Equals To [Value]", category: "Conditions", id: "0fa41ada7f6cc34b5d39459f8928f7f7")]
public partial class EdgeCounterCheckEndCondition : Condition
{
    [SerializeReference] public BlackboardVariable<int> EdgeCounter;
    [SerializeReference] public BlackboardVariable<int> Value;

    public override bool IsTrue()
    {
        return EdgeCounter >= Value.Value;
    }
}
