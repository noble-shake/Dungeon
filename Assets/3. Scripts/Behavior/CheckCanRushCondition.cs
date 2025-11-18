using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckCanRush", story: "[RushCount] Is More Than 0", category: "Conditions", id: "469182bdaee3697d9f0013f0aa9fddb6")]
public partial class CheckCanRushCondition : Condition
{
    [SerializeReference] public BlackboardVariable<int> RushCount;

    public override bool IsTrue()
    {
        if (RushCount.Value > 0)
        {
            // 왜 여기가 2번 되지? 
            Debug.Log("러쉬 카운트 계산");

            // RushCount.Value--;
            return true;
        }
        return false;


    }

}
