using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Boss1Pattern3PrisonBegin", story: "[Self] Summon Prison with Pattern3 Begin", category: "Action", id: "385e5458ab6da9b56dca48002f331ff6")]
public partial class Boss1Pattern3PrisonBeginAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        // Boss1_Summon_Prison_Ready
        Self.Value.GetComponent<AIBossCharacterAnimatorManager>().PlayAnimationOnNetwork(AnimationType.Attack, "Boss1_Summon_Prison_Ready", true);
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

