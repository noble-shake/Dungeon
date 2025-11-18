using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AI Moves To TargetEdge with RotationSpeed", story: "[Self] Moves To [TargetEdge] with [RotationSpeed]", category: "Action", id: "2214db39a2bee2e4cac3aef2addf0952")]
public partial class AiMovesToTargetEdgeWithRotationSpeedAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Vector3> TargetEdge;
    [SerializeReference] public BlackboardVariable<float> RotationSpeed;
    Vector3 OriginPos;

    protected override Status OnStart()
    {
        OriginPos = Self.Value.transform.position;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (OriginPos == null)
        {
            Debug.LogWarning("Illusion Origin Pos Not Set.");
        }

        Vector3 dir = TargetEdge - OriginPos;
        dir.y = 0;
        dir.Normalize();
        Quaternion rotation = Quaternion.LookRotation(dir);
        Self.Value.transform.rotation = Quaternion.Slerp(Self.Value.transform.rotation, rotation, RotationSpeed.Value * Time.deltaTime);

        Self.Value.GetComponent<AICharacterManager>().aiCharacterAnimatorManager.UpdateAnimatorMovementParameters(0f, 0.5f, false);
        Self.Value.GetComponent<AICharacterManager>().aiCharacterNetworkManager.animatorVerticalParameter.Value = Self.Value.GetComponent<AICharacterManager>().animator.GetFloat("Vertical");


        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

