using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PhaseChange", story: "Phase Initialize", category: "Action", id: "0b9a23233d9b7c4429d0f165a3a94560")]
public partial class PhaseChangeAction : Action
{

    protected override Status OnStart()
    {



        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

