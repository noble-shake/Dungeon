using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Heavy Attack Action")]
public class HeavyAttackWeaponItemAction : WeaponAction
{
    [SerializeField] string Parry_Action_01 = "Parry_Action_01";
    [SerializeField] float parryTime = 0.5f;
    [SerializeField] float playerStiffTime = 0.25f;
    [SerializeField] float enemyStiffTime = 0.15f;
    [SerializeField] float coolTime = 10f;
    private float elapsedTime = 0f;

    public override void AttempToPerformAction(PlayerManager playerPerformingAction, Weapon weaponPerformingAction)
    {
        base.AttempToPerformAction(playerPerformingAction, weaponPerformingAction);

        if (!playerPerformingAction.IsOwner) return;

        if (!playerPerformingAction.playerLocomotionManager.isGrounded) return;



        PerformParrying(playerPerformingAction, weaponPerformingAction);
    }

    private void PerformParrying(PlayerManager playerPerformingAction, Weapon weaponPerformingAction)
    {
        switch (playerPerformingAction.characterClass)
        {
            // 클래스에 따라서 패링 조건이 다름.
            // 기사는 공격 도중에도 가능
            // 나머지는 공격 도중에 불가.
            // 패링하게 되면 기사는 쿨타임이 초기화 된다.
            // 연속 패링 가능, 공격 중에는 불가 
            // 아래 주석된 부분은 원래 전사와 도적꺼였음. 
            //         if (playerPerformingAction.playerNetworkManager.isParrying.Value
            //             || !playerPerformingAction.isPerformingAction)
            //           {
            //     playerPerformingAction.parryingAction.AttempToPerformParrying();
            //          }
            //          break;
            case PlayerClass.Warrior:
            case PlayerClass.Thief:
            case PlayerClass.Knight:
            case PlayerClass.Archer:
                // 연속 패링 가능, 공격 중에도 가능
                if (playerPerformingAction.playerNetworkManager.isAttacking.Value // 공격중에도 패링 가능 
                    || playerPerformingAction.playerNetworkManager.isParrying.Value // 패링 중에도 패링 가능 
                    || !playerPerformingAction.isPerformingAction) // 공격, 패링 이외의 동작 중에는 패링 불가능. 
                {
                    // 우선 함수가 돌면서 패링 컴포넌트 안에서 쿨타임 체크 후 패링 실행한다.
                    // 특정 시간동안 만큼 패링 가능하게끔 만듦.
                    Debug.Log("클릭이 여러번된다는건가?");
                    InputManager.instance.BlockForSeconds(InputType.RightClick, 3f);
                    playerPerformingAction.parryingAction.AttempToPerformParrying();
                }
                break;
            default:
                break;
        }
    }


}
