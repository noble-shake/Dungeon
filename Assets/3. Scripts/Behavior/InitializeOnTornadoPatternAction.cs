using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "InitializeOnTornadoPattern", story: "[Self] Initializes [Boss]", category: "Action", id: "004a0d8b438a8dead9477a55cee4fd5d")]
public partial class InitializeOnTornadoPatternAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;

    protected override Status OnStart()
    {
        Boss.Value = Self.Value.GetComponent<AIWarriorGolemCharacterManager>();
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

