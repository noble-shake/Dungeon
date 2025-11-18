using Cysharp.Threading.Tasks;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Boss1Pattern3TimerOut", story: "[Self] Prison TimerCheck", category: "Conditions", id: "06a34ee5c49bae0fd145b70fcdee375e")]
public partial class Boss1Pattern3TimerOutCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    public override bool IsTrue()
    {
        if (Self.Value.GetComponent<WarriorGolemPattern3Component>().PatternTimer.Value <= 0f)
        {
            GameManager.Instance.SendMessageServerRpc("너무 늦었다!!");
            Self.Value.GetComponent<WarriorGolemPattern3Component>().PatternTimer.Value = 30f;
            Self.Value.GetComponent<WarriorGolemPattern3Component>().ResetState();
            return true;

        }
        else
        {
            return false;
        }
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
