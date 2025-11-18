using BehaviorDesigner.Runtime;
using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    public CharacterManager characterAttacking;
    private PlayerManager playerAttacking;

    [Header("Target Type")]
    [SerializeField] private CharacterType[] targetGroup;

    [Header("Attack Target")]
    protected CharacterManager target;

    public Collider damageCollider;

    // 콜라이더의 데미지들은 애니메이션이 시작될 때 세팅해줌. <-과거
    // 현재는 캐릭터의 스탯을 기준으로 적용.
    [Header("Damage")]
    public float physicalDamage = 0;
    public float magicDamage = 0;
    public float fireDamage = 0;
    public float lighteningDamage = 0;
    public float holyDamage = 0f;
    public int damageMultiplicationRate = 0;

    [Header("Poise")]
    public float groggyDamage = 0;

    [Header("Contact Point")]
    public Vector3 contactPoint;

    [Header("Characters Damaged")]
    [SerializeField] private List<CharacterManager> charactersDamaged = new List<CharacterManager>();

    [Header("Block")]
    [Tooltip("이 공격이 방어될 수 있는지 여부")]
    [SerializeField] protected bool canBeBlocked = true;
    protected Vector3 directionFormAttackToDamageTarget;
    protected float dotValueFromAttackToDamageTarget;

    [Header("무적을 뚫고 때릴지 여부")]
    [Tooltip("특정 공격에만 타격이 먹힐 때, true 체크 시 특정 공격이 됨.")]
    public bool canAttackWhenInvulnerableState = false;

    public bool canAttackWhenGimmick = false;

    [Header("Hit VFX")]
    [SerializeField] protected ObjectPoolType hitVfxObjectPoolType;
    [SerializeField] protected int index;


    public CharacterType[] TargetGroup { get => targetGroup; set => targetGroup = value; }
    public List<CharacterManager> CharactersDamaged { get => charactersDamaged; set => charactersDamaged = value; }

    protected virtual void Awake()
    {
        if (damageCollider == null)
            damageCollider = GetComponent<Collider>();
        damageCollider.enabled = false;

        characterAttacking = GetComponentInParent<CharacterManager>();
        playerAttacking = GetComponentInParent<PlayerManager>();
    }

    /// <summary>
    /// 피해를 받을 수 있는 trigger화 된 collider에 닿았을 때 로직을 담당한다.
    /// </summary>
    /// <param name="other"></param>
    protected virtual void OnTriggerEnter(Collider other)
    {
        target = other.GetComponentInParent<CharacterManager>();
        // AI가 공격할 때, 내 화면 기준으로 맞을 때만 데미지 처리.
        // 즉, 내가 볼 때 맞은 것만 맞았다고 처리함.
        if (target.IsOwner && target.characterGroup == CharacterType.Player)
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
        } // 플레이어(나)가 AI를 공격할 때 내 화면 기준으로 데미지 처리.
        // 즉, 내가 봤을 때 때린 것 같으면 때린거임.

        // 개선사항? 
        // 피격가능 인터페이스를 가져옴.
        // 없으면 땡.
        // 있으면? 어떤 

        else if (target.characterGroup == CharacterType.Monster && characterAttacking.characterGroup == CharacterType.Player && characterAttacking.IsOwner)
        {
            MagicCircleProcess(target);

            DamageProcess(target);
        }
        else if (target.characterGroup == CharacterType.Object && characterAttacking.IsOwner)
        {
            PrisonProcess(target);
            // if (!characterAttacking.IsOwner) return;

        }
    }

    public void RayTrigger(CharacterManager target)
    {
        // AI가 공격할 때, 내 화면 기준으로 맞을 때만 데미지 처리.
        if (target.IsOwner && target.characterGroup == CharacterType.Player)
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
        } // 플레이어(나)가 AI를 공격할 때 내 화면 기준으로 데미지 처리.
        else if (target.characterGroup == CharacterType.Monster && characterAttacking.characterGroup == CharacterType.Player && characterAttacking.IsOwner)
        {
            MagicCircleProcess(target);

            DamageProcess(target);
        }
        else if (target.characterGroup == CharacterType.Object && characterAttacking.IsOwner)
        {
            PrisonProcess(target);
            // if (!characterAttacking.IsOwner) return;

        }
    }

    protected virtual void DamageProcess(CharacterManager target)
    {
        if (target.isDead.Value)
            return;

        // 타격할 수 있는 대상인지 판단한다.
        foreach (CharacterType characterType in TargetGroup)
        {
            // 타겟이 때릴 수 있는 타입이라면
            if (target?.characterGroup == characterType)
                break;
            else
                target = null;
        }

        if (target == null)
            return;
        else
            contactPoint = target.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

        if (target.characterNetworkManager.isPartiallyInvulnerable.Value)
        {
            if (canAttackWhenInvulnerableState == false)
            {
                // 기믹 진행 중이라면 타격을 하지 않는다.
                Debug.Log("이 공격은 무적상태의 캐릭터를 때릴 수 없습니다.");
                return;
            }
        }

        // 가드 상태에서 가드 가능한 공격을 받았을 시 처리
        // GuardProcess(target);
        // 피격 처리


        if (target.characterGroup == CharacterType.Monster)
        {
            EmblemHitProcess(target);
            // 플레이어가 맞으면 봉인을 풀고
            // 보스가 맞으면 기믹 횟수를 채운다.
            // SwordWaveHitProcess(target);
        }
        HitProcess(target);
    }

    // 플레이어가 보스를 때릴 떄
    // 플레이어가 플레이어를 때릴 때
    // 보스가 플레이어를 때릴 때
    // private void SwordWaveHitProcess(CharacterManager target)
    // {
    //     if (characterAttacking.TryGetComponent(out PlayerManager player))
    //     {

    //     }


    //     if (characterAttacking.TryGetComponent(out AIWarriorGolemCharacterManager boss))
    //     {

    //     }
    //     if (
    //         // target.TryGetComponent(out AIWarriorGolemCharacterManager boss))

    //     {
    //         print("내가 과연 여기서 실행되고 있을까? 1");
    //         if (boss.swordWavePattern.patternStarted && canAttackWhenGimmick)
    //         {
    //             print("내가 과연 여기서 실행되고 있을까? 2");
    //             if (CharactersDamaged.Contains(target))
    //                 return;
    //             CharactersDamaged.Add(target);
    //             print("내가 과연 여기서 실행되고 있을까? 3");

    //             TakeSwordWaveHitEffect damageEffect = Instantiate(WorldCharacterEffectManager.instance.takeSwordWaveHitEffect);
    //             // damageEffect.attackType = characterAttacking.characterCombatManager.currentAttackType;
    //             damageEffect.attackType = AttackType.HeavyAttack;
    //             damageEffect.physicalDamage = physicalDamage;
    //             damageEffect.magicDamage = magicDamage;
    //             damageEffect.fireDamage = fireDamage;
    //             damageEffect.holyDamage = holyDamage;
    //             damageEffect.groggyDamage = groggyDamage;

    //             // NOTE : 네트워크로 전파될 시 RPC로 할당해주는데, 클라이언트단에서 피격 처리를 해주면서 할당하게 되었음.
    //             ulong characterAttackingNetworkId = ulong.MaxValue;
    //             if (characterAttacking != null)
    //             {

    //                 damageEffect.characterCausingDamage = characterAttacking;
    //                 characterAttackingNetworkId = characterAttacking.NetworkObjectId;
    //             }
    //             else
    //             {
    //                 damageEffect.characterCausingDamage = null;
    //                 characterAttackingNetworkId = ulong.MaxValue;
    //             }



    //             damageEffect.contactPoint = contactPoint;
    //             if (damageEffect.characterCausingDamage != null)
    //                 damageEffect.angleHitFrom = Vector3.SignedAngle(characterAttacking.transform.forward, target.transform.forward, Vector3.up);
    //             else
    //                 damageEffect.angleHitFrom = 0;


    //             damageEffect.vfxObjectPoolType = hitVfxObjectPoolType;
    //             damageEffect.poolIndex = index;

    //             switch (target.characterGroup)
    //             {
    //                 case CharacterType.Monster:
    //                     if (!characterAttacking.IsHost)
    //                         target.characterEffectManager.ProcessInstantEffectOnLocal(damageEffect);
    //                     target.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
    //                         target.NetworkObjectId,
    //                         characterAttacking.NetworkObjectId,
    //                         (int)DamageEffectType.SwordWaveHit,
    //                         damageEffect.physicalDamage,
    //                         damageEffect.magicDamage,
    //                         damageEffect.fireDamage,
    //                         damageEffect.holyDamage,
    //                         damageEffect.groggyDamage,
    //                         damageEffect.angleHitFrom,
    //                         damageEffect.contactPoint.x,
    //                         damageEffect.contactPoint.y,
    //                         damageEffect.contactPoint.z,
    //                         damageEffect.attackType,
    //                         damageEffect.vfxObjectPoolType,
    //                         damageEffect.poolIndex,
    //                         blocked: false,
    //                         AITarget: true
    //                     );
    //                     break;
    //             }
    //         }
    //     }
    // }



    private void EmblemHitProcess(CharacterManager target)
    {
        if (target.TryGetComponent(out AIWarriorGolemCharacterManager boss))
        {
            if (boss.emblemPattern.patternStarted)
            {
                if (CharactersDamaged.Contains(target))
                    return;
                CharactersDamaged.Add(target);
                TakeEmblemHitEffect damageEffect = Instantiate(WorldCharacterEffectManager.instance.takeEmblemHitEffect);
                // damageEffect.attackType = characterAttacking.characterCombatManager.currentAttackType;
                damageEffect.attackType = AttackType.HeavyAttack;
                damageEffect.physicalDamage = physicalDamage;
                damageEffect.magicDamage = magicDamage;
                damageEffect.fireDamage = fireDamage;
                damageEffect.holyDamage = holyDamage;
                damageEffect.groggyDamage = groggyDamage;

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
                    damageEffect.angleHitFrom = Vector3.SignedAngle(characterAttacking.transform.forward, target.transform.forward, Vector3.up);
                else
                    damageEffect.angleHitFrom = 0;


                damageEffect.vfxObjectPoolType = hitVfxObjectPoolType;
                damageEffect.poolIndex = index;

                switch (target.characterGroup)
                {
                    case CharacterType.Monster:
                        if (!characterAttacking.IsHost)
                            target.characterEffectManager.ProcessInstantEffectOnLocal(damageEffect);
                        target.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
                            target.NetworkObjectId,
                            characterAttacking.NetworkObjectId,
                            (int)DamageEffectType.EmblemHit,
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
        }
    }

    protected virtual void HitProcess(CharacterManager damagedTarget)
    {
        if (CharactersDamaged.Contains(damagedTarget))
        {
            return;
        }
        CharactersDamaged.Add(damagedTarget);

        TakeHitEffect damageEffect = Instantiate(WorldCharacterEffectManager.instance.takeHitEffect);
        damageEffect.attackType = characterAttacking.characterCombatManager.currentAttackType;
        //damageEffect.attackType = AttackType.HeavyAttack;

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
            // 어차피 플레이어가 맞았다는 것은 damageTarget의 Owner가 나임.
            // 고로 이펙트를 실행시키면, VFX, SFX 실행 및 데미지 적용
            // 네트워크 상에서는 VFX, SFX 실행 전파 및 데미지 적용 X
            // 고로 지금처럼 해도 상관이 없음.
            case CharacterType.Player:
                characterAttacking.TryGetComponent<EnemyAudioManager>(out EnemyAudioManager enemy_emitter);
                if (enemy_emitter != null)
                {
                    if (damageEffect.attackType == AttackType.LightAttack) enemy_emitter.SFXHit(damagedTarget.transform);
                    if (damageEffect.attackType == AttackType.HeavyAttack)
                    {
                        enemy_emitter.SFXHeavyHit(damagedTarget.transform);
                    }
                }
                damagedTarget.characterEffectManager.ProcessInstantEffct(damageEffect);
                damageEffect.attackType = AttackType.HeavyAttack;
                damagedTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
                    damagedTarget.NetworkObjectId,
                    characterAttackingNetworkId,
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
                break;
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

    protected virtual void GetBlockingDotValues(CharacterManager damageTarget)
    {
        // 공격 무기로부터 피격부위로 향하는 벡터
        directionFormAttackToDamageTarget = characterAttacking.transform.position - damageTarget.transform.position;
        // 위의 벡터와 맞는 캐릭터의 정면의 내적값 == 공격이 얼마나 정면에서 들어왔는지를 판단해준다.
        dotValueFromAttackToDamageTarget = Vector3.Dot(directionFormAttackToDamageTarget, damageTarget.transform.forward);
    }

    public virtual void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    // TODO : 공격을 하다 죽어서 데미지 콜라이더가 안 꺼질 때도 존재함.
    public virtual void DisableDamageCollider()
    {
        damageCollider.enabled = false;
        CharactersDamaged.Clear();
    }

    public void SetDamage(float damage)
    {
        physicalDamage = damage;
    }

    protected virtual void MagicCircleProcess(CharacterManager damageTarget)
    {
        if (damageTarget.GetComponent<WarriorGolemPattern3Component>().isMagicCirclePatternPlaying == false)
        {
            return;
        }

        if (CharactersDamaged.Contains(damageTarget))
            return;

        CharactersDamaged.Add(damageTarget);

        TakeBoss1MagicCircleEffect damageEffect = Instantiate(WorldCharacterEffectManager.instance.takeBoss1MagicCircleEffect);
        damageEffect.attackType = AttackType.ObjectAttack;
        damageEffect.physicalDamage = 0f;
        damageEffect.magicDamage = 0f;
        damageEffect.fireDamage = 0f;
        damageEffect.holyDamage = 0f;
        damageEffect.groggyDamage = 0f;

        // NOTE : 네트워크로 전파될 시 RPC로 할당해주는데, 클라이언트단에서 피격 처리를 해주면서 할당하게 되었음.
        damageEffect.characterCausingDamage = characterAttacking;

        damageEffect.contactPoint = contactPoint;
        damageEffect.angleHitFrom = Vector3.SignedAngle(characterAttacking.transform.forward, damageTarget.transform.forward, Vector3.up);

        damageTarget.characterEffectManager.ProcessInstantEffct(damageEffect);

        if (!characterAttacking.IsHost)
            damageTarget.characterEffectManager.ProcessInstantEffectOnLocal(damageEffect);
        // 발동시킨 클라이언트에게는 SFX,VFX만 실행 안 시켜주는 로직 필요.
        damageTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
            damageTarget.NetworkObjectId,
            characterAttacking.NetworkObjectId,
            (int)DamageEffectType.MagicCircleHit,
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
    }



    protected virtual void PrisonProcess(CharacterManager damageTarget)
    {

        if (CharactersDamaged.Contains(damageTarget))
            return;

        CharactersDamaged.Add(damageTarget);

        TakeBoss1PrisonEffect damageEffect = Instantiate(WorldCharacterEffectManager.instance.takeBoss1PrisonEffect);
        damageEffect.attackType = AttackType.PrisonAttack;
        damageEffect.physicalDamage = characterAttacking.characterCombatManager.currentAttackType == AttackType.LightAttack ? physicalDamage : physicalDamage * 1.5f;
        damageEffect.magicDamage = magicDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.holyDamage = holyDamage;
        damageEffect.groggyDamage = groggyDamage;

        // NOTE : 네트워크로 전파될 시 RPC로 할당해주는데, 클라이언트단에서 피격 처리를 해주면서 할당하게 되었음.
        damageEffect.characterCausingDamage = characterAttacking;

        damageEffect.contactPoint = contactPoint;
        damageEffect.angleHitFrom = Vector3.SignedAngle(characterAttacking.transform.forward, damageTarget.transform.forward, Vector3.up);

        //if (!characterAttacking.IsHost)
        //    damageTarget.characterEffectManager.ProcessInstantEffectOnLocal(damageEffect);
        // 발동시킨 클라이언트에게는 SFX,VFX만 실행 안 시켜주는 로직 필요.
        damageTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
            damageTarget.NetworkObjectId,
            characterAttacking.NetworkObjectId,
            (int)DamageEffectType.PrisonHit,
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
    }

    private void SealHitProcess(CharacterManager damageTarget)
    {
        if (damageTarget.TryGetComponent(out PlayerManager player))
        {
            if (player.playerNetworkManager.isSealed.Value)
            {
                if (CharactersDamaged.Contains(damageTarget))
                    return;

                Debug.Log("봉인된 애를 때리고 있음.");
                CharactersDamaged.Add(damageTarget);

                // 얘가 처리해야 될 효과는 VFX, SFX, 그리고 피격 횟수 덧셈뿐. 
                // 로컬에서 VFX, SFX만 처리하고 서버에게는 피격 횟수 덧셈만 요구.
                // 
                TakeSealHitEffect damageEffect = Instantiate(WorldCharacterEffectManager.instance.takeSealHitEffect);
                damageEffect.characterCausingDamage = characterAttacking;

                damageEffect.contactPoint = contactPoint;
                damageEffect.attackType = AttackType.SealAttack;
                damageTarget.characterEffectManager.ProcessInstantEffectOnLocal(damageEffect);
                // 발동시킨 클라이언트에게는 SFX,VFX만 실행 안 시켜주는 로직 필요.

                damageTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
                    damageTarget.NetworkObjectId,
                    characterAttacking.NetworkObjectId,
                    (int)DamageEffectType.SealHit,
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
                return;
            }

        }

        return;
    }
    // 타겟이 보스일 때
    // 보스가 기믹실행 중일 때
    // 보스가 타격횟수로써의 데미지만 입을 때
    // protected virtual void HitInGimmickProcess(CharacterManager damageTarget)
    // {
    //     if (charactersDamaged.Contains(damageTarget))
    //         return;
    //     if (damageTarget.characterGroup != CharacterType.Monster)
    //         return;

    // }

    // protected virtual void GuardProcess(CharacterManager damageTarget)
    // {
    //     if (!canBeBlocked) return;
    //     // 이미 공격을 받은 상태라면 리턴 -> 이 코드가 있기 때문에 다단히트도 안될 뿐더러 가드를 안했을 시의 HitProcess도 실행이 되지 않는다.
    //     if (CharactersDamaged.Contains(damageTarget))
    //         return;

    //     // 가드 시의 각도를 계산한다.
    //     GetBlockingDotValues(damageTarget);

    //     // 데미지 받고 있는 캐릭터가 방어중인지 체크 && 캐릭터가 방어중이라면, 올바른 방향으로 방어중인지 체크한다.
    //     if (damageTarget.characterNetworkManager.isGaurding.Value && dotValueFromAttackToDamageTarget > 0.3f)
    //     {
    //         CharactersDamaged.Add(damageTarget);

    //         TakeBlockEffect damageEffect = Instantiate(WorldCharacterEffectManager.instance.takeBlockedDamageEffect);
    //         damageEffect.attackType = characterAttacking.characterCombatManager.currentAttackType;

    //         damageEffect.physicalDamage = physicalDamage;
    //         damageEffect.magicDamage = magicDamage;
    //         damageEffect.fireDamage = fireDamage;
    //         damageEffect.holyDamage = holyDamage;
    //         damageEffect.groggyDamage = groggyDamage;
    //         damageEffect.contactPoint = contactPoint;

    //         // damageTarget.characterEffectManager.ProcessInstantEffct(damageEffect);
    //         if (characterAttacking.IsOwner)
    //         {
    //             damageTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
    //                 damageTarget.NetworkObjectId,
    //                 characterAttacking.NetworkObjectId,
    //                 damageEffect.physicalDamage,
    //                 damageEffect.magicDamage,
    //                 damageEffect.fireDamage,
    //                 damageEffect.holyDamage,
    //                 damageEffect.groggyDamage,
    //                 damageEffect.angleHitFrom,
    //                 damageEffect.contactPoint.x,
    //                 damageEffect.contactPoint.y,
    //                 damageEffect.contactPoint.z,
    //                 damageEffect.attackType,
    //                 damageEffect.vfxObjectPoolType,
    //                 damageEffect.poolIndex,
    //                 blocked: true
    //             );
    //         }
    //     }
    // }
}
