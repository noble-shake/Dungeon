using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Boss Gets Soul From Players", story: "[Boss] Gets Soul From Players", category: "Action", id: "55d7a2291c898de72109e001d849d74f")]
public partial class BossGetsSoulFromPlayersAction : Action
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;

    protected override Status OnStart()
    {
        // 플레이어들 캐릭터 모두에서 영혼 모양의의 VFX를 발생시키고 뽑아온다.
        Boss.Value.tornadoPattern.ExtractSoulFromPlayersServerRpc();
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

