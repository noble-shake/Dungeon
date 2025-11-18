using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CheckTornadoPatternSuccedes", story: "[Boss] Checks PatternSuccess [PatternSuccess]", category: "Action", id: "b01ae10965558c4c2ffa9242544e79f1")]
public partial class CheckTornadoPatternSuccedesAction : Action
{

    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;
    [SerializeReference] public BlackboardVariable<bool> PatternSuccess;



    protected override Status OnUpdate()
    {
        // 마법진 조건 달성, 보스 타격 조건 달성 이후 값을 true로 바꿔준다.
        if (Boss.Value.tornadoPattern.IsPatternSuccess == false)
        {
            return Status.Running;
        }
        else if (Boss.Value.tornadoPattern.IsPatternSuccess == true)
        {
            PatternSuccess.Value = true;
            return Status.Success;
        }
        return Status.Failure;
    }

}

