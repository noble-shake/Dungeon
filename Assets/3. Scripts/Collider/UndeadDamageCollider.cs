using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 다른 무기에 달려있는 데미지 콜라이더와 다른 점이 있다면, 얘는 처음부터 달려있다. 
// 따라서 awake에서 characterAttacking 등이 할당 되지만 MeleeWeaponCollider는 직접 할당해줘야 한다.
// awake에서 null로 할당되어 있다. 
public class UndeadDamageCollider : DamageCollider
{
    protected override void Awake()
    {
        base.Awake();
    }

    // protected override void OnTriggerEnter(Collider other)
    // {
    //     if (other.gameObject.GetComponentInParent<CharacterManager>().characterGroup == CharacterType.Monster)
    //         return;
    //     base.OnTriggerEnter(other);
    // }

    // protected override void HitProcess(CharacterManager damageTarget)
    // {
    //     if (charactersDamaged.Contains(damageTarget))
    //     {
    //         return;
    //     }
    //     Debug.Log("언데드 공격 프로세스 시작");

    //     charactersDamaged.Add(damageTarget);

    //     TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectManager.instance.takeDamageEffect);
    //     damageEffect.physicalDamage = physicalDamage;
    //     damageEffect.magicDamage = magicDamage;
    //     damageEffect.fireDamage = fireDamage;
    //     damageEffect.holyDamage = holyDamage;
    //     damageEffect.poiseDamage = poiseDamage;

    //     damageEffect.contactPoint = contactPoint;
    //     damageEffect.angleHitFrom = Vector3.SignedAngle(characterAttacking.transform.forward, damageTarget.transform.forward, Vector3.up);

    //     Debug.Log("가한 데미지 : " + physicalDamage);
    //     // damageTarget.characterEffectManager.ProcessInstantEffct(damageEffect);

    //     // 이 코드는 NPC한테 맞는 것처럼 보이는 클라이언트를 토대로 데미지를 적용한다. 이 외의 클라이언트들에게 어떻게 보일지는 신경 안 씀.
    //     if (damageTarget.IsOwner)
    //     {
    //         damageTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
    //             damageTarget.NetworkObjectId,
    //             characterAttacking.NetworkObjectId,
    //             damageEffect.physicalDamage,
    //             damageEffect.magicDamage,
    //             damageEffect.fireDamage,
    //             damageEffect.holyDamage,
    //             damageEffect.poiseDamage,
    //             damageEffect.angleHitFrom,
    //             damageEffect.contactPoint.x,
    //             damageEffect.contactPoint.y,
    //             damageEffect.contactPoint.z
    //         );
    //     }
    // }

    // protected override void GetBlockingDotValues(CharacterManager damageTarget)
    // {
    //     directionFormAttackToDamageTarget = characterAttacking.transform.position - damageTarget.transform.position;
    //     dotValueFromAttackToDamageTarget = Vector3.Dot(directionFormAttackToDamageTarget, damageTarget.transform.forward);
    // }

}
