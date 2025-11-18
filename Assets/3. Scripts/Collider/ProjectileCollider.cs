using UnityEngine;
using UnityEngine.Purchasing;

public class ProjectileCollider : DamageCollider
{
    ProjectileObject projectile;

    // public new CharacterType[] targetGroup;
    public bool isParryable;
    public bool isReflectable;
    public ParticleSystem particle;

    protected override void Awake()
    {
        if (damageCollider == null)
            damageCollider = GetComponent<Collider>();
        damageCollider.enabled = true;

        // 불가능함. 
        // characterAttacking = GetComponentInParent<CharacterManager>();

        projectile = GetComponent<ProjectileObject>();

        if (GetComponentsInChildren<ParticleSystem>().Length > 0) particle = GetComponentsInChildren<ParticleSystem>()[0];
    }

    protected override void OnTriggerEnter(Collider other)
    {

        // 벽에 부딪혔으면, 혹은 파괴될 수 있는 물체랑 부딪쳤으면 사라진다.
        if (other.gameObject.layer == LayerMask.NameToLayer("Destructable Collider"))
        {
            projectile.Remove();
            return;
        }

        // Case 2 : Target Check
        target = other.GetComponentInParent<CharacterManager>();
        if (target == null) return;

        // 플레이어가 맞을 때 
        if (target.IsOwner && target.characterGroup == CharacterType.Player && characterAttacking.characterGroup == CharacterType.Monster)
        {
            // 패링을 했다면 패링 처리가 되고 데미지를 입지 않는다. 
            if (target.TryGetComponent(out HeavyAttackAction_Parrying parryingComponent) != false && parryingComponent.canParrying)
            {
                if (CharactersDamaged.Contains(target))
                    return;
                CharactersDamaged.Add(target);
                if (isReflectable)
                {
                    // 아래 패링과 다른 것은, 카메라의 방향과 정반대로 밀려난다.
                    parryingComponent.ReflectionParrying(characterAttacking, projectile.objectPoolType, projectile.index);
                    Vector3 direction = Camera.main.transform.forward;
                    direction.y = 0;
                    direction.Normalize();
                    projectile.ReflectServerRpc(direction, true, new CharacterType[] { CharacterType.Monster }, target.NetworkObject);
                }
                else
                    // 공격한 캐릭터의 반대로 밀려난다.
                    parryingComponent.Parrying(characterAttacking);
                return;
            }
            // if (projectile.PenetratedCounter > 0) projectile.PenetrateCount();
            DamageProcess(target);
        }

        else if (target.characterGroup == CharacterType.Monster && characterAttacking.characterGroup == CharacterType.Player)
        {
            if (characterAttacking.IsOwner)
            {
                // 왠지 여기서 한번 더 걸러주지 않으면, 다단히트가 발동 됨. 
                // 아마 fixedUpdate 때 다단히트가 동시에 걸리며 생기는 문제 같음.
                if (CharactersDamaged.Contains(target))
                {
                    return;
                }
                DamageProcess(target);
            }

            // 어지러우니까 관통 로직은 제외 
            // if (projectile.PenetratedCounter > 0) projectile.PenetrateCount();
        }




        //else if (target.characterGroup == CharacterType.Monster && characterAttacking.IsOwner && characterAttacking.characterGroup == CharacterType.Player)

        // Case 3 : Penetrated Count Check
        // if (projectile.PenetratedCounter == 0)
        // {
        //     projectile.Remove();
        //     return;
        // }

    }

    protected override void DamageProcess(CharacterManager other)
    {
        base.DamageProcess(other);
        // if (target.isDead.Value)
        //     return;

        // // characterType is Inspector Value.
        // foreach (CharacterType characterType in targetGroup)
        // {
        //     if (target?.characterGroup == characterType)
        //         break;
        //     else
        //         target = null;
        // }

        // if (target == null)
        //     return;
        // else
        //     contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

        // GuardProcess(target);
        // HitProcess(target);
    }


    protected override void HitProcess(CharacterManager damageTarget)
    {
        if (CharactersDamaged.Contains(damageTarget))
        {
            return;
        }

        CharactersDamaged.Add(damageTarget);

        TakeHitEffect damageEffect = Instantiate(WorldCharacterEffectManager.instance.takeHitEffect);
        damageEffect.attackType = AttackType.HeavyAttack;
        // characterAttacking.characterCombatManager.currentAttackType;
        damageEffect.physicalDamage = characterAttacking.characterCombatManager.currentAttackType == AttackType.LightAttack ? physicalDamage : physicalDamage * 1.5f;
        damageEffect.magicDamage = magicDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.holyDamage = holyDamage;
        damageEffect.groggyDamage = groggyDamage;

        damageEffect.characterCausingDamage = characterAttacking;

        damageEffect.contactPoint = contactPoint;
        damageEffect.angleHitFrom = Vector3.SignedAngle(characterAttacking.transform.forward, damageTarget.transform.forward, Vector3.up);

        damageEffect.vfxObjectPoolType = hitVfxObjectPoolType;
        damageEffect.poolIndex = index; ;

        // damageTarget.characterEffectManager.ProcessInstantEffct(damageEffect);

        switch (damageTarget.characterGroup)
        {
            case CharacterType.Player:
                damageTarget.characterEffectManager.ProcessInstantEffct(damageEffect);

                // if (characterAttacking.IsOwner)
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
                        damageEffect.poolIndex,
                        blocked: false,
                        AITarget: false
                    );
                }
                break;
            case CharacterType.Monster:
                if (!characterAttacking.IsHost)
                    damageTarget.characterEffectManager.ProcessInstantEffectOnLocal(damageEffect);
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
                    damageEffect.poolIndex,
                    blocked: false,
                    AITarget: true
                );

                break;
        }



    }

    private void OnParticleCollision(GameObject other)
    {


        Debug.Log(other.layer);
        // Case 1 : Wall Check.

        // Case 2 : Target Check
        target = other.GetComponentInParent<CharacterManager>();
        if (target == null) return;
        if (target.IsOwner && target.characterGroup == CharacterType.Player && characterAttacking.characterGroup == CharacterType.Monster)
        {
            if (target.TryGetComponent(out HeavyAttackAction_Parrying parryingComponent) != false && parryingComponent.canParrying)
            {
                if (CharactersDamaged.Contains(target))
                    return;
                CharactersDamaged.Add(target);
                parryingComponent.Parrying(characterAttacking);

                return;
            }

            DamageProcess(target);
        }
        else if (target.characterGroup == CharacterType.Monster && characterAttacking.IsOwner && characterAttacking.characterGroup == CharacterType.Player)
        {
            DamageProcess(target);
        }

        // Case 3 : Penetrated Count Check
        if (projectile.PenetratedCounter == 0)
        {
            projectile.Remove();
            return;
        }
    }

    private void OnDisable()
    {
        CharactersDamaged.Clear();
    }

}