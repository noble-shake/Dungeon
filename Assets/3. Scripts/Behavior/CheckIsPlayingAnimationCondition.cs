using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckIsPlayingAnimation", story: "[AI] Is Not Playing Animation", category: "Conditions", id: "984c100748f55437c03efc8e114aab57")]
public partial class CheckIsPlayingAnimationCondition : Condition
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;

    public override bool IsTrue()
    {

        return !AI.Value.isPerformingAction;
    }

}
