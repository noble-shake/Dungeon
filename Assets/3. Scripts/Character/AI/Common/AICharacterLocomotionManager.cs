using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AICharacterLocomotionManager : CharacterLocomotionManager
{
    AICharacterManager ai;

    protected override void Awake()
    {
        base.Awake();
        ai = GetComponent<AICharacterManager>();
    }
    protected override void Update()
    {
        base.Update();
        HandleAnimation();
    }

    protected override void HandleAnimation()
    {
        // ai인데 호스트가 아닐 때
        if (!ai.IsOwner)
        {
            {
                horizontalMovement = ai.characterNetworkManager.animatorHorizontalParameter.Value;
                verticalMovement = ai.characterNetworkManager.animatorVerticalParameter.Value;
                moveAmount = ai.characterNetworkManager.animatorMoveAmount.Value;
            }
            ai.characterAnimatorManager.UpdateAnimatorMovementParameters(horizontalMovement, verticalMovement, false);
        }
    }

}
