using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Boss1Pattern3PullforDual", story: "[Self] Pulled Player for Pattern3 Begin with Anim", category: "Action", id: "c9374e21627e67aa7e7137f2fb2a42af")]
public partial class Boss1Pattern3PullforDualAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        // TODO : Animation
        Self.Value.GetComponent<AIBossCharacterAnimatorManager>().PlayAnimationOnNetwork(AnimationType.Attack, "Boss1_BeginDual_Ready", true);
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

