using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public DamageCollider damageCollider;
    public GameObject weaponTrail;

    private void Awake()
    {
        damageCollider = GetComponentInChildren<DamageCollider>();
    }

    // 무기의 데미지를 설정한다.
    public void SetWeaponDamage(CharacterManager characterWieldingWeapon, Weapon weapon, float damage, float groggyDamage)
    {
        // TODO : 여기 이 부분을 캐싱하던지 해야 됨. 
        characterWieldingWeapon.TryGetComponent(out PlayerCombatManager playerCombatManager);

        damageCollider.characterAttacking = characterWieldingWeapon;
        if (playerCombatManager != null)
        {
            damageCollider.physicalDamage = damage * (100 + playerCombatManager.AttackIncreaseRate) / 100;
        }
        else
            damageCollider.physicalDamage = damage;
        damageCollider.magicDamage = damage;
        damageCollider.fireDamage = damage;
        damageCollider.lighteningDamage = damage;
        damageCollider.holyDamage = damage;
        damageCollider.groggyDamage = groggyDamage;
    }
}
