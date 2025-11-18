using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "StackUp", story: "[Stack] Increases By [Amount]", category: "Action", id: "489291377797dfba2454dc1e082b1d28")]
public partial class StackUpAction : Action
{
    [SerializeReference] public BlackboardVariable<int> Stack;
    [SerializeReference] public BlackboardVariable<int> Amount;

    protected override Status OnStart()
    {
        Stack.Value += Amount.Value;
        return Status.Success;
    }

}

