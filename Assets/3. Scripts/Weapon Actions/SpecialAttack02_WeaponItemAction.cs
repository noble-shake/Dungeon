using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using Unity.VisualScripting;
using UnityEngine;

// 쉬프트키로 발동되는 공격
[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Special Attack Action 02")]
public class SpecialAttack02_WeaponItemAction : WeaponAction
{
    [Header("Special Attack")]
    [SerializeField] string Main_Special_Attack_02 = "Main_Special_Attack_02";
    [SerializeField] float specialAttackTransitionTime_02 = 0.2f;
    [SerializeField] AttackType attack02Type;
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

        InputManager.instance.BlockForSeconds(InputType.Shift, 20f);
        PerformSpecialAttack(playerPerformingAction, weaponPerformingAction);

    }

    private void PerformSpecialAttack(PlayerManager playerPerformingAction, Weapon weaponPerformingAction)
    {
        // 애니메이션의 액션 타입에 따라 애니메이션을 실행하기 전 처리해줘야 할 일들을 처리한다. 
        switch (playerPerformingAction.characterClass)
        {
            // 충격파 데미지는? 
            case PlayerClass.Knight:
                if (playerPerformingAction.playerCombatManager.currentTarget != null)
                {
                    // 타겟이 있으면 타겟의 네트워크 오브젝트 아이디를 넘긴다.
                    playerPerformingAction.riseAndDive?.SpecialAttackActionRiseAndDive(playerPerformingAction.playerCombatManager.currentTarget.transform.position);
                }
                else
                    playerPerformingAction.riseAndDive?.SpecialAttackActionRiseAndDive(Vector3.zero);
                playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.SpecialAttack, Main_Special_Attack_02, true, weaponPerformingAction, attack02Type, damage: damagePairs[0].damage,
                 groggyDamage: damagePairs[0].groggyDamage, transitionNomalizedTime: specialAttackTransitionTime_02);
                break;
            case PlayerClass.Warrior:
                if (playerPerformingAction.playerCombatManager.currentTarget != null)
                    playerPerformingAction.dashAction.SpecialAttackActionDash(playerPerformingAction.playerCombatManager.currentTarget.transform.position);
                else
                    playerPerformingAction.dashAction.SpecialAttackActionDash(Vector3.zero);
                float holdTime = InputManager.instance.HoldTime;
                float damage = 0;
                float groggyDamage = 0;
                if (holdTime < 1)
                {
                    damage = damagePairs[0].damage;
                    groggyDamage = damagePairs[0].groggyDamage;
                }
                else if (holdTime >= 1 && holdTime < 2)
                {
                    damage = damagePairs[1].damage;
                    groggyDamage = damagePairs[1].groggyDamage;
                }
                else if (holdTime >= 2)
                {
                    damage = damagePairs[2].damage;
                    groggyDamage = damagePairs[2].groggyDamage;
                }
                playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.SpecialAttack, Main_Special_Attack_02, true, weaponPerformingAction, attack02Type, damage: damage,
                 groggyDamage: groggyDamage, transitionNomalizedTime: specialAttackTransitionTime_02);
                break;
            // 충격파 데미지는? 
            case PlayerClass.Thief:
                if (playerPerformingAction.playerCombatManager.currentTarget != null)
                    playerPerformingAction.riseAndDive?.SpecialAttackActionRiseAndDive(playerPerformingAction.playerCombatManager.currentTarget.transform.position);
                else
                    playerPerformingAction.riseAndDive?.SpecialAttackActionRiseAndDive(Vector3.zero);
                playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.SpecialAttack, Main_Special_Attack_02, true, weaponPerformingAction, attack02Type, applyRootmotion: false, damage: damagePairs[0].damage,
                 groggyDamage: damagePairs[0].groggyDamage, transitionNomalizedTime: specialAttackTransitionTime_02);
                break;
            default:
                break;
        }
    }

}
