using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Netcode;
using UnityEngine;

public class UltimateAttackAction : NetworkBehaviour
{
    private PlayerManager player;

    [SerializeField] private ObjectPoolType objectPoolType;
    [SerializeField] private int index;
    [Header("Knight Ultimate Attack")]
    [SerializeField] private float buffRadius = 10f;
    [SerializeField] private GameObject shieldVFX;
    [Header("Warrior Ultimate Attack")]
    [SerializeField] private float interceptAggroTime = 20f;
    [SerializeField] private int superArmorCount = 3;
    [SerializeField] private float attackIncreaseRate = 0f;
    [SerializeField] private float damageDecreaseRate = 0f;
    [SerializeField] private float chargingEffectHeight = 0f;

    [Header("Thief Ultimate Attack")]
    [SerializeField] private float debuffTime = 10f;
    [SerializeField] private float damageIncreaseRate = 20f;
    [SerializeField] private float thunderEffectHeight = 3f;
    [SerializeField] private float debuffEffectHeight = 3f;

    void Awake()
    {
        player = GetComponent<PlayerManager>();

    }
    public void PerformUltimateAttack()
    {
        if (IsOwner == false) return;
        switch (player.characterClass)
        {
            case PlayerClass.Knight:
                // 따라서 여기서 VFX를 실행하진 않고 애니메이션 이벤트용 함수만 구현해주자.
                // 일정 거리 이내의 근처 플레이어들에게 방어막을 씌워준다.
                // 방어막이 씌여지면 플레이어들은 2회 일반공격 피격이 면제된다.
                // 이것도 시간제한이 있다. 시간은 스킬 쿨타임의 절반.
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, buffRadius);
                List<PlayerManager> foundTargets = new List<PlayerManager>();

                foreach (var collider in hitColliders)
                {
                    PlayerManager target = collider.GetComponent<PlayerManager>();
                    if (target != null)
                    {
                        foundTargets.Add(target);
                        Debug.Log("Found target: " + target.name);
                    }
                }
                foreach (var target in foundTargets)
                {
                    // 쉴드 생성
                    // 2회 피격 면제를 부여한다.
                    target.ultimateAttackAction.GenerateShieldServerRpc();
                }

                break;
            case PlayerClass.Warrior:
                break;
            case PlayerClass.Thief:
                break;
            case PlayerClass.Archer:
                break;

        }
    }

    // 스킬 발동 시 Enchant VFX를 실행한다.
    public void PlayEnchantVFX()
    {
        GameObject pooledObject = ObjectPoolManager.Singleton.GetObject(objectPoolType, index);

        // GameObject pooledObject = ObjectPoolManager.Singleton.GetObject(shockwaveEffectPrefab);
        pooledObject.transform.position = transform.position + transform.up;
        pooledObject.transform.SetParent(transform);

        ParticleSystem effect = pooledObject.GetComponent<ParticleSystem>();
        effect.Play();

        ObjectPoolManager.Singleton.WaitAndReturnVFX(pooledObject, ObjectPoolManager.Singleton.GetPrefab(objectPoolType, index)).Forget();
    }


    #region Knight Ultimate Attack
    // 캐릭터들에게 쉴드를 생성하게끔 한다.
    [ServerRpc(RequireOwnership = false)]
    private void GenerateShieldServerRpc()
    {
        GenerateShieldClientRpc();
    }

    [ClientRpc]
    private void GenerateShieldClientRpc()
    {
        // 쉴드 생성
        PlayShieldVFX();
        // 오너일 경우 피격 2회 면제 효과 부여
        if (IsOwner)
        {
            // 2회 피격 면제를 부여한다.
            player.playerCombatManager.SetInvulnerableCount(2);
        }
    }

    // 캐릭터들이 갖고 있는 쉴드를 제거한다. 
    [ServerRpc(RequireOwnership = false)]
    public void RemoveShieldServerRpc()
    {
        RemoveShieldClientRpc();
    }

    [ClientRpc]
    private void RemoveShieldClientRpc()
    {
        // 쉴드 VFX 제거
        if (shieldVFX != null)
        {
            shieldVFX.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ObjectPoolManager.Singleton.ReturnObject(shieldVFX, ObjectPoolManager.Singleton.GetPrefab(objectPoolType, 4));
            shieldVFX = null;
        }
    }

    // 기사 전용 함수
    // 각 플레이어의 위치에 VFX를 생성한다. 
    private void PlayShieldVFX()
    {
        if (shieldVFX != null)
        {
            shieldVFX.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ObjectPoolManager.Singleton.ReturnObject(shieldVFX, ObjectPoolManager.Singleton.GetPrefab(objectPoolType, 4));
            shieldVFX = null;
        }
        GameObject pooledObject = ObjectPoolManager.Singleton.GetObject(objectPoolType, 4);

        // GameObject pooledObject = ObjectPoolManager.Singleton.GetObject(shockwaveEffectPrefab);
        pooledObject.transform.position = transform.position + transform.up;
        pooledObject.transform.SetParent(transform);
        ParticleSystem effect = pooledObject.GetComponent<ParticleSystem>();
        effect.Play();
        shieldVFX = pooledObject;


        // ObjectPoolManager.Singleton.WaitAndReturnVFX(pooledObject, ObjectPoolManager.Singleton.GetPrefab(objectPoolType, index)).Forget();
    }


    // 얘도 애니메이션 이벤트로 실행한다.
    // 버프를 받는 범위의 VFX를 실행한다.
    public void PlayKnightBuffVFX()
    {
        GameObject pooledObject = ObjectPoolManager.Singleton.GetObject(objectPoolType, 5);

        // GameObject pooledObject = ObjectPoolManager.Singleton.GetObject(shockwaveEffectPrefab);
        pooledObject.transform.position = transform.position + transform.up * 0.1f;
        pooledObject.transform.SetParent(transform);
        ParticleSystem effect = pooledObject.GetComponent<ParticleSystem>();
        effect.Play();

        ObjectPoolManager.Singleton.WaitAndReturnVFX(pooledObject, ObjectPoolManager.Singleton.GetPrefab(objectPoolType, 5)).Forget();
    }
    #endregion

    #region Warrior Ultimate Attack
    // 보스의 공격애니메이션을 중단시키고 20초간 보스의 모든 어그로를 끈다.
    // 워리어의 애니메이션 이벤트에서 실행된다.

    public void PlayWarriorBuffVFX()
    {
        GameObject pooledObject = ObjectPoolManager.Singleton.GetObject(objectPoolType, 9);

        // GameObject pooledObject = ObjectPoolManager.Singleton.GetObject(shockwaveEffectPrefab);
        pooledObject.transform.position = transform.position + transform.up * 0.1f;
        pooledObject.transform.SetParent(transform);
        ParticleSystem effect = pooledObject.GetComponent<ParticleSystem>();
        effect.Play();

        ObjectPoolManager.Singleton.WaitAndReturnVFX(pooledObject, ObjectPoolManager.Singleton.GetPrefab(objectPoolType, 9)).Forget();
    }

    public async UniTaskVoid PlayChargingVFX()
    {

        await UniTask.WaitForSeconds(0.5f);
        GameObject pooledObject = ObjectPoolManager.Singleton.GetObject(objectPoolType, 8);

        // GameObject pooledObject = ObjectPoolManager.Singleton.GetObject(shockwaveEffectPrefab);
        pooledObject.transform.position = player.playerCombatManager.lockOnTransform.position + Vector3.up * chargingEffectHeight;
        pooledObject.transform.SetParent(player.playerCombatManager.lockOnTransform);

        ParticleSystem effect = pooledObject.GetComponent<ParticleSystem>();
        effect.Play();
        await UniTask.WaitForSeconds(interceptAggroTime);
        effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        ObjectPoolManager.Singleton.WaitAndReturnVFX(pooledObject, ObjectPoolManager.Singleton.GetPrefab(objectPoolType, 8)).Forget();
    }
    public void InterceptAggro()
    {
        CharacterManager target = player.playerCombatManager.currentTarget;
        if (target == null)
        {
            return;
        }
        if (IsHost)
        {
            target.GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("FixedTarget", player);
            target.TryGetComponent(out AIBossCharacterManager boss);
            boss.bossCombatManager.SetAggroFixedToplayer(player, interceptAggroTime).Forget();
            boss.bossAnimatorManager.StopAttackAnimationServerRpc();

            // GameManager.Instance.StopBossAnimation();
            // GameManager.Instance.SetBossAggro(player, interceptAggroTime);
        }
        if (IsOwner)
        {
            player.playerCombatManager.Enchant(superArmorCount, attackIncreaseRate, damageDecreaseRate, interceptAggroTime);
        }
    }

    #endregion

    #region Thief Ultimate Attack Skill
    // 애니메이션 이벤트로 발동된다.
    // 보스에게 죽음의 표시를 남긴다.
    // 보스는 10초간 받는 데미지가 20% 늘어난다.

    public void CameraMoving()
    {
        CharacterManager target = player.playerCombatManager.currentTarget;
        if (target == null)
        {
            return;
        }

    }
    public void MakeDeathMarkToTarget()
    {
        if (IsOwner == false) return;
        CharacterManager target = player.playerCombatManager.currentTarget;
        if (target == null)
        {
            return;
        }
        // DoSomething;
        // 타겟의 UI에 DeathMark 표시.
        // 보스의 머리 위에 낙뢰를 떨어트린다.
        PlayThunderVFXServerRpc(target.transform.position);
        PlayAfterThunderVFXServerRpc(target.NetworkObjectId);
        PlayElectronicDebuffVFX(target.NetworkObjectId).Forget();
        target.characterCombatManager.EnchantServerRpc(0, 0, damageIncreaseRate, debuffTime);
        // target.characterCombatManager.
    }

    [ServerRpc]
    private void PlayElectronicDebuffVFXServerRpc(ulong networkObjectId)
    {
        PlayElectronicDebuffVFXClientRpc(networkObjectId);
    }

    [ClientRpc]
    private void PlayElectronicDebuffVFXClientRpc(ulong networkObjectId)
    {
        PlayElectronicDebuffVFX(networkObjectId).Forget();
    }

    // 지속형 VFX이라 Loop 체크 되어있음. 
    // 일정시간 지난 후 자연스럽게 꺼줘야 함. 
    private async UniTaskVoid PlayElectronicDebuffVFX(ulong networkObjectId)
    {
        await UniTask.WaitForSeconds(0.5f);
        NetworkObject target = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
        Debug.Log(target.name);


        GameObject pooledObject = ObjectPoolManager.Singleton.GetObject(objectPoolType, 7);

        // GameObject pooledObject = ObjectPoolManager.Singleton.GetObject(shockwaveEffectPrefab);
        pooledObject.transform.position = target.transform.position + Vector3.up * debuffEffectHeight;
        pooledObject.transform.SetParent(target.transform);

        ParticleSystem effect = pooledObject.GetComponent<ParticleSystem>();
        effect.Play();
        await UniTask.WaitForSeconds(debuffTime);
        effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        ObjectPoolManager.Singleton.WaitAndReturnVFX(pooledObject, ObjectPoolManager.Singleton.GetPrefab(objectPoolType, 7)).Forget();

    }

    [ServerRpc]
    private void PlayThunderVFXServerRpc(Vector3 targetPosition)
    {
        PlayThunderVFXClientRpc(targetPosition);
    }

    [ClientRpc]
    private void PlayThunderVFXClientRpc(Vector3 targetPosition)
    {
        GameObject pooledObject = ObjectPoolManager.Singleton.GetObject(objectPoolType, 6);

        pooledObject.transform.position = targetPosition + Vector3.up * thunderEffectHeight;

        ParticleSystem effect = pooledObject.GetComponent<ParticleSystem>();
        effect.Play();

        ObjectPoolManager.Singleton.WaitAndReturnVFX(pooledObject, ObjectPoolManager.Singleton.GetPrefab(objectPoolType, 6)).Forget();
    }

    [ServerRpc]
    private void PlayAfterThunderVFXServerRpc(ulong networkObjectId)
    {
        PlayAfterThunderVFXClientRpc(networkObjectId);
    }

    [ClientRpc]
    private void PlayAfterThunderVFXClientRpc(ulong networkObjectId)
    {
        PlayAfterThunderVFX(networkObjectId).Forget();
    }

    private async UniTaskVoid PlayAfterThunderVFX(ulong networkObjectId)
    {
        await UniTask.WaitForSeconds(0.4f);

        Debug.Log(networkObjectId);
        NetworkObject target = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
        Debug.Log(target.name);
        GameObject pooledObject = ObjectPoolManager.Singleton.GetObject(objectPoolType, 10);

        pooledObject.transform.position = target.transform.position + Vector3.up * 0.1f;

        ParticleSystem effect = pooledObject.GetComponent<ParticleSystem>();
        effect.Play();

        ObjectPoolManager.Singleton.WaitAndReturnVFX(pooledObject, ObjectPoolManager.Singleton.GetPrefab(objectPoolType, 10)).Forget();
        return;
    }
    #endregion

}