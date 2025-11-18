using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetTargetAndRotate", story: "[Boss] Sets Target And Rotates For rotationTime", category: "Action", id: "5eea726c2d3d3ca70e8de35274236504")]
public partial class SetTargetAndRotateAction : Action
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;
    private float rotationTime;
    private PlayerManager player;

    protected override Status OnStart()
    {
        Boss.Value.swordWavePattern.SetTargetAndRotateServerRpc();
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

