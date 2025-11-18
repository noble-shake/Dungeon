using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 좌클릭으로 발동되는 공격
[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Light Attack Action")]
public class LightAttackWeaponItemAction : WeaponAction
{
    [Header("Light Attack")]
    [SerializeField] string light_Attack_01 = "Main_Light_Attack_01";
    [SerializeField] float attackDamage_01 = 100f;
    [SerializeField] float attackGroggyDamage_01 = 100f;
    [SerializeField] float lightAttackTranstionTime_01 = 0.2f;
    [SerializeField] AttackType attack01Type;
    [SerializeField] string light_Attack_02 = "Main_Light_Attack_02";
    [SerializeField] float attackDamage_02 = 100f;
    [SerializeField] float attackGroggyDamage_02 = 100f;
    [SerializeField] float lightAttackTranstionTime_02 = 0.2f;
    [SerializeField] AttackType attack02Type;
    [SerializeField] string light_Attack_03 = "Main_Light_Attack_03";
    [SerializeField] float attackDamage_03 = 50f;
    [SerializeField] float attackGroggyDamage_03 = 150f;
    [SerializeField] float lightAttackTranstionTime_03 = 0.2f;

    [SerializeField] AttackType attack03Type;
    [SerializeField] string light_Attack_04 = "Main_Light_Attack_04";
    [SerializeField] float attackDamage_04 = 150f;
    [SerializeField] float attackGroggyDamage_04 = 250f;
    [SerializeField] float lightAttackTranstionTime_04 = 0.2f;

    [SerializeField] AttackType attack04Type;

    [Header("Running Attack")]
    [SerializeField] string running_Attack_01 = "Main_Running_Attack_01";
    [SerializeField] float runningAttackDamage_01 = 200f;
    [SerializeField] float runningAttackGroggyDamage_01 = 200f;

    [SerializeField] AttackType runningAttack01Type;

    [Header("Counter Attack")]
    [SerializeField] string counter_Attack_01 = "Main_Counter_Attack_01";
    [SerializeField] float counterAttackDamage_01 = 100f;
    [SerializeField] float counterAttackGroggyDamage_01 = 100f;
    [SerializeField] AttackType counterAttack01Type;

    // [Header("Rolling Attack")]
    // [SerializeField] string rolling_Attack_01 = "Main_Rolling_Attack_01";
    // [SerializeField] float rollingMultiplier_01 = 2f;
    // [SerializeField] AttackType rollingAttack01Type;

    // [Header("Backstep Attack")]
    // [SerializeField] string backstep_Attack_01 = "Main_Backstep_Attack_01";
    // [SerializeField] float backstepMultiplier_01 = 2f;
    // [SerializeField] AttackType backstepAttack01Type;

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

        // NOTE : 반드시 공격을 하는가? 
        if (playerPerformingAction.characterCombatManager.canPerformCounterAttack)
        {
            PerformCounterAttack(playerPerformingAction, weaponPerformingAction);
            return;
        }

        // 대시에 쿨타임을 짧게 넣으면 그래도 되지 않나? 

        // 일반 공격
        PerformLightAttack(playerPerformingAction, weaponPerformingAction);
    }

    private void PerformCounterAttack(PlayerManager playerPerformingAction, Weapon weaponPerformingAction)
    {
        Debug.Log("카운터 실행 됐습니다.");
        // 아랫줄을 넣으면 카운터 시 자동으로 가드가 풀림. 따라서 주석화함. 나중에 컨트롤 가능할지도?
        // playerPerformingAction.characterNetworkManager.isBlocking.Value = false;
        // false로 바꿔주지 않으면 무한정 카운터 공격이 나갈 수도 있음. 
        playerPerformingAction.characterCombatManager.canPerformCounterAttack = false;
        playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Attack, counter_Attack_01, true, weaponPerformingAction, counterAttack01Type, damage: counterAttackDamage_01, groggyDamage: counterAttackGroggyDamage_01);
        // playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Attack, weaponPerformingAction, counterAttack01Type, counter_Attack_01, true);
    }

    // 공격의 특정 프레임이 지나면서 대시 가능, 콤보 가능으로 변함
    // 그러다가 대시를 하게 되면, 콤보 불가능으로 전환되는데 공격 애니메이션의 콤보 가능이 뒤늦게 실행 되면서 콤보 가능 상태로 전환
    // 함수가 두 개 ? 

    // 우선 콤보 공격 불가 함수
    // 대시 상태로 진입할 때 락
    // 대시 상태에서 벗어날 때 락 해제 


    private void PerformLightAttack(PlayerManager playerPerformingAction, Weapon weaponPerformingAction)
    {
        // 첫 타를 때리는 것이라면 
        if (!playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Attack, light_Attack_01, true, weaponPerformingAction, attack01Type, damage: attackDamage_01, groggyDamage: attackGroggyDamage_01, transitionNomalizedTime: lightAttackTranstionTime_01);
        }
        // 콤보 공격을 해야 한다면
        else if (playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.isPerformingAction)
        {
            // 해당 값은 애니메이션 이벤트에 의하여 true로 변경됨.
            // false로 바꿔주지 않으면 연타에 의해 중복 실행이 됨.
            playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;
            if (playerPerformingAction.playerCombatManager.lastAttackAnimationPerformed == light_Attack_01 && light_Attack_02 != string.Empty)
            {
                playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Attack, light_Attack_02, true, weaponPerformingAction, attack02Type, damage: attackDamage_02, groggyDamage: attackGroggyDamage_02, transitionNomalizedTime: lightAttackTranstionTime_02);
            }
            else if (playerPerformingAction.playerCombatManager.lastAttackAnimationPerformed == light_Attack_02 && light_Attack_03 != string.Empty)
            {
                playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Attack, light_Attack_03, true, weaponPerformingAction, attack03Type, damage: attackDamage_03, groggyDamage: attackGroggyDamage_03, transitionNomalizedTime: lightAttackTranstionTime_03);
            }
            else if (playerPerformingAction.playerCombatManager.lastAttackAnimationPerformed == light_Attack_03 && light_Attack_04 != string.Empty)
            {
                playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Attack, light_Attack_04, true, weaponPerformingAction, attack04Type, damage: attackDamage_04, groggyDamage: attackGroggyDamage_04, transitionNomalizedTime: lightAttackTranstionTime_04);
            }
        }
    }

}
