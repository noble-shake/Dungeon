using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PopUpSwordWaveUI", story: "[Boss] Pops Up Pattern UI", category: "Action", id: "60f25496694a954c8fb36d29824fb47d")]
public partial class PopUpSwordWaveUiAction : Action
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;

    protected override Status OnStart()
    {
        Boss.Value.swordWavePattern.SetIconServerRpc();
        return Status.Success;
    }
}

