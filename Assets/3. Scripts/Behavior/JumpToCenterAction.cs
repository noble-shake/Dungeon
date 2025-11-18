using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "JumpToCenter", story: "[Boss] Jumps To Center", category: "Action", id: "db19751c87c4a9ebdd835f044439c38d")]
public partial class JumpToCenterAction : Action
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;
    protected override Status OnStart()
    {
        // RPC로 인한 움직임과 네트워크 변수로 인한 움직임 동기화를 구분짓고, 잠시 한 쪽을 멈출 수 있다면? 
        Boss.Value.swordWavePattern.JumpToCenterServerRpc(Vector3.zero);
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

