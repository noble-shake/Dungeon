using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Boss1Pattern3MoveForChase", story: "[Self] Moves To [TargetPlayer] with [RotationSpeed]", category: "Action", id: "3a3c23c49fb0adac774099f3fb733d31")]
public partial class Boss1Pattern3MoveForChaseAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<PlayerManager> TargetPlayer;
    [SerializeReference] public BlackboardVariable<float> RotationSpeed;

    protected override Status OnStart()
    {
        Self.Value.GetComponent<AICharacterManager>().aiCharacterNetworkManager.isMoving.Value = true;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // 보스 몹을 회전시키는 코드 
        Vector3 AIPosition = Self.Value.GetComponent<AICharacterManager>().transform.position;
        Vector3 TargetPosition = TargetPlayer.Value.transform.position;

        Vector3 dir = TargetPosition - AIPosition;
        dir.y = 0;
        dir.Normalize();
        Quaternion rotation = Quaternion.LookRotation(dir);
        Self.Value.GetComponent<AICharacterManager>().transform.rotation = Quaternion.Slerp(Self.Value.GetComponent<AICharacterManager>().transform.rotation, rotation, RotationSpeed.Value * Time.deltaTime);

        Self.Value.GetComponent<AICharacterManager>().aiCharacterAnimatorManager.UpdateAnimatorMovementParameters(0f, 0.5f, false);
        Self.Value.GetComponent<AICharacterManager>().aiCharacterNetworkManager.animatorVerticalParameter.Value = Self.Value.GetComponent<AICharacterManager>().animator.GetFloat("Vertical");
        return Status.Running;
    }
}

