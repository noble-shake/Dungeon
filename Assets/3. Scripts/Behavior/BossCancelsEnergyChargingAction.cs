using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Boss Cancels Energy Charging", story: "[Boss] Cancels Energy Charging", category: "Action", id: "c10911b88b87e266e721dbc3e725b7b0")]
public partial class BossCancelsEnergyChargingAction : Action
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;
    protected override Status OnStart()
    {
        // 회오리 제거
        // 도착선 제거
        // 에너지 차징 제거
        // 
        Boss.Value.tornadoPattern.CleanPatternObjectsServerRpc();
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

