using System.Collections;
using System.Collections.Generic;
// using UnityEditor.Animations;

// using UnityEditor.Animations;
using UnityEngine;

public class Weapon : Item
{
    [Header("Animations")]
    public RuntimeAnimatorController weaponAnimator;

    [Header("Model Instantiation")]
    public WeaponModelType weaponModelType;

    [Header("Weapon Model")]
    public GameObject weaponModel;

    [Header("Weapon Requirements")]
    public int strengthREQ = 0;
    public int dexREQ = 0;
    public int intREQ = 0;
    public int faithREQ = 0;

    [Header("Weapon Base Damage")]
    public int physicalDamage = 0;
    public int magicDamage = 0;
    public int fireDamage = 0;
    public int holyDamage = 0;
    public int lighteningDamage = 0;

    [Header("Weapon Groggy Damage")]
    public float groggyDamage = 10;

    [Header("Action")]
    public WeaponAction lightAttackAction; // 약공격
    public WeaponAction heavyAttackAction; // 강공격
    public WeaponAction specialAttackAction_01; // 대시공격
    public WeaponAction specialAttackAction_02; // 대시공격
    public WeaponAction evadeAction; // 회피액션(회피, 가드)
    public WeaponAction buffAction; // 회피액션(회피, 가드)

    [Header("Whooshes")] // 휘두르는 소리들
    public AudioClip[] light_whooshes;
    public AudioClip[] medium_whooshes;
    public AudioClip[] heavy_whooshes;

    // [Header("Stamina Costs")]
    // public int baseStaminaCost = 20;
    // public float lightAttackStaminaCostMultiplier = 0.9f;
    // public float heavyAttackStaminaCostMultiplier = 1.3f;
    // public float chargedAttackStaminaCostMultiplier = 1.5f;
    // public float runningAttackStaminaCostMultiplier = 1.1f;
    // public float rollingAttackStaminaCostMultiplier = 1.1f;
    // public float backstepAttackStaminaCostMultiplier = 1.1f;

    // [Header("Weapon Blocking Absorption")]
    // public float physicalBaseDamageAbsorption = 50;
    // public float magicBaseDamageAbsorption = 50;
    // public float fireBaseDamageAbsorption = 50;
    // public float holyBaseDamageAbsorption = 50;
    // public float lighteningBaseDamageAbsorption = 50;
    // public float stability = 50;


}
