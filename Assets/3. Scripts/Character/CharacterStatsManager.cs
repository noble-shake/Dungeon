using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    CharacterManager character;
    // [Header("Stamina Regeneration")]
    // [SerializeField] float staminaRegenerationAmount = 2;
    // private float staminaRegenerationTimer = 0f;
    // private float staminaTickTimer = 0;
    // [SerializeField] float staminaRegenerationDelay = 2f;

    [Header("공격 방어 시 흡수율")]
    public float blockingPhysicalAbsorption;
    public float blockingFireAbsorption;
    public float blockingMagicAbsorption;
    public float blockingLighteningAbsorption;
    public float blockingHolyAbsorption;
    public float blockingStability;

    // [Header("캐릭터 스탯 및 배율")]
    // public int vatality = 5;
    // public int endurance = 5;
    // [SerializeField] protected int vitalityMul = 100;
    // [SerializeField] protected int enduranceMul = 100;

    // [Header("Poise")]
    // public float totalPoiseDamage;                   // 최종적으로 받는 포즈 데미지
    // public float offensivePoiseBonus;                // 내가 가할 때의 포즈 데미지 보너스
    // public float basePoiseDefense;                   // 기본적으로 갖고 있는 포즈 데미지
    // public float defaultPoiseResetTime = 0;
    // public float poiseResetTimer = 0;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    protected virtual void Start()
    {

    }

    // protected virtual void Update()
    // {
    //     HandlePoiseResetTimer();
    // }

    // private void HandlePoiseResetTimer()
    // {
    //     if (poiseResetTimer > 0)
    //     {
    //         poiseResetTimer -= Time.deltaTime;
    //     }
    //     else
    //     {
    //         totalPoiseDamage = 0;
    //     }
    // }

    // public int CalculateHealthBasedOnVitalityLevel(int vitality)
    // {
    //     float health = 0f;

    //     health = vitality * vitalityMul;
    //     return Mathf.RoundToInt(health);
    // }

    // public int CalculateStaminaBasedOnEnduranceLevel(int endurance)
    // {
    //     float stamina = 0f;

    //     stamina = endurance * enduranceMul;
    //     return Mathf.RoundToInt(stamina);
    // }


    // 스태미나 관련 코드 주석 처리
    // public virtual void RegenerateStamina()
    // {
    //     if (!character.IsOwner) return;

    //     if (character.characterNetworkManager.isSprinting.Value) return;

    //     if (character.isPerformingAction) return;

    //     staminaRegenerationTimer += Time.deltaTime;
    //     if (staminaRegenerationTimer >= staminaRegenerationDelay)
    //     {
    //         if (character.characterNetworkManager.currentStamina.Value < character.characterNetworkManager.maxStamina.Value)
    //         {
    //             staminaTickTimer += Time.deltaTime;
    //             if (staminaTickTimer >= 0.1)
    //             {
    //                 staminaTickTimer = 0;
    //                 character.characterNetworkManager.currentStamina.Value += staminaRegenerationAmount;
    //             }
    //         }
    //     }
    // }

    // public virtual void ResetStaminaRegenTimer(float previousStaminaAmount, float currentStaminaAmount)
    // {
    //     if (currentStaminaAmount < previousStaminaAmount)
    //     {
    //         staminaRegenerationTimer = 0;
    //     }
    // }


}
