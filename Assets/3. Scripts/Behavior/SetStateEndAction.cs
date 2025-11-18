using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetStateEnd", story: "Set [RushGimmickState] To End", category: "Action", id: "3a675e8b1c87b529321a82c898fa0b74")]
public partial class SetStateEndAction : Action
{
    [SerializeReference] public BlackboardVariable<RushGimmickState> RushGimmickState;

    protected override Status OnStart()
    {
        RushGimmickState.Value = global::RushGimmickState.End;
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

