using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;


[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Assign Target", story: "[TargetPlayer] is Assigned", category: "Action", id: "016bfe1add9e7a5c409fd827091a1a53")]
public partial class AssignTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> TargetPlayer;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
        // Test;
    }
}

