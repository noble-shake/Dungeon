using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Collections.Generic;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GenerateCandle", story: "[AI] Summon BossIllusion Generate [CandleGenList] in [PentagramGroup]", category: "Action", id: "efe0408ac51ae92e67dd91d178e50630")]
public partial class GenerateCandleAction : Action
{
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    [SerializeReference] public BlackboardVariable<List<Vector3>> CandleGenList;
    [SerializeReference] public BlackboardVariable<GameObject> PentagramGroup;
    protected override Status OnStart()
    {
        AI.Value.GetComponent<AIBossCharacterAnimatorManager>().PlayAnimationOnNetwork(AnimationType.Attack, "Summon_Candle_And_Illusion", true);

        //return Status.Running;
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

