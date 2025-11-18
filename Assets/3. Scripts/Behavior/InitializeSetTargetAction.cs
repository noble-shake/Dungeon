using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "InitializeSetTargetAction", story: "Initialize for Set [TargetPlayer]", category: "Action/Conditional", id: "61de69ba2bcf5401c0164f94eca97db6")]
public partial class InitializeSetTargetAction : Action
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
    }
}

