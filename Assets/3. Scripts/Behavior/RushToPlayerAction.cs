using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using DTT.AreaOfEffectRegions;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RushToPlayer", story: "[AI] Rushes To [RushTarget] Until Collision", category: "Action", id: "90df7b1753622df8fe76ef754e87e5f2")]
public partial class RushToPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    [SerializeReference] public BlackboardVariable<Transform> RushTarget;
    private Vector3 dir;

    protected override Status OnStart()
    {
        // 인디케이터 끄기
        AI.Value.GetComponent<FirstBossGimmickFunc>().indicatorActive.Value = false;


        // 러시 애니메이션 실행 하고

        // 근데 그 전에 살짝 걸어가야 됨. 

        AI.Value.aiCharacterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Attack, "Rush", true);
        dir = (RushTarget.Value.transform.position - AI.Value.transform.position).normalized;
        dir.y = 0;

        AI.Value.GetComponent<RushUntilCollision>().RushAfterInitialization(dir);
        AI.Value.GetComponent<RushUntilCollision>().OnRush = true;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (AI.Value.GetComponent<RushUntilCollision>().HasCollision)
        {
            return Status.Success;
        }
        // AI.Value.characterController.Move(dir * RushSpeed.Value * Time.deltaTime);
        return Status.Running;
    }

    protected override void OnEnd()
    {
        AI.Value.GetComponent<RushUntilCollision>().HasCollision = false;
    }
}

