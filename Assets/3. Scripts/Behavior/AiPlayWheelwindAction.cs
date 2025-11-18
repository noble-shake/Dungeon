using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.TextCore.Text;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AI Play Wheelwind Action", story: "[Self] Wheelwind Action", category: "Action", id: "5068a12d77daa2378b8f34e32fe8d6b3")]
public partial class AiPlayWheelwindAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    // GameManager

    protected override Status OnStart()
    {
        Self.Value.GetComponent<Boss1illusionAnimatorManager>().OriginPos = Self.Value.transform.position;
        Self.Value.GetComponent<Boss1illusionAnimatorManager>().isProjectileThrow = false;
        Self.Value.GetComponent<AICharacterManager>().aiCharacterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Attack, "GimmikIllusion", true, applyRootmotion: true, attackType: AttackType.HeavyAttack);
        


        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

