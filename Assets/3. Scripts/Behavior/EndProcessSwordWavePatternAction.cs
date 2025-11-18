using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "EndProcessSwordWavePattern", story: "[Boss] Ends Sword Wave Pattern", category: "Action", id: "40f82012714305a80ad0ede71a7743bd")]
public partial class EndProcessSwordWavePatternAction : Action
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;

    protected override Status OnStart()
    {
        Boss.Value.bossNetworkManager.isIgnoringRootmotion.Value = false;
        Boss.Value.bossNetworkManager.isPartiallyInvulnerable.Value = false;
        Boss.Value.bossNetworkManager.isGimmickInProcess = false;
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

