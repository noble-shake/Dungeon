using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TeleportPattern3", story: "[Self] , Caster Starts Teleport And Gets Invulable", category: "Action", id: "622800c98cf50df43ead27d6d2c89d20")]
public partial class TeleportPattern3Action : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    protected override Status OnStart()
    {
        AICharacterManager Caster = Self.Value.GetComponent<AICharacterManager>();
        Caster.animator.SetFloat("Vertical", 0);
        Caster.characterNetworkManager.animatorVerticalParameter.Value = 0f;

        Caster.characterNetworkManager.isInvulnerable.Value = true;
        Self.Value.GetComponent<AIWarriorGolemCharacterManager>().swordWavePattern.JumpToCenterServerRpc(GameManager.Instance.GetCenterOfBossRoom());
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (GameManager.Instance == null)
        {
            return Status.Running;
        }
        else
        {
            GameManager.Instance.SendMessageServerRpc("아직 끝난게 아니다!!");
            return Status.Success;
        }
    }

    protected override void OnEnd()
    {
    }
}

