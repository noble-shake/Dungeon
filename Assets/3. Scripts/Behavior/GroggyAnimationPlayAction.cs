using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GroggyAnimationPlay", story: "[Boss] Plays Groggy Animation For [second] Seconds", category: "Action", id: "9041fe89879248e4ba899b193d99061f")]
public partial class GroggyAnimationPlayAction : Action
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;
    [SerializeReference] public BlackboardVariable<int> Second;
    protected override Status OnStart()
    {
        Boss.Value.bossAnimatorManager.PlayGroggyAnimation(Second.Value);
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

