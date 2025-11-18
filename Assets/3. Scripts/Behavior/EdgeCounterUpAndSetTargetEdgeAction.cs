using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "EdgeCounterUpAndSetTargetEdge", story: "[Self] count up [EdgeCounter] And Set [TargetEdge] From [EdgeList]", category: "Action", id: "44be30c589fc29578a287eeae2a5b1ab")]
public partial class EdgeCounterUpAndSetTargetEdgeAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<int> EdgeCounter;
    [SerializeReference] public BlackboardVariable<Vector3> TargetEdge;
    [SerializeReference] public BlackboardVariable<List<Vector3>> EdgeList;

    protected override Status OnStart()
    {
        int TargetIdx = 0;
        switch (EdgeCounter)
        {
            default:
            case 0:
                TargetIdx = 2;
                break;
            case 1:
                TargetIdx = 4;
                break;
            case 2:
                TargetIdx = 1;
                break;
            case 3:
                TargetIdx = 3;
                break;
            case 4:
                TargetIdx = 0;
                break;
        }

        Debug.Log($"EdgeList : {TargetIdx} ");
        TargetEdge.Value = EdgeList.Value[TargetIdx];
        EdgeCounter.Value++;
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

