using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ToggleBlockingController : StateMachineBehaviour
{
    PlayerManager player;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null) player = animator.GetComponent<PlayerManager>();

        if (player == null) return;

        if (player.isPerformingAction) return; // 플레이어가 현재 공격중이라면 중간에 애니메이터 컨트롤러를 바꿔끼지 말아야 함.

        // if (player.characterNetworkManager.isBlocking.Value)
        // {
            // player.characterAnimatorManager.UpdateAnimatorController(player.playerInventoryManager.currentLeftHandWeapon.weaponAnimator);
        // }
        // else
        // {
            // player.characterAnimatorManager.UpdateAnimatorController(player.playerInventoryManager.currentRightHandWeapon.weaponAnimator);
        // }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null) player = animator.GetComponent<PlayerManager>();

        if (player == null) return;

        if (player.isPerformingAction) return;

        // if (player.characterNetworkManager.isBlocking.Value)
        // {
            // player.characterAnimatorManager.UpdateAnimatorController(player.playerInventoryManager.currentLeftHandWeapon.weaponAnimator);
        // }
        // else
        // {
            // player.characterAnimatorManager.UpdateAnimatorController(player.playerInventoryManager.currentRightHandWeapon.weaponAnimator);
        // }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
