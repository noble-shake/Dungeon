using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TakeStepAfterAttack", story: "[Self] Takes A Step After Attack", category: "Action", id: "f2991d53cc430e3c8a44f75e4d802a7d")]
public partial class TakeStepAfterAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        // 맵 중앙의 위치를 확인한다
        Vector3 center = GameManager.Instance.GetCenterOfBossRoom();
        DecideAngleAndTakeStep(center);
        // 각도에 따라 상하좌우 스텝을 밟아준다. 이 때 스텝을 밟을 때 충돌판정이 있다.
        return Status.Success;
    }


    public void DecideAngleAndTakeStep(Vector3 centerPosition)
    {
        // 만약의 경우 중복 실행이 안 되게끔
        Vector3 forward = Self.Value.transform.forward;
        forward.y = 0; // Y축 영향 제거 (수평 방향만 고려)
        forward.Normalize();

        Vector3 toTarget = centerPosition - Self.Value.transform.position;
        toTarget.y = 0; // Y축 영향 제거
        toTarget.Normalize();

        float angle = Mathf.Atan2(Vector3.Cross(forward, toTarget).y, Vector3.Dot(forward, toTarget)) * Mathf.Rad2Deg;

        // 결과 출력
        if (angle >= -45 && angle <= 45)
        {
            // Debug.Log("🔵 정면 영역 (-45° ~ 45°)");
            // 정면으로 대시 -> 이게 거리 좁히는 거랑 겹치게 사용되므로 삭제. 
            // Self.Value.GetComponent<AICharacterAnimatorManager>().PlayAnimationOnNetwork(AnimationType.Locomotion, "Boss_Slide_Front", true);
        }
        else if (angle > 45 && angle <= 135)
        {
            // Debug.Log("🟢 오른쪽 측면 (45° ~ 135°)");
            Self.Value.GetComponent<AICharacterAnimatorManager>().PlayAnimationOnNetwork(AnimationType.Locomotion, "Boss_Slide_Right", true, canRotate: true);
        }
        else if (angle < -45 && angle >= -135)
        {
            // Debug.Log("🟡 왼쪽 측면 (-45° ~ -135°)");
            Self.Value.GetComponent<AICharacterAnimatorManager>().PlayAnimationOnNetwork(AnimationType.Locomotion, "Boss_Slide_Left", true, canRotate: true);
        }
        else if ((angle > 135 && angle <= 225) || (angle < -135 && angle >= -225))
        {
            // Debug.Log("🔴 후면 (135° ~ 225°)");
            Self.Value.GetComponent<AICharacterAnimatorManager>().PlayAnimationOnNetwork(AnimationType.Locomotion, "Boss_Slide_Back", true);
        }
    }
}

