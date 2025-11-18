using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PlaySwordWaveAnimation", story: "[Boss] Plays SwordWave Animations", category: "Action", id: "65e01aa27248eb71c121ff28c2a727e5")]
public partial class PlaySwordWaveAnimationAction : Action
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;
    protected override Status OnStart()
    {
        Boss.Value.swordWavePattern.PlaySwordWaveAnimation();
        Boss.Value.swordWavePattern.SwordWaveCount--;
        return Status.Success;
    }

}

