using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckCharacterCanRotateWhileAttack", story: "[AI] Can Rotate And Is Playing Attack Animation", category: "Conditions", id: "6a4df7e0316b6c3fca574194fb152d2d")]
public partial class CheckCharacterCanRotateWhileAttack : Condition
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;

    public override bool IsTrue()
    {
        return AI.Value.aiCharacterLocomotionManager.canRotate && AI.Value.isPerformingAction && !AI.Value.animator.GetNextAnimatorStateInfo(2).IsName("Empty");
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
