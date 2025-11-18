using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using TeleportFX;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "StartTeleport", story: "[AI] Starts Teleport And Gets Invulnerable", category: "Action", id: "8ea2f08ef7685cd8dc8493ba9c7ad78d")]
public partial class StartTeleportAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;

    protected override Status OnStart()
    {
        // 걸음을 멈춤. 
        AI.Value.animator.SetFloat("Vertical", 0);
        AI.Value.characterNetworkManager.animatorVerticalParameter.Value = 0f;
        AI.Value.characterNetworkManager.isInvulnerable.Value = true;


        // AI.Value.GetComponent<AIBossCharacterNetworkManager>().TeleportDisappearServerRpc(true);
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
            GameManager.Instance.SendMessageServerRpc("건방지기 짝이 없는 녀석들");
            return Status.Success;
        }
    }

    protected override void OnEnd()
    {

    }
}

