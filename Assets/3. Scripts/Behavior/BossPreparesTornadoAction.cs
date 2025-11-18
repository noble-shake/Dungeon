using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.Netcode;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BossPreparesTornado", story: "[Boss] Prepares Tornado, Etc.", category: "Action", id: "ea565130d46a3f183706975e557ad2c4")]
public partial class BossPreparesTornadoAction : Action
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;

    protected override Status OnStart()
    {
        int playerCount = NetworkManager.Singleton.ConnectedClients.Count;
        Boss.Value.tornadoPattern.PrepareTornadoPatternServerRpc(playerCount);
        return Status.Success;
    }

}

