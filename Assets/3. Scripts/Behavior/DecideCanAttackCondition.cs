using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "DecideCanAttackAndSetAttack", story: "[AI] has possible attack, Set Possible Attack", category: "Conditions", id: "45e71f51fab0a638037ccf97489fe32a")]
public partial class DecideCanAttackCondition : Condition
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;

    public override bool IsTrue()
    {
        if (AI.Value.aiCharacterCombatManager.actionRecoveryTime > 0)
            return false;

        AICharacterAttackAction action = AI.Value.aiCharacterCombatManager.GetPossibleAttack();
        if (action != null)
        {
            AI.Value.aiCharacterCombatManager.currentAttack = action;
            // AI.Value.GetComponent<RushUntilCollision>().StopAction();
            return true;
        }
        return false;
    }
}
