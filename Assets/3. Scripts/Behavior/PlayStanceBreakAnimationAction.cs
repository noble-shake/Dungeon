using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PlayStanceBreakAnimation", story: "[AI] Plays StanceBreak Animation", category: "Action", id: "603e84526156f99f238fd17c8f239ee0")]
public partial class PlayStanceBreakAnimationAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;

    protected override Status OnStart()
    {
        AI.Value.characterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Hit, "Stance_Break_01", true, transitionNomalizedTime: 0f);

        return Status.Success;
    }
}

