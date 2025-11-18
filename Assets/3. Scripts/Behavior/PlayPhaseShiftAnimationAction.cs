using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PlayPhaseShiftAnimation", story: "[AI] Plays PhaseShift Animation And Changes Attack List", category: "Action", id: "589b66860846c1ffb3472cb7bd0e30a6")]
public partial class PlayPhaseShiftAnimationAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    protected override Status OnStart()
    {
        (AI.Value as AIBossCharacterManager).bossAnimatorManager.PlayAnimationOnNetwork(AnimationType.Hit, "Phase_Shift_01", true);
        // 1. Boss한테 네트워크배리어블을 줘서 거기 값이 변하면 이 값을 불값을 트루 처리하게 ㄱㄱ ?
        // 2. RPC 함수 만들어서 서버쪽 -> 클라이언트 쪽 거쳐서 해줄거냐 
        // AI.Value.animator.SetBool("SecondPhase", true);
        // (AI.Value as AIBossCharacterManager).bossCharacterCombatManager.ChangeAttackListWithPhaseShift();
        return Status.Success;
    }
}

