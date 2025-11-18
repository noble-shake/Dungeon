using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PlayCutScene", story: "[AI] Plays CutScene", category: "Action", id: "da3ec7e9a2a0209326be81878813e219")]
public partial class PlayCutSceneAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;

    protected override Status OnStart()
    {
        (AI.Value as AIBossCharacterManager).bossAnimatorManager.PlayAnimationOnNetwork(AnimationType.Etc, "CutScene", true);

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

