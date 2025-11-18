using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterNetworkManager : NetworkBehaviour
{
    CharacterManager character;

    public float stiffTimeTest;



    [Header("Active")]
    public NetworkVariable<bool> isActive = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Position")]
    public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public Vector3 networkPositionVelocity;
    public float networkPositionSmoothTime = 0.1f;
    public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public float networkRotationSmoothTime = 0.1f;

    [Header("Animator")]
    public NetworkVariable<bool> isMoving = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> animatorHorizontalParameter = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> animatorVerticalParameter = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> animatorMoveAmount = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isIgnoringRootmotion = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    [Header("Target")]
    public NetworkVariable<ulong> currentTargetNetworkObjectID = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    // 나중에 얘도 좀 손봐야 할 듯. 
    public NetworkVariable<bool> isPartiallyInvulnerable = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    [Header("Flags")]
    public NetworkVariable<bool> isGaurding = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isParrying = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isAttacking = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isInvulnerable = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isLockedOn = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isSprinting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isJumping = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isChargingAttack = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isRipostable = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isStanceBroken = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isGroggy = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);



    public bool wasBlocking = false;

    // 체력 레벨 같은 존재들인데, 레벨업 등을 고려하지 않으므로 주석 처리.
    // [Header("Stats")]
    // public NetworkVariable<int> vitality = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    // public NetworkVariable<int> enduracne = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Resources")]
    public NetworkVariable<int> maxHp = new NetworkVariable<int>(400, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> currentHp = new NetworkVariable<int>(400, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> maxStamina = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> currentStamina = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);



    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }


    public virtual void OnIsGroggyChanged(bool previousValue, bool newValue)
    {

    }

    public virtual void CheckHP(int oldValue, int newValue)
    {
        if (currentHp.Value <= 0)
        {
            character.ProcessDeathEvent();
        }


        if (character.IsOwner)
        {
            if (currentHp.Value > maxHp.Value)
            {
                currentHp.Value = maxHp.Value;
            }
        }
    }

    public virtual void SetNewMaxHealthValue(int oldHp, int newHp)
    {
        // maxHp.Value = newHp;
        if (IsOwner)
            currentHp.Value = newHp;

        // Debug.Log($"체력 입력값 : {newVitality}");
        // Debug.Log($"체력 설정 : {currentHealth.Value}");
    }

    // 스태미나 관련 코드 주석 처리
    // public virtual void SetNewMaxStaminaValue(int oldEndurance, int newEndurance)
    // {
    //     maxStamina.Value = character.characterStatsManager.CalculateStaminaBasedOnEnduranceLevel(newEndurance);

    //     currentStamina.Value = maxStamina.Value;
    //     Debug.Log($"스태미나 설정 : {currentStamina.Value}");
    //     // Debug.Log($"스태미나 입력값 : {newEndurance}");
    // }

    public virtual void OnIsAttackingChanged(bool oldStatus, bool newStatus)
    {
        if (isAttacking.Value && isGaurding.Value)
        {
            wasBlocking = true;
        }
    }

    public virtual void OnIsParryingChanged(bool oldStatus, bool newStatus)
    {
    }

    public virtual void OnIsGaurdingChanged(bool oldStatus, bool newStatus)
    {
    }

    public virtual void OnIsActiveChanged(bool oldStatus, bool newStatus)
    {
        gameObject.SetActive(isActive.Value);
    }

    public void OnLockOnTargetIDChange(ulong oldID, ulong newID)
    {
        if (!IsOwner)
        {
            // 현재 락온 타겟을 대상으로 진행해야 될 일이 없음. 
            // character.characterCombatManager.currentTarget = NetworkManager.Singleton.SpawnManager.SpawnedObjects[newID].gameObject.GetComponent<CharacterManager>();
        }
    }

    public void OnIsLockedOnChanged(bool old, bool isLockedOn)
    {
        if (!isLockedOn)
        {
            character.characterCombatManager.currentTarget = null;
        }
    }

    public void OnIsChargingattackChanged(bool oldStatus, bool newStatus)
    {
        // character.animator.SetBool("isChargingAttack", isChargingAttack.Value);

        if (newStatus == false)
        {
            character.animator.SetTrigger("TransitionStart");
        }
    }

    public void OnIsMovingChanged(bool previousValue, bool newValue)
    {
        character.animator.SetBool("isMoving", isMoving.Value);
    }

    [ServerRpc]
    public void ActionAnimationServerRpc(ulong clientId, string animaitonId, bool applyRootMotion, bool instant = false)
    {
        if (IsServer)
        {
            PlayActionAnimationClientRpc(clientId, animaitonId, applyRootMotion, instant);
        }
    }

    [ClientRpc]
    public void PlayActionAnimationClientRpc(ulong clientId, string animationId, bool applyRootMotion, bool instant = false)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId)
        {
            PerFormActionAnimationFromServer(animationId, applyRootMotion, instant);
        }
    }

    private void PerFormActionAnimationFromServer(string animaitonId, bool applyRootMotion, bool instant = false)
    {
        character.characterAnimatorManager.applyRootMotion = applyRootMotion;
        if (instant)
            character.animator.CrossFade(animaitonId, 0f);
        else
            character.animator.CrossFade(animaitonId, 0.2f);
    }

    // RPC 두 가지 종류가 있음
    // ServerRpc 서버에서 일어나는 함수 
    // ClientRpc 클라이언트에서 일어나는 함수 

    // NOTE: 애니메이션 전이 시간, 애니메이터 속도 추가할 겸 통합함수로 만듦.
    // 새롭게 통합되는 애니메이션 실행 함수
    [ServerRpc]
    public void PlayAnimationOnNetworkServerRpc(ulong clientId, AnimationType animationType, string animaitonId, bool applyRootMotion, float transitionNomalizedTime, float animatorSpeed)
    {
        if (IsServer)
        {
            PlayAnimationOnNetworkClientRpc(clientId, animationType, animaitonId, applyRootMotion, transitionNomalizedTime, animatorSpeed);
        }
    }
    [ClientRpc]
    public void PlayAnimationOnNetworkClientRpc(ulong clientId, AnimationType animationType, string animationId, bool applyRootMotion, float transitionNomalizedTime, float animatorSpeed)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId)
        {
            PlayAnimationOnLocalClient(animationId, animationType, applyRootMotion, transitionNomalizedTime, animatorSpeed);
        }
    }
    private void PlayAnimationOnLocalClient(string animaitonId, AnimationType animationType, bool applyRootMotion, float transitionNomalizedTime, float animatorSpeed)
    {
        character.characterAnimatorManager.applyRootMotion = applyRootMotion;
        character.animator.speed = animatorSpeed;

        if (animationType == AnimationType.Hit || animationType == AnimationType.Parrying || animationType == AnimationType.Evade)
        {
            character.animator.Play(animaitonId, 2, transitionNomalizedTime);
        }
        else
            character.animator.CrossFade(animaitonId, transitionNomalizedTime);
    }


    /// <summary>
    /// 네트워크 상에서 캐릭터의 피격 처리 및 피격 애니메이션 실행을 담당한다. 
    /// </summary>
    /// <param name="damagedCharacter"></param>
    /// <param name="characterCausingDamage"></param>
    /// <param name="effectIndex"></param>
    /// <param name="physicalDamage"></param>
    /// <param name="magicDamage"></param>
    /// <param name="fireDamage"></param>
    /// <param name="holyDamage"></param>
    /// <param name="poiseDamage"></param>
    /// <param name="angleHitFrom"></param>
    /// <param name="contactPointX"></param>
    /// <param name="contactPointY"></param>
    /// <param name="contactPointZ"></param>
    /// <param name="attackType"></param>
    /// <param name="objectPoolType"></param>
    /// <param name="poolIndex"></param>
    /// <param name="blocked"></param>
    /// <param name="AITarget"></param>
    [ServerRpc(RequireOwnership = false)]
    public void NotifyTheServerOfCharacterDamageServerRpc(
        ulong damagedCharacter,
        ulong characterCausingDamage,
        int effectIndex,
        float physicalDamage,
        float magicDamage,
        float fireDamage,
        float holyDamage,
        float poiseDamage,
        float angleHitFrom,
        float contactPointX,
        float contactPointY,
        float contactPointZ,
        AttackType attackType,
        ObjectPoolType objectPoolType,
        int poolIndex,
        bool blocked = false,
        bool AITarget = false)
    {
        NotifyTheServerOfCharacterDamageClientRpc(damagedCharacter, characterCausingDamage, effectIndex, physicalDamage, magicDamage, fireDamage, holyDamage, poiseDamage, angleHitFrom, contactPointX, contactPointY, contactPointZ, attackType, objectPoolType, poolIndex, blocked, AITarget);
    }

    [ClientRpc]
    public void NotifyTheServerOfCharacterDamageClientRpc(
        ulong damagedCharacterID,
        ulong characterCausingDamageID,
        int effectIndex,
        float physicalDamage,
        float magicDamage,
        float fireDamage,
        float holyDamage,
        float poiseDamage,
        float angleHitFrom,
        float contactPointX,
        float contactPointY,
        float contactPointZ,
        AttackType attackType,
        ObjectPoolType objectPoolType,
        int poolIndex,
        bool blocked = false,
        bool AITarget = false)
    {
        ProcessCharacterDamageFromServer(damagedCharacterID, characterCausingDamageID, effectIndex, physicalDamage, magicDamage, fireDamage, holyDamage, poiseDamage, angleHitFrom, contactPointX, contactPointY, contactPointZ, attackType, objectPoolType, poolIndex, blocked, AITarget);
    }

    // 맞는 캐릭터와 때리는 캐릭터 모두 경직? 

    private void ProcessCharacterDamageFromServer(
        ulong damagedCharacterID,
        ulong characterCausingDamageID,
        int effectIndex,
        float physicalDamage,
        float magicDamage,
        float fireDamage,
        float holyDamage,
        float poiseDamage,
        float angleHitFrom,
        float contactPointX,
        float contactPointY,
        float contactPointZ,
        AttackType attackType,
        ObjectPoolType objectPoolType,
        int poolIndex,
        bool blocked = false,
        bool AITarget = false)
    {

        CharacterManager damagedCharacter = NetworkManager.Singleton.SpawnManager.SpawnedObjects[damagedCharacterID].gameObject.GetComponent<CharacterManager>();
        CharacterManager characterCausingDamage = null;
        if (characterCausingDamageID != ulong.MaxValue)
            characterCausingDamage = NetworkManager.Singleton.SpawnManager.SpawnedObjects[characterCausingDamageID].gameObject.GetComponent<CharacterManager>();

        DamageEffect damageEffect = Instantiate(WorldCharacterEffectManager.instance.instanceEffectList[effectIndex]);

        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.holyDamage = holyDamage;
        damageEffect.groggyDamage = poiseDamage;
        damageEffect.angleHitFrom = angleHitFrom;
        damageEffect.contactPoint = new Vector3(contactPointX, contactPointY, contactPointZ);
        // Debug.Log(damageEffect.contactPoint);
        damageEffect.attackType = attackType;

        damageEffect.characterCausingDamage = characterCausingDamage;

        damageEffect.vfxObjectPoolType = objectPoolType;
        damageEffect.poolIndex = poolIndex;

        switch (damagedCharacter.characterGroup)
        {
            case CharacterType.Player:
                if (NetworkManager.Singleton.LocalClientId == damagedCharacter.OwnerClientId) return;


                damagedCharacter.characterEffectManager.ProcessInstantEffct(damageEffect);

                break;
            // 몬스터의 경우 SFX, VFX를 전파하는 것에만 의의가 있다. 
            // 콜라이더를 꺼주는 건.. 나중에 생각하자 
            case CharacterType.Monster:
                if (damageEffect.attackType == AttackType.SealAttack)
                {
                    if (NetworkManager.Singleton.LocalClientId == characterCausingDamage.OwnerClientId) return;
                    damagedCharacter.characterEffectManager.ProcessInstantEffct(damageEffect);
                    break;
                }

                // 때린 놈은 SFX, VFX를 실행하지 마라. 그 외에 서버에서만 돌아가야 하는 것들이 있다면 받아라.
                if (!IsHost && NetworkManager.Singleton.LocalClientId == characterCausingDamage.OwnerClientId)
                    damagedCharacter.characterEffectManager.ProcessInstantEffctOnServer(damageEffect);
                // 그리고 호스트라면 어차피 즉발로 실행하게 되고(RTT가 제로임)
                else
                {
                    characterCausingDamage.TryGetComponent<PlayerEmitter>(out PlayerEmitter emitter);
                    if (emitter != null)
                    {
                        if (characterCausingDamage.GetComponent<PlayerManager>().CharacterClass == PlayerClass.Thief)
                        {
                            if (damageEffect.attackType == AttackType.LightAttack)
                            {
                                Debug.Log("SFXHit 01");
                                emitter.SFXHit(damagedCharacter.transform, 0);
                            }
                            if (damageEffect.attackType == AttackType.LastAttack)
                            {
                                Debug.Log("SFXHit 02");
                                emitter.SFXHit(damagedCharacter.transform, 1);
                            }
                            if (damageEffect.attackType == AttackType.HeavyAttack)
                            {
                                Debug.Log("HeavyAttack");
                                emitter.SFXHeavyHit(damagedCharacter.transform);
                            }
                        }

                        else
                        {
                            if (damageEffect.attackType == AttackType.LightAttack) emitter.SFXHit(damagedCharacter.transform);
                        }

                        if (damageEffect.attackType == AttackType.HeavyAttack)
                        {
                            emitter.SFXHeavyHit(damagedCharacter.transform);
                        }
                    }
                    damagedCharacter.characterEffectManager.ProcessInstantEffct(damageEffect);
                }

                break;
            case CharacterType.Object:
                damagedCharacter.characterEffectManager.ProcessInstantEffct(damageEffect);
                break;
        }
    }

}
