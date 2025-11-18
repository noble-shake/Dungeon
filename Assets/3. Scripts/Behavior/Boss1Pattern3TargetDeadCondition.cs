using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Boss1Pattern3TargetDead", story: "If [TargetPlayer] is Dead", category: "Conditions", id: "d14fd08611015a02cb39186a9a1b069a")]
public partial class Boss1Pattern3TargetDeadCondition : Condition
{
    [SerializeReference] public BlackboardVariable<PlayerManager> TargetPlayer;

    public override bool IsTrue()
    {
        if (TargetPlayer.Value.isDead.Value == true)
        {
            GameManager.Instance.SendMessageServerRpc("다른 놈을 데려와라!!");
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
