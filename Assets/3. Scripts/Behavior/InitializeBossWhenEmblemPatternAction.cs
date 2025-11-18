using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "InitializeBossWhenEmblemPattern", story: "[Self] Initialize [Boss] For Emblem Pattern", category: "Action", id: "ba1849cc70560d5fbd6354cadea82b76")]
public partial class InitializeBossWhenEmblemPatternAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;
    protected override Status OnStart()
    {
        Boss.Value = Self.Value.GetComponent<AIWarriorGolemCharacterManager>();

        Boss.Value.bossNetworkManager.isGimmickInProcess = true;

        Boss.Value.emblemPattern.StartPatternServerRpc();

        return Status.Success;
    }

}

