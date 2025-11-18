using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "InitializeBoss", story: "[Self] Initialize [Boss]", category: "Action", id: "4843faa034ffab8ec4d7e98a0da77611")]
public partial class InitializeBossAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;
    protected override Status OnStart()
    {
        Boss.Value = Self.Value.GetComponent<AIWarriorGolemCharacterManager>();
        Boss.Value.bossNetworkManager.isPartiallyInvulnerable.Value = true;

        Boss.Value.bossNetworkManager.isIgnoringRootmotion.Value = true;

        Boss.Value.bossNetworkManager.isGimmickInProcess = true;

        Boss.Value.swordWavePattern.InitializeByPlayerNumber();
        return Status.Success;
    }

}

