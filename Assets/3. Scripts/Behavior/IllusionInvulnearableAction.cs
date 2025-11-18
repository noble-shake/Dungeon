using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "IllusionInvulnearable", story: "[Self] Invulnearable Check", category: "Action", id: "53904c07d3e36cd837a6ba7979811f60")]
public partial class IllusionInvulnearableAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        int counter;
        if (Self.Value.GetComponent<BehaviorGraphAgent>().BlackboardReference.GetVariableValue("EdgeCounter", out counter))
        {
            if (counter == 3) // before Last Move.
            {

                // Breakable State.

            }
        }
        Self.Value.GetComponent<AIBossCharacterCombatManager>().EnableIsInvulnerable();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

