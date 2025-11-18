using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using TeleportFX;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AIAppearsAtCenter", story: "[AI] Appears At BossRoomCenter With Teleport And Gets Disinvulnerable", category: "Action", id: "76a8661fc99f529c4a7a48629919f988")]
public partial class AiAppearsAtCenterAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    protected override Status OnStart()
    {
        AI.Value.transform.position = GameManager.Instance.GetCenterOfBossRoom();
        // AI.Value.GetComponent<AIBossCharacterNetworkManager>().TeleportDisappearServerRpc(false);
        AI.Value.characterNetworkManager.isInvulnerable.Value = false;
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

