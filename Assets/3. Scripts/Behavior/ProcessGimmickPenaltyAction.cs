using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ProcessGimmickPenalty", story: "[Boss] Executes Sudden Death Penalty", category: "Action", id: "2dbb6de43d875e7f62287e33bca36280")]
public partial class ProcessGimmickPenaltyAction : Action
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;

    protected override Status OnStart()
    {
        Boss.Value.swordWavePattern.AllKillServerRpc();
        return Status.Success;
    }

}

