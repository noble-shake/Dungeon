using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Boss1Pattern3FailProcess", story: "[Self] Change Values in Fail Condition", category: "Action", id: "4f7ab6d6048e4d6c3b7572106024802b")]
public partial class Boss1Pattern3FailProcessAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        Self.Value.GetComponent<WarriorGolemPattern3Component>().ResetAll();
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

