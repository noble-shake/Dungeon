using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacterAnimatorManager : CharacterAnimatorManager
{
    AICharacterManager aiCharacter;

    protected override void Awake()
    {
        base.Awake();

        aiCharacter = GetComponent<AICharacterManager>();
    }

    // 현재 이동에 관여하는 건 두 개임. navMeshAgent 와 캐릭터 컨트롤러임.
    // NOTE : 그러나 두 컴포넌트가 있을 때 실제로 이동을 제어하는 건 캐릭터 컨트롤러고, navMeshAgent의 speed 등은 무시된다. 
    // 
    // 애니메이션 루트모션이 캐릭터 컨트롤러를 통해 적용되고 있음.
    protected virtual void OnAnimatorMove()
    {
        if (aiCharacter.GetComponent<CharacterController>().enabled == false)
            return;

        // 로컬 객체라면 애니메이션의 루트 모션에 따라 위치값을 정해준다.
        if (aiCharacter.IsOwner)
        {
            // 평상시 애니메이션이라면 아래처럼 하되,
            if (aiCharacter.aiCharacterNetworkManager.isIgnoringRootmotion.Value)
            {
                return;
            }
            Vector3 velocity = aiCharacter.animator.deltaPosition;

            aiCharacter.characterController.Move(velocity);
            aiCharacter.transform.rotation *= aiCharacter.animator.deltaRotation;

            aiCharacter.aiCharacterNetworkManager.networkPosition.Value = transform.position;
            aiCharacter.aiCharacterNetworkManager.networkRotation.Value = transform.rotation;
        }

    }

    public void StopMovement()
    {
        UpdateAnimatorMovementParameters(0f, 0f, false, 0f);
        aiCharacter.aiCharacterNetworkManager.animatorVerticalParameter.Value = 0f;
    }
}
