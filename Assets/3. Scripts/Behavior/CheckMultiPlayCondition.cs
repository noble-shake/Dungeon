using System;
using Unity.Behavior;
using Unity.Netcode;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckMultiPlay", story: "if There are more than 2 Players", category: "Conditions", id: "d3b1f48cb7eda67082d869c749e351b9")]
public partial class CheckMultiPlayCondition : Condition
{

    public override bool IsTrue()
    {
        if (NetworkManager.Singleton.ConnectedClients.Count > 1)
            return true;
        else
            return false;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
