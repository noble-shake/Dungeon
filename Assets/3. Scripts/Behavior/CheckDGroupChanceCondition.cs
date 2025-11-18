using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckDGroupChance", story: "[AI] Should Execute D Group [Probability] %", category: "Conditions", id: "2146b668246dbe3eeb1657e9be6be126")]
public partial class CheckDGroupChanceCondition : Condition
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    [SerializeReference] public BlackboardVariable<float> Probability;

    public override bool IsTrue()
    {
        int randomValue = UnityEngine.Random.Range(0, 101);
        if (randomValue <= Probability.Value)
        {
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
