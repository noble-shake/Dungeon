using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.Netcode;
using DTT.AreaOfEffectRegions;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetRushDirection", story: "[AI] Disables Curse Except [Target] And Enables Indicator", category: "Action", id: "d95af294098b4f51ee7a03aeac3e012e")]
public partial class SetRushDirectionAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    [SerializeReference] public BlackboardVariable<Transform> Target;
    float elapsedTime = 0f;
    protected override Status OnStart()
    {
        // 네트워크 상에서 인디케이터 켜주기. 
        // AI.Value.GetComponent<FirstBossGimmickFunc>().indicatorActive.Value = true;

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

