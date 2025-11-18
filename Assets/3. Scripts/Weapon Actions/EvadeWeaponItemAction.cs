using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Evade Action")]
public class EvadeWeaponItemAction : WeaponAction
{
    [Header("이동 관련 설정")]
    [SerializeField] private float moveTime;
    [SerializeField] private float speed;


    [SerializeField] string evade_Action_01 = "Evade_Action_01"; // 공격중이 아닐 때의 액션
    [SerializeField] float transitionTime_01 = 0.2f;
    [SerializeField] string evade_Action_02 = "Evade_Action_02"; // 공격중일 때의 액션
    [SerializeField] float transitionTime_02 = 0.2f;
    [SerializeField] string roll_Action_Front = "Roll_Action_Front"; // 공격중일 때의 액션
    [SerializeField] float transitionTime_Front = 0.2f;
    [SerializeField] string roll_Action_Back = "Roll_Action_Back"; // 공격중일 때의 액션
    [SerializeField] float transitionTime_Back = 0.2f;
    [SerializeField] string roll_Action_Left = "Roll_Action_Left"; // 공격중일 때의 액션
    [SerializeField] float transitionTime_Left = 0.2f;
    [SerializeField] string roll_Action_Right = "Roll_Action_Right"; // 공격중일 때의 액션
    [SerializeField] float transitionTime_Right = 0.2f;

    [SerializeField] string step_Action_Front = "Step_Action_Front"; // 공격중일 때의 액션
    [SerializeField] float step_TransitionTime_Front = 0.2f;
    [SerializeField] string step_Action_Back = "Step_Action_Back"; // 공격중일 때의 액션
    [SerializeField] float step_TransitionTime_Back = 0.2f;
    [SerializeField] string step_Action_Left = "Step_Action_Left"; // 공격중일 때의 액션
    [SerializeField] float step_TransitionTime_Left = 0.2f;
    [SerializeField] string step_Action_Right = "Step_Action_Right"; // 공격중일 때의 액션
    [SerializeField] float step_TransitionTime_Right = 0.2f;
    [SerializeField] CombatStatus evadeType;

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

