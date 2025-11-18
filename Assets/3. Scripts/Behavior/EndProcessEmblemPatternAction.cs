using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "EndProcessEmblemPattern", story: "[Boss] Ends Emblem Pattern", category: "Action", id: "36d663e7c4e9600730133fe21116f827")]
public partial class EndProcessEmblemPatternAction : Action
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;

    protected override Status OnStart()
    {
        Boss.Value.emblemPattern.EndPatterServerRpc();
        return Status.Success;
    }

}

