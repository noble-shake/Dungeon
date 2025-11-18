using Sirenix.OdinInspector;
using UnityEngine;

public class ObjectDamageCollider : DamageCollider
{
    public bool isStaticHitDirection = false;
    public HitDirection hitDirection = HitDirection.xPositive;
    protected override void Awake()
    {
        if (damageCollider == null)
            damageCollider = GetComponent<Collider>();
        // damageCollider.enabled = false;

    }

    protected override void OnTriggerEnter(Collider other)
    {
        target = other.GetComponentInParent<CharacterManager>();

        // 얘는 플레이어만 때림. 패링 불가. 
        if (target.IsOwner && target.characterGroup == CharacterType.Player)
        {
            DamageProcess(target);
        }
    }

    protected override void HitProcess(CharacterManager damagedTarget)
    {
        if (CharactersDamaged.Contains(damagedTarget))
        {
            return;
        }
        // VFX.Play();

        CharactersDamaged.Add(damagedTarget);

        TakeInevitableHitEffect damageEffect = Instantiate(WorldCharacterEffectManager.instance.takeInevitableHitDamageEffect);
        // damageEffect.attackType = characterAttacking.characterCombatManager.currentAttackType;
        damageEffect.attackType = AttackType.HeavyAttack;
        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.holyDamage = holyDamage;
        damageEffect.groggyDamage = groggyDamage;

        // NOTE : 네트워크로 전파될 시 RPC로 할당해주는데, 클라이언트단에서 피격 처리를 해주면서 할당하게 되었음.
        ulong characterAttackingNetworkId = ulong.MaxValue;
        if (characterAttacking != null)
        {

            damageEffect.characterCausingDamage = characterAttacking;
            characterAttackingNetworkId = characterAttacking.NetworkObjectId;
        }
        else
        {
            damageEffect.characterCausingDamage = null;
            characterAttackingNetworkId = ulong.MaxValue;
        }



        damageEffect.contactPoint = contactPoint;
        if (damageEffect.characterCausingDamage != null)
            damageEffect.angleHitFrom = Vector3.SignedAngle(characterAttacking.transform.forward, damagedTarget.transform.forward, Vector3.up);
        else
            damageEffect.angleHitFrom = 0;


        damageEffect.vfxObjectPoolType = hitVfxObjectPoolType;
        damageEffect.poolIndex = index;


        switch (damagedTarget.characterGroup)
        {
            // 플레이어가 맞는 것은 기존의 로직으로 전혀 문제가 없음. 
            case CharacterType.Player:
                // 어차피 플레이어가 맞았다는 것은 damageTarget의 Owner가 나임.
                // 고로 이펙트를 실행시키면, VFX, SFX 실행 및 데미지 적용
                // 네트워크 상에서는 VFX, SFX 실행 전파 및 데미지 적용 X
                // 고로 지금처럼 해도 상관이 없음.
                damagedTarget.characterEffectManager.ProcessInstantEffct(damageEffect);

                // if (characterAttacking.IsOwner)
                {
                    // 맞는 애 
                    damagedTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
                        damagedTarget.NetworkObjectId,
                        characterAttackingNetworkId,
                        (int)DamageEffectType.InEvitableHit,
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
            // 플레이어가 몬스터를 때릴 때가 문제인데... 
            case CharacterType.Monster:
                // SFX, VFX만 먼저 실행
                if (!characterAttacking.IsHost)
                    damagedTarget.characterEffectManager.ProcessInstantEffectOnLocal(damageEffect);
                // 발동시킨 클라이언트에게는 SFX,VFX 외에 실행시켜주는 로직 필요.
                damagedTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
                    damagedTarget.NetworkObjectId,
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

    void OnEnable()
    {
        CharactersDamaged.Clear();
    }
}

public enum HitDirection
{
    xPositive,
    xNegative,
    yPositive,
    yNegative,
    zPositive,
    zNegative,

}

