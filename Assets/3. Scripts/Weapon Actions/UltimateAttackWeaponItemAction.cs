using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Ultimate Skill Weapon Item Action")]
public class UltimateSkillAction : WeaponAction
{
    [Header("Ultimate Skill")]
    [SerializeField] string Buff_Action_01 = "Buff_Action_01";
    [SerializeField] float ultimateSkillTransitionTime_01 = 0.2f;
    [SerializeField] AttackType attack01Type;
    [SerializeField] ActionType actionType;
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

        switch (playerPerformingAction.characterClass)
        {
            case PlayerClass.Knight:
                InputManager.instance.BlockForSeconds(InputType.R, 5f);
                // InputManager.instance.BlockForSeconds(InputType.R, 120f);
                // PerformUltimateSkill(playerPerformingAction, weaponPerformingAction);
                break;
            case PlayerClass.Warrior:
                InputManager.instance.BlockForSeconds(InputType.R, 5f);
                // InputManager.instance.BlockForSeconds(InputType.R, 120f);
                // PerformUltimateSkill(playerPerformingAction, weaponPerformingAction);
                break;
            case PlayerClass.Thief:
                InputManager.instance.BlockForSeconds(InputType.R, 5f);
                // InputManager.instance.BlockForSeconds(InputType.R, 120f);
                // PerformUltimateSkill(playerPerformingAction, weaponPerformingAction);
                break;
            case PlayerClass.Archer:
                InputManager.instance.BlockForSeconds(InputType.R, 5f);
                // InputManager.instance.BlockForSeconds(InputType.R, 120f);
                break;
        }
        PerformUltimateSkill(playerPerformingAction, weaponPerformingAction);
    }
    private void PerformUltimateSkill(PlayerManager playerPerformingAction, Weapon weaponPerformingAction, bool applyRootmotion = true)
    {
        switch (playerPerformingAction.characterClass)
        {
            case PlayerClass.Knight:
                playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.SpecialAttack, Buff_Action_01, true, weaponPerformingAction, attack01Type, applyRootmotion: applyRootmotion, transitionNomalizedTime: ultimateSkillTransitionTime_01);
                break;
            case PlayerClass.Warrior:
                playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.SpecialAttack, Buff_Action_01, true, weaponPerformingAction, attack01Type, applyRootmotion: applyRootmotion, transitionNomalizedTime: ultimateSkillTransitionTime_01);
                break;
            case PlayerClass.Thief:
                playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.SpecialAttack, Buff_Action_01, true, weaponPerformingAction, attack01Type, applyRootmotion: applyRootmotion, transitionNomalizedTime: ultimateSkillTransitionTime_01);
                break;
            case PlayerClass.Archer:
                playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.SpecialAttack, Buff_Action_01, true, weaponPerformingAction, attack01Type, applyRootmotion: applyRootmotion, transitionNomalizedTime: ultimateSkillTransitionTime_01);
                break;
        }
    }

}
