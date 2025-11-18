using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetImprovedState", story: "[AI] Set [ImprovedEnemyState] To [PhaseTypeVariable]", category: "Action", id: "0e62d2309ab6cc27efe2bfe998e724cf")]
public partial class SetImprovedStateAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    [SerializeReference] public BlackboardVariable<ImprovedEnemyState> ImprovedEnemyState;
    [SerializeReference] public BlackboardVariable<PhaseType> PhaseTypeVariable;
    protected override Status OnStart()
    {
        switch (PhaseTypeVariable.Value)
        {
            case PhaseType.Phase1:
                // AI.Value.GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("ImprovedEnemyState", global::ImprovedEnemyState.FirstPattern);
                ImprovedEnemyState.Value = global::ImprovedEnemyState.FirstPattern;
                break;
            case PhaseType.Phase2:
                // AI.Value.GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("ImprovedEnemyState", global::ImprovedEnemyState.SecondPattern);
                ImprovedEnemyState.Value = global::ImprovedEnemyState.SecondPattern;
                break;
            case PhaseType.Phase3:
                // AI.Value.GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("ImprovedEnemyState", global::ImprovedEnemyState.ThirdPattern);
                ImprovedEnemyState.Value = global::ImprovedEnemyState.ThirdPattern;
                break;
        }

        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

