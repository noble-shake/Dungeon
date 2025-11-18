using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Pattern3 BuffCount", story: "[Self] , When [OnBuff] activated, as per [BuffTerm]", category: "Action", id: "b2f7b1d5702821719df7efa091484f29")]
public partial class Pattern3BuffCountAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<bool> OnBuff;
    [SerializeReference] public BlackboardVariable<float> BuffTerm;
    private float curTime;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (OnBuff.Value == false)
        {
            return Status.Success;
        }
        else
        {
            curTime -= Time.deltaTime;
            if (curTime < 0f)
            {
                curTime = BuffTerm.Value;


            }


            return Status.Running;
        }


    }

    protected override void OnEnd()
    {
    }
}

