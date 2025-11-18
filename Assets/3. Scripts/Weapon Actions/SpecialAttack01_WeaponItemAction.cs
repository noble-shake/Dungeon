using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 쉬프트키로 발동되는 공격
[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Special Attack Action 01")]
public class SpecialAttack01_WeaponItemAction : WeaponAction
{
    [Header("Special Attack")]
    [SerializeField] string Main_Special_Attack_01 = "Main_Special_Attack_01";
    [SerializeField] float specialAttackTransitionTime_01 = 0.2f;
    [SerializeField] AttackType attack01Type;
    [SerializeField] ActionType actionType;


    // 공격을 시도할 때 실행되는 함수
    public override void AttempToPerformAction(PlayerManager playerPerformingAction, Weapon weaponPerformingAction)
    {
        base.AttempToPerformAction(playerPerformingAction, weaponPerformingAction);

        // 취소 조건 시작
        if (!playerPerformingAction.IsOwner) return;

        // 스태미나가 없을 때 리턴
        // if (playerPerformingAction.playerNetworkManager.currentStamina.Value <= 0) return;
        // 땅에 붙어있지 않을 때 리턴
        if (!playerPerformingAction.playerLocomotionManager.isGrounded) return;

        if (playerPerformingAction.isPerformingAction) return;

        // 클래스별로 처리해줘야 할 일들을 애니메이션 실행 전에 처리한다.
        InputManager.instance.BlockForSeconds(InputType.Q, 10f);
        switch (playerPerformingAction.characterClass)
        {
            // TODO : 기사도 전사처럼 루트모션 사용 안해줘야 겠는데? 왜 이렇게 했지; 
            case PlayerClass.Knight:
            case PlayerClass.Thief:
            case PlayerClass.Archer:
                PerformSpecialAttack(playerPerformingAction, weaponPerformingAction);
                break;
            case PlayerClass.Warrior:
                PerformSpecialAttack(playerPerformingAction, weaponPerformingAction, false);
                break;
        }
    }

    private void PerformSpecialAttack(PlayerManager playerPerformingAction, Weapon weaponPerformingAction, bool applyRootmotion = true)
    {
        // 애니메이션의 액션 타입에 따라 애니메이션을 실행하기 전 처리해줘야 할 일들을 처리한다. 
        switch (playerPerformingAction.characterClass)
        {
            case PlayerClass.Knight:
                playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Attack, Main_Special_Attack_01, true, weaponPerformingAction, attack01Type, applyRootmotion: applyRootmotion, damage: damagePairs[0].damage, groggyDamage: damagePairs[0].groggyDamage
                , transitionNomalizedTime: specialAttackTransitionTime_01);
                break;
            case PlayerClass.Warrior:
                if (playerPerformingAction.playerCombatManager.currentTarget != null)
                {
                    playerPerformingAction.spearAndBackSlash.SpecialAttackActionSpearAndBackSlash(playerPerformingAction.playerCombatManager.currentTarget.transform.position);
                }
                else
                {
                    playerPerformingAction.spearAndBackSlash.SpecialAttackActionSpearAndBackSlash(Vector3.zero);
                }
                playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Attack, Main_Special_Attack_01, true, weaponPerformingAction, attack01Type, applyRootmotion: applyRootmotion, damage: damagePairs[0].damage, groggyDamage: damagePairs[0].groggyDamage
                , transitionNomalizedTime: specialAttackTransitionTime_01);
                break;
            case PlayerClass.Thief:
                if (playerPerformingAction.playerCombatManager.currentTarget != null)
                {
                    playerPerformingAction.dashAction.SpecialAttackActionDash(playerPerformingAction.playerCombatManager.currentTarget.transform.position);
                }
                else
                {
                    playerPerformingAction.dashAction.SpecialAttackActionDash(Vector3.zero);
                }

                playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Attack, Main_Special_Attack_01, true, weaponPerformingAction, attack01Type, applyRootmotion: applyRootmotion, damage: damagePairs[0].damage, groggyDamage: damagePairs[0].groggyDamage
                , transitionNomalizedTime: specialAttackTransitionTime_01);
                break;

            case PlayerClass.Archer:
                playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Attack, Main_Special_Attack_01, true, weaponPerformingAction, attack01Type, applyRootmotion: applyRootmotion, damage: damagePairs[0].damage, groggyDamage: damagePairs[0].groggyDamage
, transitionNomalizedTime: specialAttackTransitionTime_01);
                break;
        }

    }


}
