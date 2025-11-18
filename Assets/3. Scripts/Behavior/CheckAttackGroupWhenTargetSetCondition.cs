using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckAttackGroupWhenTargetSet", story: "[AI] , [Target] Conditions Are Met When [AttackGroupValue]", category: "Conditions", id: "3806fd38965ff180b9144a4bf3a38db8")]
public partial class CheckAttackGroupWhenTargetSetCondition : Condition
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    [SerializeReference] public BlackboardVariable<CharacterManager> Target;
    [SerializeReference] public BlackboardVariable<AttackGroup> AttackGroupValue;
    float distance = 0f;

    public override bool IsTrue()
    {
        if (Target.Value == null)
        {
            Debug.Log("타겟이 null입니다.");
            return false;
        }
        if (Target.Value.isDead.Value == true)
        {
            Debug.Log("어그로 끈 녀석이 죽었습니다.");
            return false;
        }
        switch (AttackGroupValue.Value)
        {
            case AttackGroup.None:
                return false;
            // 타겟과의 거리가 
            case AttackGroup.AGroup:
                distance = Vector3.Distance(Target.Value.transform.position, AI.Value.transform.position);
                if (distance <= 8f)
                {
                    AI.Value.aiCharacterCombatManager.currentTarget = Target.Value;
                    return true;
                }
                return false;
            // AI 뒤 쪽 일정거리 이내에 살아있는 플레이어들 중 하나를 랜덤으로 지정한다.
            case AttackGroup.BGroup:
                distance = Vector3.Distance(Target.Value.transform.position, AI.Value.transform.position);
                if (distance < 3f)
                {
                    Vector3 dir = (Target.Value.transform.position - AI.Value.transform.position).normalized;
                    float dot = Vector3.Dot(AI.Value.transform.forward, dir);
                    if (dot < 0f)
                    {
                        AI.Value.aiCharacterCombatManager.currentTarget = Target.Value;
                        return true;
                    }
                }
                return false;
            case AttackGroup.DGroup:

                float distacne = Vector3.Distance(Target.Value.transform.position, AI.Value.transform.position);
                if (8f < distacne)
                {
                    AI.Value.aiCharacterCombatManager.currentTarget = Target.Value;
                    return true;
                }
                return false;
            // 거리가 4 초과인인 플레이어들 중 하나를 랜덤으로 지정한다.
            case AttackGroup.RGroup:
            case AttackGroup.Dash:
                distance = Vector3.Distance(Target.Value.transform.position, AI.Value.transform.position);
                if (distance > 10f)
                {
                    AI.Value.aiCharacterCombatManager.currentTarget = Target.Value;
                    return true;
                }
                return false;
        }
        return false;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
