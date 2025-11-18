using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckRGroupChance", story: "[AI] Should Execute R Group [RStack] : [Interval_1] [Interval_2] [Interval_3]", category: "Conditions", id: "747566f815ed92f67066b3d0de4e10bb")]
public partial class CheckRGroupChanceCondition : Condition
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    [SerializeReference] public BlackboardVariable<int> RStack;
    [SerializeReference] public BlackboardVariable<int> Interval_1;
    [SerializeReference] public BlackboardVariable<int> Interval_2;
    [SerializeReference] public BlackboardVariable<int> Interval_3;
    public override bool IsTrue()
    {
        int stack = RStack.Value;
        int randomValue = UnityEngine.Random.Range(0, 101);
        if (Interval_1.Value <= stack && stack < Interval_2.Value)
        {
            if (randomValue <= 33)
                return true;
            return false;
        }
        else if (Interval_2.Value <= stack && stack < Interval_3.Value)
        {
            if (randomValue <= 66)
                return true;
            return false;
        }
        else if (Interval_3.Value <= stack)
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
