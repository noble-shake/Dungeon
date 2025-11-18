using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "DealWithDeathWhenEmblem", story: "[Boss] Deals With Player Death When Emblem", category: "Action", id: "8acd454a7c3500b1b26350679369efef")]
public partial class DealWithDeathWhenEmblemAction : Action
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;

    protected override Status OnStart()
    {
        Boss.Value.bossNetworkManager.currentHp.Value += (int)(0.05f * Boss.Value.bossNetworkManager.maxHp.Value);
        if (Boss.Value.bossNetworkManager.currentHp.Value > Boss.Value.bossNetworkManager.maxHp.Value)
        {
            Boss.Value.bossNetworkManager.currentHp.Value = Boss.Value.bossNetworkManager.maxHp.Value;
        }
        Boss.Value.emblemPattern.emblemSettingReady = false;
        return Status.Success;
    }
}

