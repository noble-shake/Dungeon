using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckTornadoPatternEnds", story: "[Boss] Checks If Tornado Pattern Ended", category: "Conditions", id: "1a832ae2093746dcde5728c20915ec9e")]
public partial class CheckTornadoPatternEndsCondition : Condition
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;

    public override bool IsTrue()
    {
        if (Boss.Value.tornadoPattern.tornadoCount > 0)
        {
            return false;
        }
        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
