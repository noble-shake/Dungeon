using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WaitForEmblemPattern", story: "[Boss] Waits For [Seconds] When Emblem Pattern", category: "Action", id: "0ad9a95e967ac462ce03fd478aa1bbcf")]
public partial class WaitForEmblemPatternAction : Action
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;
    [SerializeReference] public BlackboardVariable<int> Seconds;
    private float timer = 0f;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Boss.Value.emblemPattern.patternStarted == false)
            return Status.Success;
        else
        {
            timer += Time.deltaTime;
            if (Seconds.Value < timer)
                return Status.Success;
            return Status.Running;
        }
    }
}

