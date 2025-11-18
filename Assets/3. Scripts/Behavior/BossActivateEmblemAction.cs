using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BossActivateEmblem", story: "[Boss] State Turns Into Emblem Damageable", category: "Action", id: "1319dd48a769ba782ed3058de805e367")]
public partial class BossActivateEmblemAction : Action
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;

    protected override Status OnStart()
    {
        Boss.Value.emblemPattern.emblemSettingReady = true;
        return Status.Success;
    }
}

