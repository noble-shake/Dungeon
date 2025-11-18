using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.Netcode;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SealPlayer2", story: "Seal Player With [Message]", category: "Action", id: "9f7cf0ce7fe43aa50e9b59061884a1e0")]
public partial class SealPlayer2Action : Action
{
    [SerializeReference] public BlackboardVariable<string> Message;
    protected override Status OnStart()
    {
        int num = NetworkManager.Singleton.ConnectedClientsList.Count;
        switch (num)
        {
            case 1:
                num = 1;
                break;
            case 2:
            case 3:
                num = 1;
                break;
            case 4:
                num = 2;
                break;
        }

        int temp = 0;
        while (num > 0)
        {
            temp++;
            if (temp == 5)
            {
                break;
            }
            PlayerManager player = GameManager.Instance.GetRandomPlayer();
            if (player.playerNetworkManager.isSealed.Value)
                continue;
            // 플레이어가 호스트가 아니라면 봉인하지 않는다.
            // if (!player.IsHost)
            //     continue;
            num--;
            // 네트워크 변수의 권한을 서버로 바꿨음. -> 즉각적으로 서버가 누가 봉인되었는지 체크하고, 봉인 안 된 사람을 고르기 위함.
            player.playerNetworkManager.isSealed.Value = true;

            Debug.Log("클라이언트 번호 " + player.OwnerClientId + " 봉인합니다.");
            player.playerCombatManager.GetSealedServerRpc(Message.Value);
            Debug.Log("클라이언트 번호 " + player.OwnerClientId + " 봉인합니다.");
        }
        return Status.Success;
    }

}

