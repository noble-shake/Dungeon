using System.Collections;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Netcode;
using UnityEngine;


public class AIWarriorGolemCombatManager : AIBossCharacterCombatManager
{
    AIWarriorGolemCharacterManager boss;

    [Header("Damage Colliders")]
    [SerializeField] DamageCollider damageCollider;
    [SerializeField] DamageCollider shockwaveDamageCollider;

    [Header("Trail VFX")]
    [SerializeField] WeaponTrailController weaponTrailController;

    [Header("Damage")]
    [SerializeField] int baseDamage = 25;
    [SerializeField] int basePoiseDamage = 25;

    [Header("Pattern2 Inspector")]
    [SerializeField] private int AttackCycle = 0;

    [Header("???? ???????")]
    [SerializeField] List<AICharacterAttackAction> Pattern3AttackList = new List<AICharacterAttackAction>();

    protected override void Awake()
    {
        base.Awake();

        boss = GetComponent<AIWarriorGolemCharacterManager>();

        weaponTrailController = GetComponent<WeaponTrailController>();
    }

    // 
    public override AICharacterAttackAction GetPossibleAttack()
    {
        if (aiCharacter.aiCharacterCombatManager.currentTarget == null) return null;
        possibleList.Clear();
        foreach (var potentialAttack in attackList)
        {
            if (potentialAttack.minimumAttackDistance >= aiCharacter.aiCharacterCombatManager.distanceFromTarget)
                continue;
            if (potentialAttack.maximumAttackDistance <= aiCharacter.aiCharacterCombatManager.distanceFromTarget)
                continue;
            if (potentialAttack.minimumAttackAngle >= aiCharacter.aiCharacterCombatManager.viewableAngle)
                continue;
            if (potentialAttack.maximumAttackAngle <= aiCharacter.aiCharacterCombatManager.viewableAngle)
                continue;

            possibleList.Add(potentialAttack);
        }
        if (possibleList.Count <= 0) return null;

        var totalWeight = 0;

        foreach (var attack in possibleList)
        {
            totalWeight += attack.attackWeight;
        }

        var randomWeightValue = Random.Range(1, totalWeight + 1);
        var processedWeight = 0f;

        foreach (var attack in possibleList)
        {
            processedWeight += attack.attackWeight;

            if (randomWeightValue <= processedWeight)
            {
                return attack;
            }
        }
        return null;
    }

    private AICharacterAttackAction PentagramAttack()
    {
        Debug.Log("In Pattern ");

        AttackCycle++;
        if (AttackCycle == 3)
        {
            AttackCycle = 0;

            var atk = attackList.Find(x => (x.AttackAnimation).Equals("Combo 4-1"));
            return atk;
        }

        AICharacterAttackAction exceptAttack = attackList.Find(x => (x.AttackAnimation).Equals("Combo 4-1"));
        int temp_weight = exceptAttack.attackWeight;
        exceptAttack.attackWeight = 0;

        possibleList.Clear();
        foreach (var potentialAttack in attackList)
        {
            possibleList.Add(potentialAttack);
        }
        if (possibleList.Count <= 0) return null;

        var totalWeight = 0;

        foreach (var attack in possibleList)
        {
            totalWeight += attack.attackWeight;
        }

        var randomWeightValue = Random.Range(1, totalWeight + 1);
        var processedWeight = 0f;

        foreach (var attack in possibleList)
        {
            processedWeight += attack.attackWeight;

            if (randomWeightValue <= processedWeight)
            {
                exceptAttack.attackWeight = temp_weight;
                return attack;
            }
        }
        exceptAttack.attackWeight = temp_weight;
        return null;
    }


    public void OpenDamageCollider()
    {
        aiCharacter.aiCharacterEquipmentManager.RightWeaponManager?.damageCollider.EnableDamageCollider();
        // damageCollider.EnableDamageCollider();
        weaponTrailController.EnableTrail();
    }

    public void CloseDamageCollider()
    {
        aiCharacter.aiCharacterEquipmentManager.RightWeaponManager?.damageCollider.DisableDamageCollider();

        // damageCollider.DisableDamageCollider();
        weaponTrailController.DisableTrail();
    }

    public override void CloseAllDamageCollider()
    {
        CloseDamageCollider();
    }

    // 애니메이션 이벤트로 걸려 있음. 
    public void SetDamageOnCollider(int index)
    {
        // currentAttack.attackDamageList[index]
        if (currentAttack != null)
        {
            SetDamageOnColliderServerRpc(currentAttack.attackDamageList[index]);
            // aiCharacter.aiCharacterEquipmentManager.RightWeaponManager?.damageCollider.SetDamage(currentAttack.attackDamageList[index]);
        }
    }

    // 충격파 범위를 세팅해주는 로직.
    // AI한테 돌아가기 때문에 RPC 처리해줄 필요가 있다. 
    public void SetShockWaveRadius(float radius)
    {
        if (currentAttack != null)
        {
            // shockwaveDamageCollider.SetRadius(radius);
        }
    }

    [ServerRpc]
    private void SetDamageOnColliderServerRpc(int damage)
    {
        SetDamageOnColliderClientRpc(damage);
    }

    [ClientRpc]
    private void SetDamageOnColliderClientRpc(int damage)
    {
        boss.aiCharacterEquipmentManager.RightWeaponManager?.damageCollider.SetDamage(damage);
        // 보스의 공격은 현재 그로기 데미지가 없음. 
        boss.shockwave?.SetShockWaveDamage(damage, 0);
    }
}
