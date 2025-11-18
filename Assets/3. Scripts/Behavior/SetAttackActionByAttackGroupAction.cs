using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetAttackActionByAttackGroup", story: "[AI] Sets Attack Action With [AttackGroup]", category: "Action", id: "515891eaff920ab80f478ec503d8fdd2")]
public partial class SetAttackActionByAttackGroupAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    [SerializeReference] public BlackboardVariable<AttackGroup> AttackGroup;
    protected override Status OnStart()
    {
        AI.Value.aiCharacterCombatManager.SetCurrentActionByGroup(AttackGroup.Value);
        return Status.Success;
    }

}

