using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Caster ForcePull Animation Play", story: "[Self] , Caster ForcePull Animation Plays", category: "Action", id: "3f2f64479fe7a50ee7e3b9754e62a85a")]
public partial class CasterForcePullAnimationPlayAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    protected override Status OnStart()
    {
        Self.Value.GetComponent<AIBossCharacterAnimatorManager>().PlayAnimationOnNetwork(AnimationType.Attack, "ForcePull", true);

        return Status.Running;
    }
}

