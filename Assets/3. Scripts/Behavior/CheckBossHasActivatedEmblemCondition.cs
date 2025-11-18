using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckBossHasActivatedEmblem", story: "[Boss] Has Activated Emblem", category: "Conditions", id: "350447919df5ca9cfb47afdf4473bcf4")]
public partial class CheckBossHasActivatedEmblemCondition : Condition
{
    [SerializeReference] public BlackboardVariable<AIWarriorGolemCharacterManager> Boss;

    public override bool IsTrue()
    {
        // return Boss.Value.emblemPattern.CheckActivatedEmblem();
        return Boss.Value.emblemPattern.emblemSettingReady;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
