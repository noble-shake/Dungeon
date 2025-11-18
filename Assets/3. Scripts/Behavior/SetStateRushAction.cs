using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetStateRush", story: "Set [RushGimmickState] To Rush", category: "Action", id: "7b52633a9aaee4b04d92aba342c1f6a3")]
public partial class SetStateRushAction : Action
{
    [SerializeReference] public BlackboardVariable<RushGimmickState> RushGimmickState;

    protected override Status OnStart()
    {
        RushGimmickState.Value = global::RushGimmickState.Rush;
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

