using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class CharacterCombatManager : NetworkBehaviour
{
    protected CharacterManager character;

    [HideInInspector] public string lastAttackAnimationPerformed;

    [Header("캐릭터 체력 및 스태미나")]
    public int hp = 5;
    public int stamina = 5;

    // [Header("Hit Effect")]
    // public GameObject hitEffectPerClass;
    // public int hitEffectIndex;
    [HideInInspector] public CharacterManager currentTarget;
    [HideInInspector] public Vector3 targetPosition = Vector3.zero;
    [HideInInspector] public AttackType currentAttackType;
    [SerializeField] public Transform lockOnTransform;

    [Header("Trail VFX")]


    [HideInInspector] public bool canPerformRollingAttack = false;
    [HideInInspector] public bool canPerformBackstepAttack = false;
    [HideInInspector] public bool canGuard = true; // 이게 true면 캐릭터가 방어를 할 수 있다.
    [HideInInspector] public bool canPerformCounterAttack = false;
    [HideInInspector] public bool canDoEvade = false;
    [HideInInspector] public bool canDoDoubleDash = false;
    [HideInInspector] public bool canDoCombo = false;


    [Header("Poise")]
    public float totalPoiseDamage;                   // 최종적으로 받는 포즈 데미지
    public float offensivePoiseBonus;                // 내가 가할 때의 포즈 데미지 보너스
    public float basePoiseDefense;                   // 기본적으로 갖고 있는 포즈 데미지
    public float defaultPoiseResetTime = 0;
    public float poiseResetTimer = 0;


    [Header("강화 버프 설정")]
    [SerializeField] private float attackIncreaseRate = 0f; // 공격력 증가율율
    [SerializeField] private float damageDecreaseRate = 0f; // 방어력 증가율
    [SerializeField] private float enchanctTime = 0; // 버프 지속시간
    // 데미지는 받는 무적
    private int superArmorCount = 0;
    public int SuperArmorCount
    {
        get => superArmorCount;
        set => superArmorCount = value;
    }
    public float AttackIncreaseRate { get => attackIncreaseRate; set => attackIncreaseRate = value; }
    public float DamageDecreaseRate { get => damageDecreaseRate; set => damageDecreaseRate = value; }

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();

    }
    protected virtual void Update()
    {
        HandlePoiseResetTimer();
    }

    private void HandlePoiseResetTimer()
    {
        if (poiseResetTimer > 0)
        {
            poiseResetTimer -= Time.deltaTime;
        }
        else
        {
            totalPoiseDamage = 0;
        }
    }

    // public int CalculateHealthBasedOnVitalityLevel(int newHp)
    // {
    //     float health = 0f;

    //     health = newHp * vitalityMul;
    //     return Mathf.RoundToInt(health);
    // }

    // public int CalculateStaminaBasedOnEnduranceLevel(int endurance)
    // {
    //     float stamina = 0f;

    //     stamina = endurance * enduranceMul;
    //     return Mathf.RoundToInt(stamina);
    // }

    public virtual void SetTarget(CharacterManager newTarget)
    {
        if (character.IsOwner)
        {
            if (newTarget != null)
            {
                currentTarget = newTarget;
                character.characterNetworkManager.currentTargetNetworkObjectID.Value = newTarget.GetComponent<NetworkObject>().NetworkObjectId;

            }
            else
            {
                currentTarget = null;
                character.characterNetworkManager.currentTargetNetworkObjectID.Value = 987654321;
            }
        }
    }

    public void EnableIsRipostable()
    {
        if (character.IsOwner)
            character.characterNetworkManager.isRipostable.Value = true;
    }

    public void EnableIsInvulnerable()
    {
        if (character.IsOwner)
            character.characterNetworkManager.isInvulnerable.Value = true;
    }

    public void DisableIsInvulnerable()
    {
        if (character.IsOwner)
            character.characterNetworkManager.isInvulnerable.Value = false;
    }

    public void EnableCanDoRollingAttack()
    {
        canPerformRollingAttack = true;
    }
    public void DisableCanDoRollingAttack()
    {
        canPerformRollingAttack = false;
    }

    public void EnableCanDoBackstepAttack()
    {
        canPerformBackstepAttack = true;
    }
    public void DisableCanDoBackstepAttack()
    {
        canPerformBackstepAttack = false;
    }

    public void EnableCanDoCounterAttack()
    {
        canPerformCounterAttack = true;
    }

    public void DisableCanDoCounterAttack()
    {
        canPerformCounterAttack = false;
    }

    public void EnableTrailVFX()
    {

    }

    public void DisableTrailVFX()
    {

    }

    public virtual void EnableCanDoCombo() { }
    public virtual void DisableCanDoCombo() { }
    public virtual void EnableCanDoEvade() { }
    public virtual void DisableCanDoEvade() { }
    public virtual void EnableCanDoDoubleDash() { }
    public virtual void DisableCanDoDoubleDash() { }
    public virtual void InitializeLastPlayedAnimation() { }
    public virtual void OpenAllDamageCollider() { }
    public virtual void CloseAllDamageCollider() { }
    public virtual void SetStanceBroken() { }
    public virtual void ResetStanceBroken() { }

    public virtual void EnableTrailRenderer() { }
    public virtual void DisableTrailRenderer() { }

    public virtual void ShootProjectile() { }

    // 워리어 궁극기 전용 함수
    public void Enchant(int superArmorCount, float attackIncreaseRate, float damageDecreaseRate, float interceptAggroTime)
    {
        EnchantForSeconds(superArmorCount, attackIncreaseRate, damageDecreaseRate, interceptAggroTime).Forget();
    }
    [ServerRpc]
    public void EnchantServerRpc(int superArmorCount, float attackIncreaseRate, float damageDecreaseRate, float interceptAggroTime)
    {
        EnchantForSeconds(superArmorCount, attackIncreaseRate, damageDecreaseRate, interceptAggroTime).Forget();
    }

    private async UniTaskVoid EnchantForSeconds(int superArmorCount, float attackIncreaseRate, float damageDecreaseRate, float enchanctTime)
    {
        // 슈퍼아머머 횟수
        // TakeHitEffect에서 피격 애니메이션 실행을 한번 막아준다.
        this.superArmorCount = superArmorCount;
        // 공격력 증가율
        // damagecollider에서 적용된다.
        AttackIncreaseRate = attackIncreaseRate;
        // 방어력 증가율
        // TakeHitEffect에서 적용된다.
        DamageDecreaseRate = damageDecreaseRate;
        // 버프 지속시간
        this.enchanctTime = enchanctTime;
        // 버프가 지속되는 시간
        await UniTask.Delay(TimeSpan.FromSeconds(enchanctTime));

        this.superArmorCount = 0;
        AttackIncreaseRate = 0f;
        DamageDecreaseRate = 0f;
        this.enchanctTime = 0f;
    }
}
