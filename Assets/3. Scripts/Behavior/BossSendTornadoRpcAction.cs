using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BossSendTornadoRpc", story: "[Boss] Sends Rpc To Make Tornado Launched", category: "Action", id: "39c47555668698252e735458e52e0c48")]
public partial class BossSendTornadoRpcAction : Action
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;
    protected override Status OnStart()
    {
        // 무조건 3초 안에 끝남. 
        Boss.Value.tornadoPattern.GenerateAndLaunchTornadoServerRpc();
        Boss.Value.tornadoPattern.tornadoCount--;
        return Status.Success;
    }

}

