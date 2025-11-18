using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Self Anim Stop", story: "[Self] Stop Wheelwind", category: "Action", id: "82fe3867788facc9a9fd16fab7e56a1e")]
public partial class SelfAnimStopAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    protected override Status OnStart()
    {
        Self.Value.GetComponent<AICharacterManager>().aiCharacterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Attack, "GimmikIllusionEnd", true);
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

