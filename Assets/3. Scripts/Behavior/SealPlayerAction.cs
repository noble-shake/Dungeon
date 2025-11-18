using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SealPlayer", story: "Seal [Player] Randomly", category: "Action", id: "6f3cc25d0fdece9b86666f2dbe54ac8d")]
public partial class SealPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<PlayerManager> Player;

    // 봉인된 플레이어는 움직일 수 없다.
    // 다른 플레이어가 일정 횟수 이상(혹은 무력화 수치) 타격해야 봉인이 풀린다.

    protected override Status OnStart()
    {
        Player.Value = GameManager.Instance.GetRandomPlayer();
        Debug.Log(Player.Value.name + "을 봉인합니다.");
        Debug.Log("봉인 로직 실행");
        // Player.Value.playerCombatManager.GetSealedServerRpc();

        return Status.Running;
    }

}

