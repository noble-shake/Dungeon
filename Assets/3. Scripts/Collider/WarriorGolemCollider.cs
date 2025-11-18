
// using UnityEngine;

// public class WarriorGolemCollider : DamageCollider
// {

//     /// <summary>
//     /// 피해를 받을 수 있는 trigger화 된 collider에 닿았을 때 로직을 담당한다.
//     /// </summary>
//     /// <param name="other"></param>
//     protected virtual void OnTriggerEnter(Collider other)
//     {
//         // 공격하는 캐릭터가 몬스터라면 호스트 기준
//         // 공격하는 캐릭터가 플레이어라면 그 플레리어 로컬 기준으로 적용 된다. 

//         target = other.GetComponentInParent<CharacterManager>();
//         // AI가 공격할 때, 내 화면 기준으로 맞을 때만 데미지 처리.
//         if (target.IsOwner && target.characterGroup == CharacterType.Player)
//         {
//             if (target.TryGetComponent(out HeavyAttackAction_Parrying parryingComponent) != false && parryingComponent.canParrying)
//             {
//                 if (CharactersDamaged.Contains(target))
//                     return;
//                 CharactersDamaged.Add(target);
//                 Debug.Log("이쪽으로 오는건 아니지?");
//                 parryingComponent.Parrying(characterAttacking);

//                 return;
//             }
//             DamageProcess(target);
//         } // 내가 AI를 공격할 때 내 화면 기준으로 데미지 처리.
//         else if (target.characterGroup == CharacterType.Monster && characterAttacking.IsOwner)
//         {
//             DamageProcess(target);
//         }

//         // if (!characterAttacking.IsOwner) return;

//     }

//     private void DamageProcess(CharacterManager other)
//     {

//         if (target.isDead.Value)
//             return;

//         // 타격할 수 있는 대상인지 판단한다.
//         foreach (CharacterType characterType in TargetGroup)
//         {
//             // 타겟이 때릴 수 있는 타입이라면면
//             if (target?.characterGroup == characterType)
//                 break;
//             else
//                 target = null;
//         }

//         if (target == null)
//             return;
//         else
//             contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

//         // MagicCircleProcess(target);

//         HitProcess(target);
//     }


// }
