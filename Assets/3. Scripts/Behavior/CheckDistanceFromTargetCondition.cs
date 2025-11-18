using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckDistanceFromTarget", story: "[AI] Is [ChasingDistance] Far From Target Player", category: "Conditions", id: "98373107cf9d51d9ea032557a68d4548")]
public partial class CheckDistanceFromTargetCondition : Condition
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    [SerializeReference] public BlackboardVariable<float> ChasingDistance;

    public override bool IsTrue()
    {
        if (AI.Value.aiCharacterCombatManager.distanceFromTarget > ChasingDistance)
        {
            return true;
        }

        return false;
    }
}
