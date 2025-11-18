using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Players is Sealed", story: "[SealedPlayer] Is Sealed", category: "Conditions", id: "4f890bd10e86bf60c65b24cdaa4bce91")]
public partial class PlayersIsSealedCondition : Condition
{
    [SerializeReference] public BlackboardVariable<PlayerManager> SealedPlayer;

    public override bool IsTrue()
    {
        if (SealedPlayer.Value.playerNetworkManager.isSealed.Value)
            return true;
        return false;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
