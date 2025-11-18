using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CalculateAggro", story: "Calculate Aggro", category: "Action", id: "4509ad149b3b3dde33059e9cfdbcd738")]
public partial class CalculateAggroAction : Action
{

    protected override Status OnStart()
    {
        // Target Aggro Value.

        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        //Debug.Log("도달 했는가?");
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

