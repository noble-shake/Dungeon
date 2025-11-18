using System;
using Unity.Behavior;
using Unity.Netcode;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckNeedToSeal", story: "If There Is Need To Seal Player", category: "Conditions", id: "69a847a4ab75db9399334bf82aee2d6b")]
public partial class CheckNeedToSealCondition : Condition
{

    public override bool IsTrue()
    {
        return false;
        // if (NetworkManager.Singleton.ConnectedClients.Count > 1)
        //     return true;
        // else
        //     return false;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
