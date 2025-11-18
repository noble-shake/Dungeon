using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckBGroupChance", story: "[AI] Should Execute B Group [BStack] : [Interval_1] [Interval_2]", category: "Conditions", id: "d58f7e6b088e58f5dc7d63599e57a722")]
public partial class CheckBGroupChanceCondition : Condition
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    [SerializeReference] public BlackboardVariable<int> BStack;
    [SerializeReference] public BlackboardVariable<int> Interval_1;
    [SerializeReference] public BlackboardVariable<int> Interval_2;
    public override bool IsTrue()
    {
        int dashStack = BStack.Value;
        int randomValue = UnityEngine.Random.Range(0, 101);
        if (Interval_1.Value <= dashStack && dashStack < Interval_2.Value)
        {
            if (randomValue <= 50)
                return true;
            return false;
        }
        else if (Interval_2.Value <= dashStack)
        {
            if (randomValue <= 100)
                return true;
            return false;
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
