using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Buff Action")]
public class BuffWeaponItemAction : WeaponAction
{
    [SerializeField] string buff_Action_01 = "Buff_Action_01"; // 공격중이 아닐 때의 액션
    [SerializeField] float transitionTime_01 = 0.2f;

    public override void AttempToPerformAction(PlayerManager playerPerformingAction, Weapon weaponPerformingAction)
    {
        // 이건 그냥 현재 사용중인 무기가 뭔지 네트워크 상에 전파만 함.
        base.AttempToPerformAction(playerPerformingAction, weaponPerformingAction);

        // 취소 조건 시작
        if (!playerPerformingAction.IsOwner) return;

        // 스태미나가 없을 때 리턴
        // if (playerPerformingAction.playerNetworkManager.currentStamina.Value <= 0) return;
        // 땅에 붙어있지 않을 때 리턴
        if (!playerPerformingAction.playerLocomotionManager.isGrounded) return;

        // 취소 조건 종료

        if (playerPerformingAction.characterClass == PlayerClass.Archer)
        {
            Vector3 targetDirection = Vector3.zero;
            targetDirection = Camera.main.transform.forward;
            targetDirection.Normalize();
            targetDirection.y = 0;

            if (targetDirection == Vector3.zero)
                targetDirection = playerPerformingAction.transform.forward;

            Debug.Log("how much run?");
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            playerPerformingAction.transform.rotation = targetRotation;
        }


        InputManager.instance.BlockForSeconds(InputType.R, 5f);
        // 일반 회피 액션. 워리어일 땐 애니메이션의 어색함으로 주석화.
        // NOTE : 추후 다른 캐릭터에도 넣어줘야 한다면... 애니메이션 상태 존재 체크 후 실행하는 걸로 수정해야함.
        if (!playerPerformingAction.isPerformingAction)
            PerformBuffAction(playerPerformingAction, weaponPerformingAction);
    }

    private void PerformBuffAction(PlayerManager playerPerformingAction, Weapon weaponPerformingAction)
    {
        // playerPerformingAction.playerLocomotionManager.PlayerRotationToWardsInputDirection();
        playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Buff, buff_Action_01, true, weaponPerformingAction, transitionNomalizedTime: transitionTime_01);
    }
}
