using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Boss1Pattern3MagicCircleAnimatipnPlay", story: "[Self] Play Summon MagicCircle Animation", category: "Action", id: "55fa2c0424b3a2f68541f051a080fe98")]
public partial class Boss1Pattern3MagicCircleAnimatipnPlayAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        Self.Value.GetComponent<WarriorGolemPattern3Component>().isBreakTime = true;
        Self.Value.GetComponent<AIBossCharacterAnimatorManager>().PlayAnimationOnNetwork(AnimationType.Attack, "Boss1_Summon_MagicCircle_Ready", true);
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

