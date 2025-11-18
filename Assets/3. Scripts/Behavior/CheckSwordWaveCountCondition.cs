using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckSwordWaveCount", story: "[Boss] Threw SwordWave Over Limit", category: "Conditions", id: "494cc964c7a9c8af25255a85092f9783")]
public partial class CheckSwordWaveCountCondition : Condition
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;

    public override bool IsTrue()
    {
        int count = Boss.Value.swordWavePattern.SwordWaveCount;
        if (count == 0)
            return true;
        return false;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
