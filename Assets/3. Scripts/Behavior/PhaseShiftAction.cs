using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PhaseShiftAction", story: "[Self] Set [State] to [PhaseTypeVariable]", category: "Action", id: "d2d1624ba843253cbdd836aefbc350b9")]
public partial class PhaseShiftAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<EnemyState> State;
    [SerializeReference] public BlackboardVariable<PhaseType> PhaseTypeVariable;
    protected override Status OnStart()
    {
        switch (PhaseTypeVariable.Value)
        {
            default:
            case PhaseType.Phase1:
                Self.Value.GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("State", EnemyState.Pattern1);
                State.Value = EnemyState.Pattern1;
                break;
            case PhaseType.Phase2:
                Self.Value.GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("State", EnemyState.Pattern2);
                State.Value = EnemyState.Pattern2;
                break;
            case PhaseType.Phase3:
                Self.Value.GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("State", EnemyState.Pattern3);
                State.Value = EnemyState.Pattern3;
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

