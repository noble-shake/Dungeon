using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponDamageCollider : DamageCollider
{
    protected override void Awake()
    {
        base.Awake();

        damageCollider.enabled = false;
    }

    // 
    protected override void HitProcess(CharacterManager damageTarget)
    {
        if (CharactersDamaged.Contains(damageTarget))
        {
            return;
        }

        CharactersDamaged.Add(damageTarget);

        TakeHitEffect damageEffect = Instantiate(WorldCharacterEffectManager.instance.takeHitEffect);
        damageEffect.attackType = characterAttacking.characterCombatManager.currentAttackType;

        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.holyDamage = holyDamage;
        damageEffect.groggyDamage = groggyDamage;

        damageEffect.contactPoint = contactPoint;
        damageEffect.angleHitFrom = Vector3.SignedAngle(characterAttacking.transform.forward, damageTarget.transform.forward, Vector3.up);

        // 주석 처리된 이유는 캐릭터의 피격 처리를 ClientRPC로 처리하기 때문.
        // 이러면 피격되는 순간이 동기화되긴 함. 다만 공격을 가한 쪽 입장의 화면에선 약간의 지연이 있을 수 있음. 
        // damageTarget.characterEffectManager.ProcessInstantEffct(damageEffect);

        switch (characterAttacking.characterCombatManager.currentAttackType)
        {

            default:
                break;
        }

        // 공격한 캐릭터가 소유한 캐릭터라면 서버에게 피격 처리를 알림. 
        if (characterAttacking.IsOwner)
        {
            damageTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
                damageTarget.NetworkObjectId,
                characterAttacking.NetworkObjectId,
                (int)DamageEffectType.Hit,
                damageEffect.physicalDamage,
                damageEffect.magicDamage,
                damageEffect.fireDamage,
                damageEffect.holyDamage,
                damageEffect.groggyDamage,
                damageEffect.angleHitFrom,
                damageEffect.contactPoint.x,
                damageEffect.contactPoint.y,
                damageEffect.contactPoint.z,
                damageEffect.attackType,
                damageEffect.vfxObjectPoolType,
                damageEffect.poolIndex
            );
        }
    }



}
