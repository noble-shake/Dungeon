using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Caster ForcePull Wave Animation Plays", story: "[Self] , Caster ForcePull Wave Animation Plays", category: "Action", id: "927eafa725497eed7ad22577982e730a")]
public partial class CasterForcePullWaveAnimationPlaysAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        GameManager.Instance.SendMessageServerRpc("이것도 한번 피해봐라!");
        Self.Value.GetComponent<AIBossCharacterAnimatorManager>().PlayAnimationOnNetwork(AnimationType.Attack, "Boss1_Force_Ready", true);
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

