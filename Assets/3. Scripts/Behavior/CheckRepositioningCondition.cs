using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckRepositioning", story: "[Target] Is Around [AI] In [Distance]", category: "Conditions", id: "fa78baec3c2fd225eb1c51080fe17c8e")]
public partial class CheckRepositioningCondition : Condition
{
    [SerializeReference] public BlackboardVariable<CharacterManager> Target;
    [SerializeReference] public BlackboardVariable<AICharacterManager> AI;
    [SerializeReference] public BlackboardVariable<float> Distance;

    public override bool IsTrue()
    {
        Vector3 distance = Target.Value.transform.position - AI.Value.transform.position;
        float distanceMagnitude = distance.magnitude;
        if (distanceMagnitude > 10f)
        {
            return false;
        }
        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
