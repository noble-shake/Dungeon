using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIUndeadCombatManager : AICharacterCombatManager
{
    [Header("Damage Colliders")]
    [SerializeField] UndeadDamageCollider rightHandDamageCollider;
    [SerializeField] UndeadDamageCollider leftHandDamageCollider;
    [SerializeField] UndeadDamageCollider bodyDamageCollider;

    [Header("Damage")]
    [SerializeField] int baseDamage = 25;
    [SerializeField] int basePoiseDamage = 25;
    [SerializeField] float attack01DamageModifier = 1.0f;
    [SerializeField] float attack02DamageModifier = 1.4f;


    protected override void Awake()
    {
        base.Awake();

    }

    public void SetAttack01Damage()
    {
        rightHandDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;
        leftHandDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;

        rightHandDamageCollider.groggyDamage = basePoiseDamage * attack01DamageModifier;
        leftHandDamageCollider.groggyDamage = basePoiseDamage * attack01DamageModifier;
    }

    public void SetAttack02Damage()
    {
        rightHandDamageCollider.physicalDamage = baseDamage * attack02DamageModifier;
        leftHandDamageCollider.physicalDamage = baseDamage * attack02DamageModifier;

        rightHandDamageCollider.groggyDamage = basePoiseDamage * attack02DamageModifier;
        leftHandDamageCollider.groggyDamage = basePoiseDamage * attack02DamageModifier;
    }

    public void EnableRightHandDamageCollider()
    {
        rightHandDamageCollider.EnableDamageCollider();
    }

    public void DisableRightHandDamageCollider()
    {
        rightHandDamageCollider.DisableDamageCollider();
    }

    public void EnableLeftHandDamageCollider()
    {
        leftHandDamageCollider.EnableDamageCollider();
    }

    public void DisableLeftHandDamageCollider()
    {
        leftHandDamageCollider.DisableDamageCollider();
    }

    public void EnableBodyDamageCollider()
    {
        bodyDamageCollider.EnableDamageCollider();
    }

    public void DisableBodyDamageCollider()
    {
        bodyDamageCollider.DisableDamageCollider();
    }

    public override void CloseAllDamageCollider()
    {
        rightHandDamageCollider?.DisableDamageCollider();
        leftHandDamageCollider?.DisableDamageCollider();
        bodyDamageCollider?.DisableDamageCollider();
    }

}
