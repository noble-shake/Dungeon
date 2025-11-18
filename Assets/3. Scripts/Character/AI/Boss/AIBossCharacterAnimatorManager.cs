using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AIBossCharacterAnimatorManager : AICharacterAnimatorManager
{
    private AIBossCharacterManager bossCharacterManager;

    protected override void Awake()
    {
        base.Awake();
        bossCharacterManager = GetComponent<AIBossCharacterManager>();
    }

    [Header("Reset Time")]
    [SerializeField] float resetTimer = 0f;
    [SerializeField] float resetTime = 4f;
    public override void PlayAnimationOnNetwork(
        AnimationType animationType, string targetAnimation, bool isPerformingAction, Weapon weapon = null, AttackType attackType = AttackType.None,
        bool applyRootmotion = true, bool canRotate = false, bool canMove = false, bool canGaurd = false, float transitionNomalizedTime = 0.2f, float animatorSpeed = 1f, float damage = 0f,
        float groggyDamage = 0f)
    {
        base.PlayAnimationOnNetwork(animationType, targetAnimation, isPerformingAction, weapon,
        attackType, applyRootmotion, canRotate, canMove, canGaurd, transitionNomalizedTime,
        animatorSpeed);
        resetTimer = resetTime;
    }

    private void Update()
    {
        if (resetTimer >= 0)
        {
            resetTimer -= Time.deltaTime;
        }
        else
        {
            bossCharacterManager.characterCombatManager.lastAttackAnimationPerformed = string.Empty;
        }
    }

    // OnIsGroggy 라는 변수가 생긴 마당에, 이것도 NetworkVariable의 delegate에 걸어줘야 하나? 
    public void PlayGroggyAnimation(int seconds)
    {
        StartCoroutine(PlayGroggyAnimationCoroutine(seconds));
    }

    private IEnumerator PlayGroggyAnimationCoroutine(int value)
    {
        bossCharacterManager.bossNetworkManager.isGroggy.Value = true;
        PlayAnimationOnNetwork(AnimationType.Hit, "Groggy_Start", true);
        yield return new WaitForSeconds(value);
        GroggyEndServerRpc();
        bossCharacterManager.bossNetworkManager.isGroggy.Value = false;

    }

    [ServerRpc]
    private void PlayGroggyVFXServerRpc()
    {
        PlayGroggyVFXClientRpc();

    }

    [ClientRpc]
    private void PlayGroggyVFXClientRpc()
    {
        GameObject pooledObject = ObjectPoolManager.Singleton.GetObject(ObjectPoolType.Hit, 10);

        pooledObject.transform.SetParent(transform);
        pooledObject.transform.localPosition = transform.up * 3f;

        ObjectPoolManager.Singleton.WaitAndReturnVFX(pooledObject, ObjectPoolManager.Singleton.GetPrefab(ObjectPoolType.Hit, 9)).Forget();
    }

    [ServerRpc]
    private void GroggyEndServerRpc()
    {
        GroggyEndClientRpc();
    }
    [ClientRpc]
    private void GroggyEndClientRpc()
    {
        bossCharacterManager.animator.SetTrigger("GroggyTrigger");
    }

    [ServerRpc]
    public void StopAttackAnimationServerRpc()
    {
        StopAttackAnimationClientRpc();
    }

    [ClientRpc]
    private void StopAttackAnimationClientRpc()
    {
        bossCharacterManager.animator.SetTrigger("StopAttackTrigger");
    }
}
