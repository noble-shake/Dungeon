using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetGimmickVariable", story: "[AI] Sets [RushCount] and [GimmickSuccessCount]", category: "Action", id: "cac9ae52068111d2d5a5ee384eea462f")]
public partial class SetGimmickVariableAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    [SerializeReference] public BlackboardVariable<int> RushCount;
    [SerializeReference] public BlackboardVariable<int> GimmickSuccessCount;
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

