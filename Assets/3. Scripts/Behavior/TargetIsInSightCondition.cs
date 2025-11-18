using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Target is in sight", story: "Target is in sight of AI, Set Target", category: "Conditions", id: "3506e8aed144374556a693a56ff1a6e1")]
public partial class TargetIsInSightCondition : Condition
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

    public override void OnStart()
    {

    }

    public override void OnEnd()
    {

    }
}
