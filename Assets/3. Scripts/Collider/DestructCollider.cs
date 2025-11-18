using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;



public class DestructCollider : DamageCollider
{

    // 돌진을 멈추는 충돌은 IDestructable을 가진 객체와 충돌했을 때.

    // 그 외의 충돌은 플레이어와의 충돌인데 이 때 헤비어택 판정을 가진다.

    protected override void OnTriggerEnter(Collider other)
    {
        // 공격하는 캐릭터가 몬스터라면 호스트 기준
        // 공격하는 캐릭터가 플레이어라면 그 플레리어 로컬 기준으로 적용 된다. 

        if (!characterAttacking.IsOwner) return;
        // 부딪쳤다면 러시다운 애니메이션 실행 

        // 타겟이 부서질 수 있는가?
        IDestructable destructable = other.GetComponent<IDestructable>();
        if (destructable != null)
        {
            characterAttacking.GetComponent<RushUntilCollision>().HasCollision = true;
            if (destructable.isGimmickObject())
            {
                characterAttacking.GetComponent<RushUntilCollision>().HasCollisionWithGimmickObject = true;
            }
            characterAttacking.characterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Hit, "RushDown", true);
            destructable.BeingDestructed();
            // 부술 때 애니메이션이 있는가?

            RushUntilCollision rushComponent = characterAttacking.GetComponent<RushUntilCollision>();
            if (rushComponent != null)
            {
                if (rushComponent.OnRush)
                {
                    rushComponent.OnRush = false;
                    DisableDamageCollider();
                }
            }
        }


        Debug.Log($"부딪힌 대상 : {other.name}");
        target = other.GetComponentInParent<CharacterManager>();
        if (target.isDead.Value)
            return;

        // 타격할 수 있는 대상인지 판단한다.
        foreach (CharacterType characterType in TargetGroup)
        {
            // 타겟이 때릴 수 있는 타입이라면면
            if (target?.characterGroup == characterType)
                break;
            else
                target = null;
        }

        if (target == null)
            return;
        else
            contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
        if (target != null)
            HitProcess(target);

    }

    // protected override void HitProcess(CharacterManager damageTarget)
    // {
    //     if (charactersDamaged.Contains(damageTarget))
    //     {
    //         return;
    //     }
    //     // VFX.Play();

    //     charactersDamaged.Add(damageTarget);

    //     Debug.Log("나 실행 됐어");
    //     TakeHitEffect damageEffect = Instantiate(WorldCharacterEffectManager.instance.takeDamageEffect);
    //     damageEffect.attackType = AttackType.HeavyAttack;
    //     damageEffect.physicalDamage = characterAttacking.characterCombatManager.currentAttackType == AttackType.LightAttack ? physicalDamage : physicalDamage * 1.5f;
    //     damageEffect.magicDamage = magicDamage;
    //     damageEffect.fireDamage = fireDamage;
    //     damageEffect.holyDamage = holyDamage;
    //     damageEffect.poiseDamage = poiseDamage; // 
    //     // damageEffect.stamina

    //     damageEffect.contactPoint = contactPoint;
    //     damageEffect.angleHitFrom = Vector3.SignedAngle(characterAttacking.transform.forward, damageTarget.transform.forward, Vector3.up);

    //     // damageTarget.characterEffectManager.ProcessInstantEffct(damageEffect);
    //     if (characterAttacking.IsOwner)
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
    //             damageEffect.contactPoint.z,
    //             damageEffect.attackType
    //         );
    //     }
    // }
}
