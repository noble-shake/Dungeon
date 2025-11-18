using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.Netcode;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FindPlayers", story: "CurrentPlayerCheck", category: "Action", id: "2d30787b2b2df079a035e6f431df0b36")]
public partial class FindPlayersAction : Action
{

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        //PlayerManager player = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerObjectID].GetComponent<PlayerManager>();


        foreach (ulong playerObjectID in NetworkManager.Singleton.SpawnManager.SpawnedObjects.Keys)
        {
            PlayerManager player = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerObjectID].GetComponent<PlayerManager>();

            foreach (var networkobject in NetworkManager.Singleton.SpawnManager.SpawnedObjects)
            {
                if (networkobject.Value.GetComponent<PlayerManager>() != null)
                {
                    Debug.Log("Player Check");
                    Debug.Log(playerObjectID);
                    Debug.Log(networkobject.Value.GetComponent<CharacterNetworkManager>().currentHp.Value);
                }
            }
        }


        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

