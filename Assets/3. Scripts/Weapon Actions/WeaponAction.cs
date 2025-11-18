using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Test Action")]
public class WeaponAction : ScriptableObject
{
    public int actionID;
    public DamagePair[] damagePairs;


    // 사용중인 무기의 값을 네트워크 상에서 변경함. 그로 인해 애니메이터 컨트롤러가 무기에 딸린 오버라이드 컨트롤러로 변경됨.
    public virtual void AttempToPerformAction(PlayerManager playerPerformingAction, Weapon weaponPerformingAction)
    {
        if (playerPerformingAction.IsOwner)
        {
            playerPerformingAction.playerCombatManager.currentWeaponAction = this;
            playerPerformingAction.playerNetworkManager.currentWeaponBeingUsed.Value = weaponPerformingAction.itemID;
        }
    }

    [Serializable]
    public class DamagePair
    {
        public int damage;
        public int groggyDamage;
        public DamagePair(int damage, int groggyDamage)
        {
            this.damage = damage;
            this.groggyDamage = groggyDamage;
        }
    }
}
