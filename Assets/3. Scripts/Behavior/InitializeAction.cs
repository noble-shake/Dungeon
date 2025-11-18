using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Initialize", story: "Initialize [AI]", category: "Action", id: "484d81ffe5143df0f66b792d56506cab")]
public partial class InitializeAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;

    protected override Status OnStart()
    {
        // isMoving이 현재는 의미 없긴 함. 
        AI.Value.aiCharacterNetworkManager.isMoving.Value = false;

        // 목표가 있었다면 이 목표를 없앤다.
        AI.Value.aiCharacterCombatManager.currentTarget = null;
        // 블랙보드에서도 널을 입력해주기 위해 널 객체 생성.
        PlayerManager nullPlayer = null;

        AI.Value.GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("TargetPlayer", nullPlayer);
        
        return Status.Success;
    }

}

