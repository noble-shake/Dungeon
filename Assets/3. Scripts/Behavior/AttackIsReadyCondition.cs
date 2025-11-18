using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Attack Is Ready", story: "[AI] is Ready To Play Attack Action", category: "Conditions", id: "c8a852ed279e3f8fc2eb6e9f01a9a3f7")]
public partial class AttackIsReadyCondition : Condition
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;

    public override bool IsTrue()
    {
        AICharacterAttackAction action = AI.Value.aiCharacterCombatManager.currentAttack;
        // 
        if (action.minimumAttackDistance <= AI.Value.aiCharacterCombatManager.distanceFromTarget
            && action.maximumAttackDistance >= AI.Value.aiCharacterCombatManager.distanceFromTarget
            && action.minimumAttackAngle <= AI.Value.aiCharacterCombatManager.viewableAngle
            && action.maximumAttackAngle >= AI.Value.aiCharacterCombatManager.viewableAngle)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
