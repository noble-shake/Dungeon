using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Test", story: "Do Nothing", category: "Action", id: "705a2836d2689b3a2eb636c6939cd908")]
public partial class TestAction : Action
{

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {

        return Status.Running;
    }

    protected override void OnEnd()
    {
        // 게임매니저에게 일러서 보스 UI 띄우기, 보스 UI와 해당 보스몹 체력 연결 
        // GameManager.Instance.SetCurrentBoss();
    }
}

