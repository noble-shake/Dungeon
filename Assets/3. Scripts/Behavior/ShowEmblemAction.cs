using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ShowEmblem", story: "[Boss] Shows Emblem For One Class", category: "Action", id: "87c41e54a44e3601ea07186f19b9d21a")]
public partial class ShowEmblemAction : Action
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;


    protected override Status OnStart()
    {
        Boss.Value.emblemPattern.emblemSettingReady = false;
        Boss.Value.emblemPattern.patternChance--;
        Boss.Value.emblemPattern.PopUpEmblemUI();
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

