using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PlayComboAttackAnimation", story: "[AI] Plays Combo Attack Animation", category: "Action", id: "565a298f0d44cf51d09bc49f62bafac7")]
public partial class PlayComboAttackAnimationAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;

    protected override Status OnStart()
    {
        // 콤보를 재실행하지 못하게끔 막음.
        AI.Value.aiCharacterCombatManager.canDoCombo = false;


        AI.Value.aiCharacterCombatManager.actionRecoveryTime = AI.Value.aiCharacterCombatManager.comboAttack.actionRecoveryTime;
        // AI.Value.aiCharacterCombatManager.SetDamageOnCollider(AI.Value.aiCharacterCombatManager.comboAttack.attackDamage);
        AI.Value.aiCharacterCombatManager.comboAttack.AttempToPerformAction(AI.Value);
        AI.Value.aiCharacterCombatManager.comboAttack = AI.Value.aiCharacterCombatManager.comboAttack.comboAction;
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

