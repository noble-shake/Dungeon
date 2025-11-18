using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Boss3Pattern3Invulnearable", story: "[Self] Boss Invulnearable", category: "Action", id: "11e21215e0d76064ed4610da56a08825")]
public partial class Boss3Pattern3InvulnearableAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Self.Value.GetComponent<WarriorGolemPattern3Component>().PrisonPatternPlaying)
        {
            Self.Value.GetComponent<AIBossCharacterCombatManager>().DisableIsInvulnerable();
        }
        else
        {
            Self.Value.GetComponent<AIBossCharacterCombatManager>().EnableIsInvulnerable();
        }


        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