        switch (playerPerformingAction.characterClass)
        {
            // 기사의 경우에는 단순 가드. 현재는 액션이 아닐 때만 가드가 되게끔 구현되어 있는데
            // 얘를 공격 중에도 가드할 수 있게 해줘야 하지 않나?  
            case PlayerClass.Knight:
                break;
            case PlayerClass.Warrior:
                // playerPerformingAction.playerLocomotionManager.PlayerRotationToWardsInputDirection();
                break;
        }
        PerformEvadeAction(playerPerformingAction, weaponPerformingAction);
    }

    private void PerformEvadeActionWhileAttack(PlayerManager playerPerformingAction, Weapon weaponPerformingAction)
    {
        playerPerformingAction.playerLocomotionManager.PlayerRotationToWardsInputDirection();
        playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, evade_Action_02, true, weaponPerformingAction, transitionNomalizedTime: transitionTime_02);
    }

    private void PerformEvadeAction(PlayerManager playerPerformingAction, Weapon weaponPerformingAction)
    {
        // 락온 상태 
        if (playerPerformingAction.playerNetworkManager.isLockedOn.Value)
        {
            // 락온, 공격중일 때 구르기가 가능한 순간
            if (playerPerformingAction.playerNetworkManager.isAttacking.Value && playerPerformingAction.playerCombatManager.canDoEvade)
            {
                // 재입력을 방지하기 위해 값을 바꿔준다.
                playerPerformingAction.playerCombatManager.canDoEvade = false;

                PerformEvadeActionPerCondition(playerPerformingAction, weaponPerformingAction, CombatStatus.RockOn_Attacking);
                return;
            }
            if (playerPerformingAction.isPerformingAction && playerPerformingAction.playerCombatManager.canDoEvade)
            {
                playerPerformingAction.playerCombatManager.canDoEvade = false;

                PerformEvadeActionPerCondition(playerPerformingAction, weaponPerformingAction, CombatStatus.RockOn_DoingSomething);
                return;
            }

            // 특정 애니메이션을 실행하고 있는 게 아닐 때 -> 그냥 락온 중이기만 하고 걷기 등을 진행하고 있을 때
            if (!playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerCombatManager.canDoEvade = false;

                PerformEvadeActionPerCondition(playerPerformingAction, weaponPerformingAction, CombatStatus.RockOn_Normal);
                return;
            }

        }

        // 락온 중이 아니고, 특정 애니메이션을 실행하고 있는 게 아닐 때 -> 그냥 이동중일 때 
        if (!playerPerformingAction.isPerformingAction)
        {
            PerformEvadeActionPerCondition(playerPerformingAction, weaponPerformingAction, CombatStatus.Normal);
            return;
        }

        // 공격 애니메이션 실행 중인데 회피가 가능할 때
        if (playerPerformingAction.playerNetworkManager.isAttacking.Value && playerPerformingAction.playerCombatManager.canDoEvade)
        {
            // 재입력을 방지하기 위해 회피가 불가능하게 값을 바꿔준다.
            playerPerformingAction.playerCombatManager.canDoEvade = false;
            PerformEvadeActionPerCondition(playerPerformingAction, weaponPerformingAction, CombatStatus.Attacking);

            return;
        }

        // 특정 애니메이션을 실행 중인데 회피가 가능한 순간이라면 -> 애니메이션 이벤트로 회피 변수 발동했을 때
        // 다운 애니메이션 등 
        if (playerPerformingAction.isPerformingAction && playerPerformingAction.playerCombatManager.canDoEvade)
        {
            playerPerformingAction.playerCombatManager.canDoEvade = false;
            PerformEvadeActionPerCondition(playerPerformingAction, weaponPerformingAction, CombatStatus.DoingSomething);
            return;
        }
    }



    private void PerformEvadeActionPerCondition(PlayerManager playerPerformingAction, Weapon weaponPerformingAction, CombatStatus evadeType)
    {
        PlayerClass playerClass = playerPerformingAction.characterClass;
        switch (evadeType)
        {
            case CombatStatus.Normal:
                switch (playerClass)
                {
                    case PlayerClass.Knight:
                    case PlayerClass.Warrior:
                    case PlayerClass.Thief:
                    case PlayerClass.Archer:
                        PerformRollingAction(playerPerformingAction, weaponPerformingAction);
                        break;
                    default:
                        break;
                }
                break;
            case CombatStatus.Attacking:
                switch (playerClass)
                {
                    case PlayerClass.Knight:
                        break;
                    case PlayerClass.Warrior:
                        playerPerformingAction.playerLocomotionManager.PlayerRotationToWardsInputDirection();
                        PerformDashAction(playerPerformingAction, weaponPerformingAction);
                        // 전사가 대시할 땐 다시 풀어줘야 함.
                        // InputManager.instance.BlockForSeconds(InputType.Space, 1f);

                        break;
                    case PlayerClass.Thief:
                        playerPerformingAction.playerLocomotionManager.PlayerRotationToWardsInputDirection();
                        PerformRollingAction(playerPerformingAction, weaponPerformingAction);
                        // PerformRollingAction(playerPerformingAction, weaponPerformingAction, DecidePlayerInputDirection(playerPerformingAction));

                        break;
                    case PlayerClass.Archer:
                        playerPerformingAction.playerLocomotionManager.PlayerRotationToWardsInputDirection();
                        PerformRollingAction(playerPerformingAction, weaponPerformingAction);
                        break;
                    default:
                        break;
                }
                break;
            // 다운 후, 혹은 패링 후를 상정하고 있음. 
            case CombatStatus.DoingSomething:
                switch (playerClass)
                {
                    case PlayerClass.Knight:
                    case PlayerClass.Warrior:
                    case PlayerClass.Thief:
                    case PlayerClass.Archer:

                        playerPerformingAction.playerLocomotionManager.PlayerRotationToWardsInputDirection();
                        PerformRollingAction(playerPerformingAction, weaponPerformingAction);
                        // PerformRollingAction(playerPerformingAction, weaponPerformingAction, DecidePlayerInputDirection(playerPerformingAction));
                        break;

                    default:
                        break;
                }
                break;
            case CombatStatus.RockOn_Normal:
                switch (playerClass)
                {
                    case PlayerClass.Knight:
                    case PlayerClass.Warrior:
                    case PlayerClass.Thief:
                        Debug.Log("기본 구르기로 실행이 되고 있다?");
                        // playerPerformingAction.playerLocomotionManager.PlayerRotationToWardsInputDirection();
                        PerformRollingAction(playerPerformingAction, weaponPerformingAction, DecidePlayerInputDirection(playerPerformingAction), true);
                        break;
                    case PlayerClass.Archer:

                    // case PlayerClass.Thief:
                    // PerformStepingAction(playerPerformingAction, weaponPerformingAction, DecidePlayerInputDirection(playerPerformingAction));
                    // break;

                    default:
                        break;
                }
                break;
            case CombatStatus.RockOn_Attacking:
                switch (playerClass)
                {
                    case PlayerClass.Knight:
                        break;
                    case PlayerClass.Warrior:
                        playerPerformingAction.playerLocomotionManager.PlayerRotationToWardsInputDirection();
                        PerformDashAction(playerPerformingAction, weaponPerformingAction);
                        break;
                    case PlayerClass.Thief:
                        PerformRollingAction(playerPerformingAction, weaponPerformingAction, DecidePlayerInputDirection(playerPerformingAction), true);
                        //  PerformStepingAction(playerPerformingAction, weaponPerformingAction, DecidePlayerInputDirection(playerPerformingAction));
                        break;
                    case PlayerClass.Archer:
                        PerformRollingAction(playerPerformingAction, weaponPerformingAction, DecidePlayerInputDirection(playerPerformingAction));
                        //  PerformStepingAction(playerPerformingAction, weaponPerformingAction, DecidePlayerInputDirection(playerPerformingAction));
                        break;
                    default:
                        break;
                }
                break;
            case CombatStatus.RockOn_DoingSomething:
                switch (playerClass)
                {
                    case PlayerClass.Warrior:
                    case PlayerClass.Knight:
                    case PlayerClass.Thief:
                    case PlayerClass.Archer:
                        // playerPerformingAction.playerLocomotionManager.PlayerRotationToWardsInputDirection();
                        PerformRollingAction(playerPerformingAction, weaponPerformingAction, DecidePlayerInputDirection(playerPerformingAction), true);

                        // PerformRollingAction(playerPerformingAction, weaponPerformingAction);
                        break;

                    default:
                        break;
                }
                break;
        }

        // if(playerPerformingAction.playerSoundManager != null) playerPerformingAction.playerSoundManager.SFXEvade();

    }

    // 얘가 8각 대쉬 하도록 바꿔야 함. 
    private void PerformRollingAction(PlayerManager playerPerformingAction, Weapon weaponPerformingAction, Direction direction = Direction.F, bool rockOn = false)
    {
        InputManager.instance.BlockForSeconds(InputType.Space, 1f);

        Vector3 moveDirection = GetMoveDirection(direction, rockOn, playerPerformingAction);
        switch (direction)
        {
            case Direction.F:
                playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, roll_Action_Front, true, weaponPerformingAction, applyRootmotion: false, transitionNomalizedTime: transitionTime_Front);
                break;
            case Direction.FR:
                playerPerformingAction.playerLocomotionManager.PlayerRotationToWardsInputDirection(moveDirection, 0.1f);
                playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, roll_Action_Front, true, weaponPerformingAction, applyRootmotion: false, transitionNomalizedTime: transitionTime_Front);
                break;
            case Direction.FL:
                playerPerformingAction.playerLocomotionManager.PlayerRotationToWardsInputDirection(moveDirection, 0.1f);
                // playerPerformingAction.playerLocomotionManager.PlayerRotationToWardsInputDirection(0.1f);
                playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, roll_Action_Front, true, weaponPerformingAction, applyRootmotion: false, transitionNomalizedTime: transitionTime_Front);
                break;
            case Direction.B:
                playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, roll_Action_Back, true, weaponPerformingAction, applyRootmotion: false, transitionNomalizedTime: transitionTime_Back);
                break;
            case Direction.BR:
                playerPerformingAction.playerLocomotionManager.PlayerRotationToWardsInputDirection(-moveDirection, 0.1f);
                // playerPerformingAction.playerLocomotionManager.PlayerRotationToWardsInputDirection(0.1f, negative: true);
                playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, roll_Action_Back, true, weaponPerformingAction, applyRootmotion: false, transitionNomalizedTime: transitionTime_Back);
                break;
            case Direction.BL:
                playerPerformingAction.playerLocomotionManager.PlayerRotationToWardsInputDirection(-moveDirection, 0.1f);
                // playerPerformingAction.playerLocomotionManager.PlayerRotationToWardsInputDirection(0.1f, negative: true);
                playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, roll_Action_Back, true, weaponPerformingAction, applyRootmotion: false, transitionNomalizedTime: transitionTime_Back);
                break;
            case Direction.L:
                playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, roll_Action_Left, true, weaponPerformingAction, applyRootmotion: false, transitionNomalizedTime: transitionTime_Left);
                break;
            case Direction.R:
                playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, roll_Action_Right, true, weaponPerformingAction, applyRootmotion: false, transitionNomalizedTime: transitionTime_Right);
                break;
        }

        playerPerformingAction.playerLocomotionManager.MoveRoll(moveDirection, moveTime, speed, rockOn);


    }

    private void PerformDashAction(PlayerManager playerPerformingAction, Weapon weaponPerformingAction)
    {
        // 빠르게 회전 변환 후 대쉬 액션
        // playerPerformingAction.playerLocomotionManager.PlayerRotationToWardsInputDirection();
        playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, evade_Action_01, true, weaponPerformingAction, transitionNomalizedTime: transitionTime_01);
    }

    private void PerformStepingAction(PlayerManager playerPerformingAction, Weapon weaponPerformingAction, Direction direction = Direction.F)
    {
        switch (direction)
        {
            case Direction.F:
                playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, step_Action_Front, true, weaponPerformingAction, transitionNomalizedTime: transitionTime_Front);
                break;

            case Direction.B:
                playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, step_Action_Back, true, weaponPerformingAction, transitionNomalizedTime: transitionTime_Back);
                break;
            case Direction.L:
                playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, step_Action_Left, true, weaponPerformingAction, canRotate: true, transitionNomalizedTime: transitionTime_Left);
                break;
            case Direction.R:
                playerPerformingAction.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, step_Action_Right, true, weaponPerformingAction, canRotate: true, transitionNomalizedTime: transitionTime_Right);
                break;
        }
    }

    // 플레이어가 맞아서 누운 상태
    // 그 와중에 보스는 내 왼쪽으로 이동.
    // 내 발 방향은 북쪽인데 보스는 서쪽에 있음.

    private Vector3 GetMoveDirection(Direction direction, bool rockOn, PlayerManager player)
    {
        Vector3 moveDirection = Vector3.zero;
        if (!rockOn)
            switch (direction)
            {
                case Direction.F:
                    moveDirection = player.transform.forward;
                    break;
                case Direction.B:
                    moveDirection = -player.transform.forward;
                    break;
                case Direction.L:
                    moveDirection = -player.transform.right;
                    break;
                case Direction.R:
                    moveDirection = player.transform.right;
                    break;
                case Direction.FL:
                    moveDirection = (player.transform.forward - player.transform.right).normalized;
                    break;
                case Direction.FR:
                    moveDirection = (player.transform.forward + player.transform.right).normalized;
                    break;
                case Direction.BL:
                    moveDirection = (-player.transform.forward - player.transform.right).normalized;
                    break;
                case Direction.BR:
                    moveDirection = (-player.transform.forward + player.transform.right).normalized;
                    break;
            }
        else
        {
            Quaternion rotation = Quaternion.identity;
            switch (direction)
            {
                case Direction.F:
                    Debug.Log("내가 과연 여기서 실행되고 있을까?");
                    rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case Direction.FR:
                    rotation = Quaternion.Euler(0, 45, 0);
                    break;
                case Direction.R:
                    rotation = Quaternion.Euler(0, 90, 0);
                    break;
                case Direction.BR:
                    rotation = Quaternion.Euler(0, 135, 0);
                    break;
                case Direction.B:
                    rotation = Quaternion.Euler(0, 180, 0);
                    break;
                case Direction.BL:
                    rotation = Quaternion.Euler(0, -135, 0);
                    break;
                case Direction.L:
                    rotation = Quaternion.Euler(0, -90, 0);
                    break;
                case Direction.FL:
                    rotation = Quaternion.Euler(0, -45, 0);
                    break;
            }
            // moveDirection = player.playerCombatManager.currentTarget.transform.position - player.transform.position;
            moveDirection = Camera.main.transform.forward;
            moveDirection.y = 0;
            moveDirection.Normalize();
            moveDirection = rotation * moveDirection;
        }

        return moveDirection;
    }



    private Direction DecidePlayerInputDirection(PlayerManager playerPerformingAction)
    {
        float vertical = InputManager.instance.verticalInput;
        float horizontal = InputManager.instance.horizontalInput;
        // if (horizontalInput >= 0.5f)
        // {
        //     return Direction.R;
        // }
        // else if (horizontalInput <= -0.5f)
        // {
        //     return Direction.L;
        // }
        // else if (vertical >= 0.5f)
        // {
        //     return Direction.F;
        // }
        // else if (vertical <= -0.5f)
        // {
        //     return Direction.B;
        // }
        // return Direction.F;
        if (vertical > 0.5f && horizontal > 0.5f)
            return Direction.FR; // ↗️
        if (vertical > 0.5f && horizontal < -0.5f)
            return Direction.FL; // ↖️
        if (vertical < -0.5f && horizontal > 0.5f)
            return Direction.BR; // ↘️
        if (vertical < -0.5f && horizontal < -0.5f)
            return Direction.BL; // ↙️

        // 기본 4방향 처리
        if (vertical > 0.5f)
            return Direction.F; // ↑
        if (vertical < -0.5f)
            return Direction.B; // ↓
        if (horizontal > 0.5f)
            return Direction.R; // →
        if (horizontal < -0.5f)
            return Direction.L; // ←
        return Direction.F;

    }
}
