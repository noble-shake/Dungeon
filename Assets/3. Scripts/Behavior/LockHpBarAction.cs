using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "LockHpBar", story: "[Boss] Set Lock On Hp Bar", category: "Action", id: "1f3df461ec6c2b7030992fc595037c7e")]
public partial class LockHpBarAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> Boss;

    protected override Status OnStart()
    {
        HUD_UIManager.instance.combatUIManager.SetLockOnHpBar(true);
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

