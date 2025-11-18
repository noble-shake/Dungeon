using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Boss1_MoveCheck", story: "Caompare [Self] Position And [TargetEdge]", category: "Conditions", id: "a61417ea0604c14e80487b719d4ac6ac")]
public partial class Boss1MoveCheckCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Vector3> TargetEdge;

    public override bool IsTrue()
    {
        return Vector3.Distance(Self.Value.transform.position, TargetEdge.Value) < 1f;
    }
}
