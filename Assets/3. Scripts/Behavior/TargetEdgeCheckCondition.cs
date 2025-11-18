using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "TargetEdge Check", story: "[TargetEdge] is null", category: "Conditions", id: "5e43cfc3d6cf571d3e1da25f9e12f6b5")]
public partial class TargetEdgeCheckCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Vector3> TargetEdge;

    public override bool IsTrue()
    {
        return TargetEdge == null;
    }
}
