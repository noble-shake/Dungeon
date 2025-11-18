using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PlayIdleAnimation", story: "[AniamtorManager] Plays Idle Animation", category: "Action", id: "850ee408cc081fda8950881f69444ff4")]
public partial class PlayIdleAnimationAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterAnimatorManager> AniamtorManager;
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

