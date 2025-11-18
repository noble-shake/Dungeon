using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "EdgeCounterEqual", story: "[EdgeCounter] Equal Check [Value]", category: "Conditions", id: "95ea304326a7a32d8d5caaa68a62751e")]
public partial class EdgeCounterEqualCondition : Condition
{
    [SerializeReference] public BlackboardVariable<int> EdgeCounter;
    [SerializeReference] public BlackboardVariable<int> Value;

    public override bool IsTrue()
    {
        return EdgeCounter >= 5;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
