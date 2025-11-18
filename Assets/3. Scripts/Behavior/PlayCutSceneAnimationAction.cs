using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PlayCutSceneAnimation", story: "[AI] Plays CutScene Animation", category: "Action", id: "bc47aff66aba0bef98fcf53a8f793f74")]
public partial class PlayCutSceneAnimationAction : Action
{

    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    protected override Status OnStart()
    {
        Debug.Log("나 실행 됨.");
        (AI.Value as AIBossCharacterManager).bossAnimatorManager.PlayAnimationOnNetwork(AnimationType.Etc, "CutScene", true);

        return Status.Success;
    }
}

