using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Sleep", story: "[AI] Sleep", category: "Action", id: "741fe1b39c61811343580b94c220c395")]
public partial class SleepAction : Action
{
    [SerializeReference] public BlackboardVariable<AIBossCharacterManager> AI;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Running;
    }

    protected override void OnEnd()
    {
        // GameManager.Instance.SetCurrentBoss(AI.Value);
        // 
        AI.Value.bossNetworkManager.bossFightIsActive.Value = true;
    }
}

