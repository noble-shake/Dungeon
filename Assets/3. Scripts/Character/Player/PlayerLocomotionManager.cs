using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    public float transitionNomalizedTime = 0.2f;

    PlayerManager player;
    // [HideInInspector] public float verticalMovement;
    // [HideInInspector] public float horizontalMovement;
    // [HideInInspector] public float moveAmount;


    [Header("Movement Settings)")]
    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;

    [SerializeField] private float walkingSpeed = 2f;
    [SerializeField] private float runningSpeed = 5f;
    [SerializeField] private float sprintingSpeed = 6.5f;
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private int sprintingStaminaCost = 2;
    [SerializeField] private float attack_rotation_term;

    [Header("Dodge")]
    public Vector3 rollDirection;
    [SerializeField] float dodgeStaminaCost = 25f;

    [Header("Jump")]
    [SerializeField] float jumpStaminaCost = 25f;
    [SerializeField] float jumpHeight = 4;
    [SerializeField] float jumpForwardSpeed = 5f;
    [SerializeField] float freeFallSpeed = 2f;
    private Vector3 jumpDirection;

    [Header("Dash")]
    public bool dashInstant = false;

    [Header("Gravity Temporary")]
    private float temp_GroundedYVelocity;
    private float temp_GravityForce;
    private float temp_FallStartYVelocity;


    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
        temp_GravityForce = gravityForce;
        temp_GroundedYVelocity = groundedYVelocity;
        temp_FallStartYVelocity = fallStartYVelocity;

    }
    void Start()
    {

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        HandleAnimation();
    }

    public void HandleAllMovement()
    {
        if (!player.IsOwner) return;
        // if (InputManager.instance.playerInputLimit) return;
        HandleGroundMovement();
        HandleRotationMovement();
        HandleJumpingMovement();
        HandleFreeFallMovement();
    }

    private void GetMovementInputs()
    {
        verticalMovement = InputManager.instance.verticalInput;
        horizontalMovement = InputManager.instance.horizontalInput;
        moveAmount = InputManager.instance.moveAmount;

    }

    private void HandleGroundMovement()
    {
        GetMovementInputs();

        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 cameraRight = Camera.main.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        moveDirection = cameraForward * verticalMovement;

        moveDirection = moveDirection + cameraRight * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if (!player.playerLocomotionManager.canMove) return;

        // 1. 현재 전이중인데 대기 상태로 전이중이면 이동속도를 절반으로 줄임.
        // 대기 상태에서 이동 상태로 변경할 때 속도를 줄여서 애니메이션의 이상함을 줄임.

        // 아마 달리다가 멈출 때 멈추는 애니메이션이 따로 필요하다고 생각함.

        if (verticalMovement != 0 || horizontalMovement != 0)
        {
            player.playerSoundManager?.SFXMove(moveAmount);
        }


        if (player.playerNetworkManager.isLockedOn.Value)
        {
            // player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
            player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
            return;
        }

        if (player.playerNetworkManager.isSprinting.Value == true)
        {
            player.characterController.Move(moveDirection * sprintingSpeed * Time.deltaTime);
        }
        else
        {
            if (moveAmount <= 0.5f)
            {
                player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
            }
            if (moveAmount > 0.5f)
            {
                player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
            }
        }
    }



    private void HandleRotationMovement()
    {
        if (player.isDead.Value) return;

        if (!player.playerLocomotionManager.canRotate) return;

        if (player.playerNetworkManager.isLockedOn.Value)
        {
            if (player.playerNetworkManager.isSprinting.Value || player.playerLocomotionManager.isRolling)
            {
                Vector3 targetDirection = Vector3.zero;
                targetDirection = Camera.main.transform.forward * verticalMovement;
                targetDirection += Camera.main.transform.right * horizontalMovement;
                targetDirection.Normalize();
                targetDirection.y = 0;

                if (targetDirection == Vector3.zero)
                    targetDirection = transform.forward;

                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                player.rigidbody.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime));
            }
            else
            {
                if (player.playerCombatManager.currentTarget == null) return;

                Vector3 targetDirection;
                targetDirection = player.playerCombatManager.currentTarget.transform.position - transform.position;
                targetDirection.y = 0;
                targetDirection.Normalize();

                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                // if (player.playerNetworkManager.isAttacking.Value)
                // {
                //     player.rigidbody.MoveRotation(Quaternion.Slerp(player.rigidbody.rotation, targetRotation, rotationSpeed * Time.deltaTime));
                // }
                // else
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                // player.rigidbody.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime));

                // player.rigidbody.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime));
            }
        }
        else
        {

            targetRotationDirection = Vector3.zero;
            targetRotationDirection = Camera.main.transform.forward * verticalMovement;
            targetRotationDirection += Camera.main.transform.right * horizontalMovement;
            targetRotationDirection.Normalize();
            targetRotationDirection.y = 0;
            if (targetRotationDirection == Vector3.zero)
            {
                // y값을 0으로 안 만들어도 되나?
                targetRotationDirection = transform.forward;
            }
            Quaternion rotation = Quaternion.LookRotation(targetRotationDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleJumpingMovement()
    {
        if (player.characterNetworkManager.isJumping.Value)
        {
            player.characterController.Move(jumpDirection * jumpForwardSpeed * Time.deltaTime);
        }
    }

    private void HandleFreeFallMovement()
    {
        if (!player.playerLocomotionManager.isGrounded)
        {
            Vector3 freeFallDirection;

            freeFallDirection = Camera.main.transform.forward * InputManager.instance.verticalInput;
            freeFallDirection = freeFallDirection + Camera.main.transform.right * InputManager.instance.horizontalInput;
            freeFallDirection.y = 0;

            player.characterController.Move(freeFallDirection * freeFallSpeed * Time.deltaTime);
        }
    }

    public void HandleSprinting()
    {
        if (player.isPerformingAction)
        {
            player.playerNetworkManager.isSprinting.Value = false;
        }
        if (player.playerNetworkManager.currentStamina.Value <= 0)
        {
            player.playerNetworkManager.isSprinting.Value = false;
        }


        if (moveAmount >= 0.5)
        {
            player.playerNetworkManager.isSprinting.Value = true;
        }
        else
        {
            player.playerNetworkManager.isSprinting.Value = false;
        }

        if (player.playerNetworkManager.isSprinting.Value)
        {
            player.playerNetworkManager.currentStamina.Value -= sprintingStaminaCost * Time.deltaTime;
        }
    }

    public void AttempToPerformJump()
    {
        if (player.isPerformingAction) return;

        if (player.playerNetworkManager.currentStamina.Value <= 0) return;

        if (player.characterNetworkManager.isJumping.Value) return;

        if (!player.playerLocomotionManager.isGrounded) return;

        player.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, "Main_Jump_Start_01", false);

        ApplyJumpingVelocity();

        player.characterNetworkManager.isJumping.Value = true;

        player.playerNetworkManager.currentStamina.Value -= jumpStaminaCost;

        jumpDirection = Camera.main.transform.forward * InputManager.instance.verticalInput;
        jumpDirection += Camera.main.transform.right * InputManager.instance.horizontalInput;
        jumpDirection.y = 0;

        if (jumpDirection != Vector3.zero)
        {
            if (player.playerNetworkManager.isSprinting.Value)
            {
                jumpDirection *= 1;
            }
            else if (InputManager.instance.moveAmount > 0.5)
            {
                jumpDirection *= 0.5f;
            }
            else if (InputManager.instance.moveAmount <= 0.5)
            {
                jumpDirection *= 0.25f;
            }
        }
    }

    public void ApplyJumpingVelocity()
    {
        yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce);
    }

    public void AttempToPerformDash()
    {

        if (player.playerNetworkManager.currentStamina.Value <= 0) return;

        if (player.characterNetworkManager.isJumping.Value) return;

        if (!player.playerLocomotionManager.isGrounded) return;

        // 공격 도중 캔슬함. 
        if (player.isPerformingAction)
        {
            if (player.playerCombatManager.canDoEvade)
            {
                PlayerRotationToWardsInputDirection();
                Debug.Log("지금 얘가 실행되는겨?");
                player.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, "Dash_Forward_01", true, transitionNomalizedTime: 0f);
                return;
            }
            return;
        }

        if (dashInstant)
        {
            PlayerRotationToWardsInputDirection();

            player.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, "Dash_Forward_01", true, transitionNomalizedTime: 0f);
        }
        else
        {
            PlayerRotationToWardsInputDirection();
            player.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, "Dash_Forward_01", true, transitionNomalizedTime: 0f);
        }

        TryGetComponent<PlayerEmitter>(out PlayerEmitter playerSoundManager);
        // if(playerSoundManager != null) playerSoundManager.SFXEvade();
    }

    public void PlayerRotationToWardsInputDirection(float time = 0f, bool negative = false)
    {
        Vector3 direction;
        Quaternion targetRotation = Quaternion.identity;
        direction = player.playerLocomotionManager.moveDirection;
        if (direction == Vector3.zero)
        {
            direction = player.transform.forward;
            // 카메라의 정면으로 하게 한다? 
            if (player.playerCombatManager.autoAttackArrange)
            {
                direction = Camera.main.transform.forward;
                direction.y = 0;
            }
        }
        targetRotation = Quaternion.LookRotation(direction);
        if (negative == true)
            targetRotation = Quaternion.LookRotation(-direction);
        // DoRotate를 쓰면 다음 업데이트 때 실행되는 것 같아 즉시 변환해주기 위함. 

        if (time == 0)
        {
            player.transform.rotation = targetRotation;
        }
        else
        {
            // RotateWithRigidbody(targetRotation, time);
            player.transform.DORotateQuaternion(targetRotation, 5f * time).SetEase(Ease.Linear);
        }

    }

    public void RotateTowardsTarget(Vector3 targetPosition, float duration)
    {
        StartCoroutine(RotateTowardsTargetCoroutine(targetPosition, duration));
    }

    IEnumerator RotateTowardsTargetCoroutine(Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0f;
        Quaternion startRotation = player.rigidbody.rotation;

        // 목표 방향 계산
        Vector3 direction = targetPosition - player.transform.position;
        direction.y = 0; // Y축 회전만 고려 (수평 회전)
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            player.rigidbody.MoveRotation(Quaternion.Slerp(startRotation, targetRotation, t));
            yield return null;
        }

        // 최종적으로 목표 회전값을 확실히 설정
        player.rigidbody.MoveRotation(targetRotation);
    }

    public void RotateWithRigidbody(Quaternion targetRotation, float duration)
    {
        Debug.Log("코루틴 실행");
        StartCoroutine(RotateOverTime(targetRotation, duration));
    }

    IEnumerator RotateOverTime(Quaternion targetRotation, float duration)
    {
        float elapsedTime = 0f;
        Quaternion startRotation = player.rigidbody.rotation;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            player.rigidbody.MoveRotation(Quaternion.Slerp(startRotation, targetRotation, t));
            yield return null;
        }

        // 최종적으로 목표 회전값을 확실히 설정
        player.rigidbody.MoveRotation(targetRotation);
    }

    public void PlayerRotationToWardsInputDirection(Vector3 direction, float time = 0f)
    {
        Quaternion rotation;
        rotation = Quaternion.LookRotation(direction);
        player.transform.DORotateQuaternion(rotation, time).SetEase(Ease.Linear);
        // player.transform.rotation = rotation;
    }

    public void PlayerRotationToWardsInputDirection(Transform targetTransform, float time = 0f)
    {
        Vector3 direction = targetTransform.position - player.transform.position;
        direction.y = 0; // Y축 회전을 무시하여 수평 회전만 처리
        Quaternion rotation = Quaternion.LookRotation(direction);
        player.transform.DORotateQuaternion(rotation, time).SetEase(Ease.Linear);
    }


    // Y축으로 점프하는 애니메이션의 경우 중력값들의 영향을 받기 때문에
    // 애니메이션 실행하는 동안 중력값을 초기화해줌.
    // 이걸 해야 되나? 굳이? 하면 카메라가 따라가는데 그건 원치 않는데? 
    // Y축 점프가 높은 애들은 카메라가 따라가는 게 맞다. 

    // 해당 함수
    public void DisableGravitySettings()
    {
        // gravityForce = 0;
        // groundedYVelocity = 0;
        // fallStartYVelocity = 0;
        useGravity = false;
    }
    public void EnableGravitySettings()
    {
        useGravity = true;
    }

    public Vector3 GetMoveDirection()
    {
        return moveDirection;
    }

    public void MakePlayerUncontrollerable()
    {
        InputManager.instance.playerInputLimit = true;

        InputManager.instance.movementInput.x = 0;
        InputManager.instance.movementInput.y = 0;
        InputManager.instance.moveAmount = 0;

        player.playerAnimatorManager.StopAnimation();
        InputManager.instance.BlockLInput();
    }

    public void MakePlayerControllerable()
    {
        InputManager.instance.playerInputLimit = false;

    }

    public void MoveRoll(Vector3 direction, float time, float speed, bool rockOn = false)
    {
        player.coroutineInProcess = StartCoroutine(MoveRollCoroutine(direction, time, speed, rockOn));
    }

    public IEnumerator MoveRollCoroutine(Vector3 moveDirection, float time, float speed, bool rockOn = false)
    {
        float elapsedTime = 0f;
        // Vector3 moveDirection = GetMoveDirection(direction, rockOn);
        while (elapsedTime < time)
        {
            float deltaTime = Time.deltaTime;
            elapsedTime += deltaTime;

            // CharacterController를 사용한 이동 (중력 반영)
            player.characterController.Move(moveDirection * speed * deltaTime);

            yield return null;
        }
    }


}
