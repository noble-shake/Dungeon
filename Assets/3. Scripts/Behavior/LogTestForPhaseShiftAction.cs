using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "LogTestForPhaseShift", story: "Test", category: "Action", id: "38ca8b728594098e9110452ff23d7f44")]
public partial class LogTestForPhaseShiftAction : Action
{

    protected override Status OnStart()
    {
        Debug.Log("Pattern 1 Engage !!!!!!");
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

