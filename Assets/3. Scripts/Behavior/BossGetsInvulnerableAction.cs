using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BossGetsInvulnerable", story: "[Boss] Gets Invulnerable Or Not [True]", category: "Action", id: "f6849cda4c585abf1988f1d70f9da1e5")]
public partial class BossGetsInvulnerableAction : Action
{
    [SerializeReference] public BlackboardVariable<AIBossCharacterManager> Boss;
    [SerializeReference] public BlackboardVariable<bool> True;
    protected override Status OnStart()
    {
        if (True.Value)
        {
            Boss.Value.bossNetworkManager.isInvulnerable.Value = true;
            Boss.Value.bossNetworkManager.isPartiallyInvulnerable.Value = true;
        }
        else
        {
            Boss.Value.bossNetworkManager.isInvulnerable.Value = false;
            Boss.Value.bossNetworkManager.isPartiallyInvulnerable.Value = false;
        }
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

