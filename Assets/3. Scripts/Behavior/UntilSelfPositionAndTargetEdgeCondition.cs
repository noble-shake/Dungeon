using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Until Self Position And TargetEdge", story: "Untill [Self] Position And [TargetEdge] not match", category: "Conditions", id: "87c8d3788541ab426e866ef39ee37a10")]
public partial class UntilSelfPositionAndTargetEdgeCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Vector3> TargetEdge;

    public override bool IsTrue()
    {
        return true;
    }
}
