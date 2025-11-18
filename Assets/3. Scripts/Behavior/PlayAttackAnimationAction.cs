using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PlayAttackAnimation", story: "[AI] Plays Attack Animation", category: "Action", id: "57a07834554866c184301d6fbfa45ae3")]
public partial class PlayAttackAnimationAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;

    protected override Status OnStart()
    {
        AI.Value.aiCharacterAnimatorManager.StopMovement();
        AI.Value.aiCharacterCombatManager.actionRecoveryTime = AI.Value.aiCharacterCombatManager.currentAttack.actionRecoveryTime;
        AI.Value.aiCharacterCombatManager.currentAttack.AttempToPerformAction(AI.Value);
        return Status.Success;
    }


}

