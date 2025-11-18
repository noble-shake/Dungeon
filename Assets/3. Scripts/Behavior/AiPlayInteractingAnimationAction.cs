using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AIPlayInteractingAnimation", story: "[Boss] Plays Charging Energy Animaiton", category: "Action", id: "5885d854d865f644b55672ceca3d0875")]
public partial class AiPlayInteractingAnimationAction : Action
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;

    protected override Status OnStart()
    {
        Boss.Value.bossAnimatorManager.PlayAnimationOnNetwork(AnimationType.Etc, "Boss_EnergyCharging_Start", true, applyRootmotion: true);
        Boss.Value.tornadoPattern.StartChargingEnergyServerRpc();
        return Status.Success;
    }


}

