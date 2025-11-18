using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Boss1Pattern3PrisonDualTargetCheck", story: "[Self] Aggro Always to [TargetPlayer]", category: "Action", id: "27908dd4793fae4cb9b53b70faf4bb60")]
public partial class Boss1Pattern3PrisonDualTargetCheckAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> TargetPlayer;

    protected override Status OnStart()
    {
        TargetPlayer.Value = Self.Value.GetComponent<AIBossCharacterCombatManager>().currentTarget.gameObject;
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

