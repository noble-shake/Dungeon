using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using TeleportFX;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Boss1Pattern3disappearAction", story: "[TargetPlayer] Disappear Forever", category: "Action", id: "1074d000628bd1d6d6de54b529b142fe")]
public partial class Boss1Pattern3DisappearAction : Action
{
    [SerializeReference] public BlackboardVariable<PlayerManager> TargetPlayer;

    protected override Status OnStart()
    {
        TargetPlayer.Value.DisappearEffectServerRpc();


        

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

