using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Boss1Pattern3CircleBreakBegin", story: "[Self] Start Teleport To Middle", category: "Action", id: "fa03a6f0bf8bd38de4151f0c02f37590")]
public partial class Boss1Pattern3CircleBreakBeginAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        AICharacterManager Caster = Self.Value.GetComponent<AICharacterManager>();
        Caster.animator.SetFloat("Vertical", 0);
        Caster.characterNetworkManager.animatorVerticalParameter.Value = 0f;
        Caster.characterNetworkManager.isInvulnerable.Value = false;

        Self.Value.GetComponent<AIWarriorGolemCharacterManager>().swordWavePattern.JumpToCenterServerRpc(GameManager.Instance.GetCenterOfBossRoom());
        return Status.Running;
    }

}

