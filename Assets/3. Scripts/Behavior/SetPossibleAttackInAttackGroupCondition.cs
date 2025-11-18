using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "SetPossibleAttackInAttackGroup", story: "AI has possible specific attack, Set possible [AttackGroup] attack", category: "Conditions", id: "0cc2b2b2ac2195ad17fc61bf08904322")]
public partial class SetPossibleAttackInAttackGroupCondition : Condition
{
    [SerializeReference] public BlackboardVariable<AttackGroup> AttackGroup;

    public override bool IsTrue()
    {
        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
