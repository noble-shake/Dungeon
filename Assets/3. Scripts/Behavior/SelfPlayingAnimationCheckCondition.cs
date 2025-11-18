using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "SelfPlayingAnimationCheck", story: "[Self] is Not Playing Animation", category: "Conditions", id: "092691c305311be455f3eb3545b79858")]
public partial class SelfPlayingAnimationCheckCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    public override bool IsTrue()
    {

        return !Self.Value.GetComponent<AICharacterManager>().isPerformingAction;
    }

}
