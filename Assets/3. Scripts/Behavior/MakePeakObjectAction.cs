using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MakePeakObject", story: "[AI] Generates 2 Peak Obejct", category: "Action", id: "fee1cd4e3ec1bd61d09f38f37d5f3bb6")]
public partial class MakePeakObjectAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    // 찍기 애니메이션 실행
    // 충격파 발생 -> 선택적 
    // 보스 기믹 UI 업데이트
    // 봉우리 3개 생성

    protected override Status OnStart()
    {
        AI.Value.GetComponent<AIBossCharacterAnimatorManager>().PlayAnimationOnNetwork(AnimationType.Attack, "Summon_Stone_Wall", true);
        return Status.Running;
    }

}

