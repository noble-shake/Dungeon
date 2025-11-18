using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Boss1Pattern3GroggyAnim", story: "[Self] Play GroggyAnim", category: "Action", id: "5a58b5be585b61e2a00c74468dc23629")]
public partial class Boss1Pattern3GroggyAnimAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        Self.Value.GetComponent<WarriorGolemPattern3Component>().OnGroggyStart();
        Self.Value.GetComponent<WarriorGolemPattern3Component>().ResetState();
        // Self.Value.GetComponent<WarriorGolemPattern3Component>().ResetAll();
        Self.Value.GetComponent<AIBossCharacterAnimatorManager>().PlayAnimationOnNetwork(AnimationType.Attack, "PhaseBreakStart", true);
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

