using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimatorManager : CharacterAnimatorManager
{
    PlayerManager player;
    public float moveMultiplier = 1.5f;
    [SerializeField] const string rollAnimation = "roll_front";
    [SerializeField] public float rollMul = 7f;
    [SerializeField] const string DashAnimation = "avoid_front";
    [SerializeField] float dashMul = 2.5f;
    [SerializeField] float warrior_dashMul = 1f;
    [SerializeField] float rise_y_mul = 1f;
    [SerializeField] float rise_xz_mul = 1f;
    [SerializeField] float dive_y_mul = 1f;
    [SerializeField] float dive_xz_mul = 1f;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }

    private void OnAnimatorMove()
    {
        if (player.playerAnimatorManager.applyRootMotion)
        {
            Vector3 velocity;
            // 애니메이터 레이어의 Action Override를 쓰겠단 뜻임.
            AnimatorClipInfo[] clipInfo = player.animator.GetCurrentAnimatorClipInfo(2);
            // velocity = player.animator.deltaPosition;
            // player.characterController.Move(velocity); // 이 때 확인해야 할 것. 애니메이션이 Y축 값으로 루프 매치 하는가?
            // player.transform.rotation *= player.animator.deltaRotation; // 왜 곱하지? 
            if (clipInfo.Length > 0)
            {
                switch (clipInfo[0].clip.name)
                {
                    case "roll_front":
                        player.characterController.Move(player.transform.forward * rollMul * Time.deltaTime);
                        player.transform.rotation *= player.animator.deltaRotation;
                        break;
                    case "roll_back":
                        player.characterController.Move(-player.transform.forward * rollMul * Time.deltaTime);
                        player.transform.rotation *= player.animator.deltaRotation;
                        break;
                    case "roll_left":
                        player.characterController.Move(-player.transform.right * rollMul * Time.deltaTime);
                        player.transform.rotation *= player.animator.deltaRotation;
                        break;
                    case "roll_right":
                        player.characterController.Move(player.transform.right * rollMul * Time.deltaTime);
                        player.transform.rotation *= player.animator.deltaRotation;
                        break;
                    case "attack08":
                        velocity = player.animator.deltaPosition;

                        // 앞으로 움직이고 위로 뜰 때, 즉 도약할 때
                        if (IsMovingForward(player.transform, velocity) && velocity.y > 0)
                        {
                            velocity.y *= rise_y_mul;
                            velocity.x *= rise_xz_mul;
                            velocity.z *= rise_xz_mul;
                        }
                        if (IsMovingForward(player.transform, velocity) && velocity.y < 0)
                        {
                            velocity.y *= dive_y_mul;
                            velocity.x *= dive_xz_mul;
                            velocity.z *= dive_xz_mul;
                        }
                        // player.characterController.Move(velocity); // 이 때 확인해야 할 것. 애니메이션이 Y축 값으로 루프 매치 하는가?
                        // player.transform.rotation *= player.animator.deltaRotation; // 왜 곱하지? 
                        break;
                    // NOTE : 워리어의 대시 에니 메이션은 변화량 증폭 불가.
                    // 대시 애니메이션을 즉발로 설정할 경우, 프레임 끊기는 듯한 현상이 보여 즉발로 설정 불가
                    // 크로스페이드로 애니메이션 실행 시, 10% 혹은 20%로 전이하는데 이 때 실제적인 위치 변화가 발생함.
                    // 
                    // case DashAnimation:
                    //     velocity = player.animator.deltaPosition;
                    //     player.characterController.Move(velocity); // 이 때 확인해야 할 것. 애니메이션이 Y축 값으로 루프 매치 하는가?
                    //     player.transform.rotation *= player.animator.deltaRotation; // 왜 곱하지? 
                    //     Debug.Log($"대시가 {dashMul} 만큼 변화되어 진행됐습니다.");
                    //     break;
                    default:
                        // Debug.Log("")
                        velocity = player.animator.deltaPosition;
                        player.characterController.Move(velocity); // 이 때 확인해야 할 것. 애니메이션이 Y축 값으로 루프 매치 하는가?
                        player.transform.rotation *= player.animator.deltaRotation; // 왜 곱하지? 
                        break;
                }
            }
        }
    }

    bool IsMovingForward(Transform playerTransform, Vector3 velocity)
    {
        Vector3 forward = playerTransform.forward.normalized; // 현재 바라보는 방향 (정규화)
        Vector3 velocityDir = velocity.normalized; // 속도 방향 (정규화)

        float dot = Vector3.Dot(forward, velocityDir); // 내적 계산

        return dot > 0; // 0 초과면 true (전진), 0 이하면 false (후진)
    }

    public void StopAnimation()
    {
        player.animator.SetFloat(horizontal, 0);
        player.animator.SetFloat(vertical, 0);
        player.playerNetworkManager.animatorHorizontalParameter.Value = 0;
        player.playerNetworkManager.animatorVerticalParameter.Value = 0;
        player.playerNetworkManager.animatorMoveAmount.Value = 0;
    }
}
