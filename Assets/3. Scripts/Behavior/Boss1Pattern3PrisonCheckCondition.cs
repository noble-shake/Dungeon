using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Boss1Pattern3PrisonCheck", story: "[Self] will Check [OnPrisonDual]", category: "Conditions", id: "846758a4edba28673b4aa351e88737e7")]
public partial class Boss1Pattern3PrisonCheckCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<bool> OnPrisonDual; // Actually SubGraph Boolean not worked.

    public override bool IsTrue()
    {
        if (OnPrisonDual.Value == true || Self.Value.GetComponent<WarriorGolemPattern3Component>().PrisonPatternPlaying)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
