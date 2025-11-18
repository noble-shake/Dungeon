using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckTargetInSight", story: "Target Is In Sight Of [AI] , Set Target", category: "Conditions", id: "8838c2385ff68d67e215ee493f99a2e3")]
public partial class CheckTargetInSightCondition : Condition
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;

    public override bool IsTrue()
    {
         AI.Value.aiCharacterCombatManager.FindTargetViaLineOfSight(AI.Value);
        if (AI.Value.aiCharacterCombatManager.IsTargetFound())
        {
            AI.Value.GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("TargetPlayer", AI.Value.aiCharacterCombatManager.currentTarget);
            return true;
        }
        return false;
    }
}
