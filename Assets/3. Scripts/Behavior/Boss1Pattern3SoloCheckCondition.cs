using System;
using Unity.Behavior;
using Unity.Netcode;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Boss1Pattern3SoloCheck", story: "[Self] Check Solo Playing", category: "Conditions", id: "555ecf3a050e52f936cab94f7a6a877e")]
public partial class Boss1Pattern3SoloCheckCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

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
