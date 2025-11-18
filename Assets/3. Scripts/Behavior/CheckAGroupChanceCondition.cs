using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckAGroupChance", story: "[AI] Should Execute A Group [Probability] %", category: "Conditions", id: "86abd2352be0a5771f02fa0ff1ffb3b3")]
public partial class CheckAGroupChanceCondition : Condition
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    [SerializeReference] public BlackboardVariable<float> Probability;

    public override bool IsTrue()
    {
        // 67%의 확률로 실행 됨.
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
