using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckDashChance", story: "[AI] Should Execute Dash With [DashStack] : [Interval_1] [Interval_2] [Interval_3]", category: "Conditions", id: "9cfef751d0fa889fbf41416745830349")]
public partial class CheckDashChanceCondition : Condition
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    [SerializeReference] public BlackboardVariable<int> DashStack;
    [SerializeReference] public BlackboardVariable<int> Interval_1;
    [SerializeReference] public BlackboardVariable<int> Interval_2;
    [SerializeReference] public BlackboardVariable<int> Interval_3;


    public override bool IsTrue()
    {
        int dashStack = DashStack.Value;
        int randomValue = UnityEngine.Random.Range(0, 101);
        if (Interval_1.Value <= dashStack && dashStack < Interval_2.Value)
        {
            if (randomValue <= 33)
                return true;
            return false;
        }
        else if (Interval_2.Value <= dashStack && dashStack < Interval_3.Value)
        {
            if (randomValue <= 66)
                return true;
            return false;
        }
        else if (Interval_3.Value <= dashStack)
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
