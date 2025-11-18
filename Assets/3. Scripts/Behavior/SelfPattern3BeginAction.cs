using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Self Pattern3 Begin", story: "[Self] , Caster Appears with Teleport and Pattern3 begin", category: "Action", id: "00be1bb03e3d23369edf7ffa85dcc05e")]
public partial class SelfPattern3BeginAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        Self.Value.transform.position = GameManager.Instance.GetCenterOfBossRoom();
        // Self.Value.GetComponent<AIBossCharacterNetworkManager>().TeleportDisappearServerRpc(false);
        // Self.Value.GetComponent<AICharacterManager>().characterNetworkManager.isInvulnerable.Value = false;
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

