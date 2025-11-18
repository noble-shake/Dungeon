// using System.Collections;
// using System.Collections.Generic;
// using TMPro;
// using UnityEngine;

// [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Ultimate Skill Action")]
// public class UltimateSkillAction : WeaponAction
// {
//     public override void AttempToPerformAction(PlayerManager playerPerformingAction, Weapon weaponPerformingAction)
//     {
//         base.AttempToPerformAction(playerPerformingAction, weaponPerformingAction);

//         // 취소 조건 시작
//         if (!playerPerformingAction.IsOwner) return;

//         // 스태미나가 없을 때 리턴
//         // if (playerPerformingAction.playerNetworkManager.currentStamina.Value <= 0) return;
//         // 땅에 붙어있지 않을 때 리턴
//         if (!playerPerformingAction.playerLocomotionManager.isGrounded) return;

//         if (playerPerformingAction.isPerformingAction) return;

//         // 클래스별로 처리해줘야 할 일들을 애니메이션 실행 전에 처리한다.
//         InputManager.instance.BlockForSeconds(InputType.R, 10f);
//         switch (playerPerformingAction.characterClass)
//         {
//             case PlayerClass.Knight:
//                 PerformUltimateSkill(playerPerformingAction, weaponPerformingAction);
//                 break;
//             case PlayerClass.Warrior:
//                 PerformUltimateSkill(playerPerformingAction, weaponPerformingAction, false);
//                 break;
//         }


//     }

   
// }
