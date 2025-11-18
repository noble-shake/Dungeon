using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Boss1Pattern3JumpToCenter", story: "[Self] Jumps To Center", category: "Action", id: "47324b34ad1cc5f21700ce050cffd361")]
public partial class Boss1Pattern3JumpToCenterAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        Self.Value.GetComponent<AIWarriorGolemCharacterManager>().swordWavePattern.JumpToCenterServerRpc(GameManager.Instance.GetCenterOfBossRoom());
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

