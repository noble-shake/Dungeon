using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "InitializeRushTarget", story: "[AI] Sets [Target] Except For [SealedPlayer]", category: "Action", id: "7a6892d465e0d3e3ca01af978d2447e4")]
public partial class InitializeRushTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    [SerializeReference] public BlackboardVariable<Transform> Target;
    [SerializeReference] public BlackboardVariable<PlayerManager> SealedPlayer;
    protected override Status OnStart()
    {
        // 
        Target.Value = GameManager.Instance.GetRandomPlayer().transform;


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

