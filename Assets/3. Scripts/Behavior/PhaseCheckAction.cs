using System;
using Unity.Behavior;
using UnityEngine;
using Condition = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PhaseCheckAction", story: "If [AI] 's HP Check", category: "Action/Conditional", id: "81095498d10f40095c75d8068e79f39b")]
public partial class PhaseCheckAction : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> AI;
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

    }
}

