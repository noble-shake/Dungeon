using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// RPC란 Remote Procedure call 
// 원격 함수 호출

// 유니티 넷코드 서버 구조 

// 클라이언트 호스트
// 여러 개의 클라이언트와 단 하나의 호스트(클라이언트)

// 클라이언트 <-> 릴레이서버 <-> 호스트 

// RPC의 필요성 

// 호스트가 몬스터를 때렸음. 몬스터가 맞았을 때의 VFX, SFX 이펙트도 발생, 
// 몬스터의 체력이 깎일 때마다 VFX, SFX로 발동 

// RPC에 파라미터로 보내줄 수 있는 데이터는 프리미티브 데이터, 문자열, Vector3 값 
// 

public class CharacterLocomotionManager : MonoBehaviour
{
    CharacterManager character;
    // [HideInInspector] 
    public float verticalMovement;
    // [HideInInspector] 
    public float horizontalMovement;
    // [HideInInspector]
    public float moveAmount;

    [Header("Ground Check & Jumping")]

    [SerializeField] protected float gravityForce = -5.55f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckSphereRadius = 0.5f;
    [SerializeField] protected Vector3 yVelocity;
    [SerializeField] protected float groundedYVelocity = -20;
    [SerializeField] protected float fallStartYVelocity = -5;
    protected bool fallingVelocityHasBeenSet = false;
    protected float inAirTimer = 0;


    [Header("Flags")]
    public bool isRolling = false;
    public bool isGrounded = true;
    public bool canRotate = true;
    public bool canMove = true;
    public bool useGravity = true;


    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    protected virtual void Update()
    {
        if (GetComponent<CharacterController>().enabled == false)
            return;
        HandleGroundCheck();

        if (isGrounded)
        {
            if (yVelocity.y < 0)
            {
                inAirTimer = 0;
                fallingVelocityHasBeenSet = false;
                yVelocity.y = groundedYVelocity;
            }
        }
        else
        {
            if (!character.characterNetworkManager.isJumping.Value && !fallingVelocityHasBeenSet)
            {
                fallingVelocityHasBeenSet = true;
                yVelocity.y = fallStartYVelocity;
            }

            inAirTimer = inAirTimer + Time.deltaTime;
            character.animator.SetFloat("InAirTimer", inAirTimer);


            yVelocity.y += gravityForce * Time.deltaTime;

        }
        if (useGravity)
            character.characterController.Move(yVelocity * Time.deltaTime);

        // HandleAnimation();
    }



    protected virtual void HandleAnimation()
    {
        // if (InputManager.instance.playerInputLimit) return;

        // 내 플레이어 캐릭터의 움직임 애니메이션 제어
        if (character.IsOwner)
        {
            // 기본적으로 값은 1이 들어옴. 
            character.characterNetworkManager.animatorVerticalParameter.Value = verticalMovement;
            character.characterNetworkManager.animatorHorizontalParameter.Value = horizontalMovement;
            character.characterNetworkManager.animatorMoveAmount.Value = moveAmount;

        }
        // 
        else
        {
            verticalMovement = character.characterNetworkManager.animatorVerticalParameter.Value;
            horizontalMovement = character.characterNetworkManager.animatorHorizontalParameter.Value;
            moveAmount = character.characterNetworkManager.animatorMoveAmount.Value;
        }
        // 락온 상태가 아닐 때

        if (!character.characterNetworkManager.isLockedOn.Value || character.characterNetworkManager.isSprinting.Value)
        {
            character.characterAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, character.characterNetworkManager.isSprinting.Value);
        }
        // 그러나 현재 걷고 있는 기능이 없음. 현재 존재하는 씬에 따라 걷기만을 디폴트로 할 수도 있음.
        // 락온 상태일 때
        // 질주 상태가 아닐 때
        else
        {
            character.characterAnimatorManager.UpdateAnimatorMovementParameters(horizontalMovement, verticalMovement, character.characterNetworkManager.isSprinting.Value);
        }
    }

    protected void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(character.transform.position, groundCheckSphereRadius, groundLayer);
    }

    protected void OnDrawGizmosSelected()
    {
        // Gizmos.DrawSphere(character.transform.position, groundCheckSphereRadius);
    }

    public void EnableCanRotate()
    {
        canRotate = true;
    }

    public void DisableCanRotate()
    {
        canRotate = false;
    }

}
