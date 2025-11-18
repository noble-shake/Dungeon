using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckSealedPlayer", story: "No Sealed [Player]", category: "Conditions", id: "b1072c1b46b489ec2d9def33cc2a4caf")]
public partial class CheckSealedPlayerCondition : Condition
{
    [SerializeReference] public BlackboardVariable<PlayerManager> Player;

    public override bool IsTrue()
    {
        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
