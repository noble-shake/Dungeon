using System;
using Unity.Behavior;
using Unity.Netcode;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckAlivePlayers", story: "There Are Alive Players", category: "Conditions", id: "9a5f403f7fd9b35c53a97ac058e258db")]
public partial class CheckAlivePlayersCondition : Condition
{

    public override bool IsTrue()
    {
        foreach (var networkObject in NetworkManager.Singleton.SpawnManager.SpawnedObjects)
        {
            if (networkObject.Value.TryGetComponent(out PlayerManager player))
            {
                if (player.isDead.Value == false && player.IsSpawned)
                {
                    return true;
                }
            }
        }
        return false;

        // foreach (PlayerManager player in GameManager.Instance.connectedPlayerList)
        // {
        //     if (player != null && !player.isDead.Value && player.IsSpawned)
        //     {
        //         Debug.Log("플레이어가 캐릭터가 스폰됐고 살아있습니다.");
        //         return true;
        //     }
        // }
        // return false;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
