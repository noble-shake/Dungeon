using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BossMakesHpPenalty", story: "[Boss] Implements Hp Limit Penalty [HpLimit]", category: "Action", id: "4d7b69bc838ce1d9af731d49bc2027f4")]
public partial class BossMakesHpPenaltyAction : Action
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;
    [SerializeReference] public BlackboardVariable<float> HpLimit;
    protected override Status OnStart()
    {
        Boss.Value.tornadoPattern.SetHpPenalty(HpLimit.Value);
        return Status.Success;
    }
}

