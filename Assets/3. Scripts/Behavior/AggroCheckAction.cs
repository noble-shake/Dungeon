using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.Netcode;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AggroCheck", story: "[Self] Aggro Check For Set [TargetPlayer]", category: "Action", id: "947ad9d5825d9136b38c6fd06ea1ef17")]
public partial class AggroCheckAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> TargetPlayer;
    AIAggroManager manager;

    protected override Status OnStart()
    {
        manager = Self.Value.GetComponent<AIAggroManager>();
        ulong targetPlayerNetworkObjectID = manager.GetTarget();
        if (targetPlayerNetworkObjectID == 123456789UL)
        {
            TargetPlayer.Value = null;
        }
        else
        {
            // Debug.Log("TargetPlayerID" + targetPlayerNetworkObjectID);
            PlayerManager targetPlayer = NetworkManager.Singleton.SpawnManager.SpawnedObjects[targetPlayerNetworkObjectID].GetComponent<PlayerManager>();
            Self.Value.GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("TargetPlayer", targetPlayer);
            Self.Value.GetComponent<AICharacterCombatManager>().currentTarget = targetPlayer;
            TargetPlayer.Value = targetPlayer.gameObject;
        }


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

