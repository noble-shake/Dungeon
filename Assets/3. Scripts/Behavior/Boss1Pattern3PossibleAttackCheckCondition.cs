using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Boss1Pattern3PossibleAttackCheck", story: "[Self] has Possible Attack, Set Possible Attack", category: "Conditions", id: "3cb112537edc76ce18b9e904a9280cb2")]
public partial class Boss1Pattern3PossibleAttackCheckCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    public override bool IsTrue()
    {
        if (Self.Value.GetComponent<AICharacterManager>().aiCharacterCombatManager.actionRecoveryTime > 0)
            return false;

        AICharacterAttackAction action = Self.Value.GetComponent<AICharacterManager>().aiCharacterCombatManager.GetPossibleAttack();
        if (action != null)
        {
            Self.Value.GetComponent<AICharacterManager>().aiCharacterCombatManager.currentAttack = action;
            return true;
        }
        return false;
    }
}
