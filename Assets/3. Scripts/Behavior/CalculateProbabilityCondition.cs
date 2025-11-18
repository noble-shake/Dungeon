using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CalculateProbability", story: "Chance is [Probability] %", category: "Conditions", id: "ddac2abfd96755f14bfe8f9e536c47ed")]
public partial class CalculateProbabilityCondition : Condition
{
    [SerializeReference] public BlackboardVariable<int> Probability;
    private int randomProbability;

    public override bool IsTrue()
    {
        randomProbability = UnityEngine.Random.Range(0, 101);
        if (randomProbability <= Probability.Value)
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
