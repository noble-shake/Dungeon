using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BossWathcesEmblemPlayerDie", story: "[Boss] Watches Emblem Player Die", category: "Action", id: "cd2514fbd60320f6a1a9ad09a8dad925")]
public partial class BossWathcesEmblemPlayerDieAction : Action
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Boss.Value.emblemPattern.selectedPlayer.isDead.Value)
        {
            return Status.Success;
        }
        else
            return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

