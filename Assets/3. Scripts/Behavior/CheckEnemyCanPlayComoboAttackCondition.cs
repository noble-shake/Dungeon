using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckEnemyCanPlayComoboAttack", story: "[AI] Can Play Combo", category: "Conditions", id: "59ca2a74bfa9453c12c63542872797ba")]
public partial class CheckEnemyCanPlayComoboAttackCondition : Condition
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;

    public override bool IsTrue()
    {
        return AI.Value.aiCharacterCombatManager.canDoCombo && AI.Value.aiCharacterCombatManager.comboAttack != null;
    }


}
