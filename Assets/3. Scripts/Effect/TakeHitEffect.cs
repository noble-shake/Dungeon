using System;
using Unity.Behavior;
using UnityEngine;


/// <summary>
/// 피격 시 발생해야 되는 일들과 거기에 필요한 정보를 포함한다.
/// </summary>
[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Hit Damage")]
public class TakeHitEffect : DamageEffect
{
    private bool isPerfomingAction;
    private bool disableCollider;
    private PlayerManager player;
    public override void ProcessEffect(CharacterManager damagedCharacter)
    {
        if (damagedCharacter.characterNetworkManager.isInvulnerable.Value) return;

        if (damagedCharacter.isDead.Value) return;

        damagedCharacter.TryGetComponent(out PlayerManager playerManager);
        if (playerManager != null)
        {
            player = playerManager;
        }

        if (player?.playerCombatManager.InvulnerableCount > 0)
        {
            player.playerCombatManager.InvulnerableCount--;
            return;
        }

        player?.playerLocomotionManager.EnableGravitySettings();

        // 피격 판정 시 내 무기의 데미지 콜라이더를 비활성화 해야 함. 
        // 근데 반드시 비활성화 해야 되는 건 아님. 

        // 피격 시 플레이어들이 구르기가 가능할 때가 있음. 그걸 강제적으로 막아주려고 함. 
        // 재현 루트 -> 패링 -> 공격 -> 피격 -> 구르기 가능 (아마 공격 중에 피격될 시 canDoEvade가 true로 바뀌는 듯)
        // 봉인된 상태에서 타격 받을 시 어케어케 해줘야 함. 
        if (damagedCharacter.IsOwner)
        {
            player?.CoroutineStop();

            // 위의 if문에서 이미 실행했음. 
            // damagedCharacter.TryGetComponent(out PlayerCombatManager playerCombatManager);
            if (player != null)
            {
                player.playerCombatManager.canDoEvade = false;
                player.playerCombatManager.canPerformCounterAttack = false;
            }

            damagedCharacter.characterNetworkManager.isParrying.Value = false;
            if (player != null)
            {
                player.playerNetworkManager.isInteracting.Value = false;
            }

        }

        disableCollider = false;

        switch (damagedCharacter.characterGroup)
        {
            case CharacterType.Player:
                player.playerCombatManager.canComboWithMainHandWeapon = false;
                break;
        }

        // 몬스터 혹은 플레이어가 특수한 상태일 때, 타격 횟수를 체크해야 할 때 발동한다.
        // 플레이어 
        // 플레이어가 봉인 되었을 때
        // 보스
        // 특정 타수 이상을 맞아야만 풀리는 조건 
        CalculateHitCountWhenPartiallyInvulnerableState(damagedCharacter);

        if (playDamageAnimation == true)
        {
            // 오너만
            PlayDirectionalBaseDamageAnimation(damagedCharacter);
        }

        // 오너 아니어도 실행 됨.
        PlayDamageSFX(damagedCharacter);
        // 오너 아니어도 실행 됨.
        PlayDamageVFX(damagedCharacter);


        // 오너만 실행한다.
        CalculateDamage(damagedCharacter);

        // 가장 마지막에 실행해야 하는데 스탠스 브레이크가 발동되면 애니메이션을 피격에서 덮어씌워야 하기 때문.
        // 즉, 데미지를 입는 애니메이션이 시작하자마자 스탠스 브레이크 애니메이션이 시작된다. 
        // 오너만 실행한다.
        CalculateGroggyDamage(damagedCharacter);

        // 오너 아니어도 실행 됨. 
        DisableDamageCollider(damagedCharacter);
        // 캐릭터가 AI라면, 
    }

    /// <summary>
    /// 보스가 기믹을 진행중이고
    /// 데미지가 아닌 타격 횟수만 체크해야 되는 상황에 발동한다.
    /// 기믹 진행 중에는 그로기 데미지는 입지 않고, 체력 또한 깎이지 않는다. 
    /// </summary>
    /// <param name="character"></param>
    private void CalculateHitCountWhenPartiallyInvulnerableState(CharacterManager character)
    {
        if (!character.IsOwner) return;

        if (character.characterGroup == CharacterType.Monster)
        {
            // 캐릭터가 보스몹이라면 
            if (character.TryGetComponent(out AIWarriorGolemCharacterManager boss))
            // if (boss != null)
            {
                // 기믹이 진행중이라 부분 무적상태인데, 피격효과가 발생했다는 것은
                // 부분 무적 상태를 뚫는 공격을 맞았다는 뜻. 데미지는 주지 않으나, 타격 횟수만 증가시킨다.
                if (boss.bossNetworkManager.isPartiallyInvulnerable.Value)
                {
                    // 왜 두 번 올라가지? 
                    boss.bossNetworkManager.gimmickHitCount.Value++;
                    boss.swordWavePattern.RemoveIconServerRpc();
                }
            }
            // 캐릭터가 플레이어라면
            if (character.TryGetComponent(out PlayerManager player))
            {
                // 기믹이 진행중이라 부분 무적상태인데, 피격효과가 발생했다는 것은
                // 부분 무적 상태를 뚫는 공격을 맞았다는 뜻. 데미지는 주지 않으나, 타격 횟수만 증가시킨다.
                if (player.playerNetworkManager.isPartiallyInvulnerable.Value && player.playerNetworkManager.isSealed.Value)
                {
                    player.playerCombatManager.HitCountToRelase--;
                    Debug.Log("여기 들어온거 맞음?");
                    if (player.playerCombatManager.HitCountToRelase <= 0)
                    {
                        player.playerNetworkManager.isPartiallyInvulnerable.Value = false;
                        // sealed가 서버 권한이라 RPC로 풀어준다. 
                        player.playerNetworkManager.SetIsSealedServerRpc(false);
                    }
                }
            }
        }
    }

    public override void ProcessEffectOnServer(CharacterManager character)
    {
        if (character.characterNetworkManager.isInvulnerable.Value) return;

        if (character.isDead.Value) return;

        DisableDamageCollider(character);

    }

    public override void ProcessEffectOnLocal(CharacterManager character)
    {
        if (character.characterNetworkManager.isInvulnerable.Value) return;

        if (character.isDead.Value) return;

        PlayDamageSFX(character);

        PlayDamageVFX(character);

        DisableDamageCollider(character);

    }

    // 현재는 Enemy 에게만 적용 됨 -> 얘를 플레이어에게도 적용시켜주면 어떨까? 여러 대 맞으면 ㅈ 되는 걸로
    private void CalculateGroggyDamage(CharacterManager character)
    {
        if (!character.IsOwner) return;

        if ((character.characterNetworkManager as AIBossCharacterNetworkManager)?.isGimmickInProcess == true)
        {
            return;
        }

        if (IsCharacterPartiallyInvulnerable(character))
        {
            return;
        }

        if (character.characterNetworkManager.isGroggy.Value)
        {
            return;
        }

        AICharacterManager aiCharacter = character as AICharacterManager;

        int stanceDamage = Mathf.RoundToInt(groggyDamage);

        if (aiCharacter != null)
        {
            aiCharacter.aiCharacterCombatManager.ProcessGroggyDamage(stanceDamage);
        }
    }

    private void CalculateDamage(CharacterManager character)
    {
        if (!character.IsOwner) return;

        if (IsCharacterPartiallyInvulnerable(character))
        {
            return;
        }

        // finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lighteningDamage + holyDamage);
        // Debug.Log("물리 데미지 : " + physicalDamage);
        Debug.Log(character.characterCombatManager.DamageDecreaseRate);
        finalDamageDealt = Mathf.RoundToInt(physicalDamage);
        if (character != null)
        {
            finalDamageDealt *= (int)(100 - character.characterCombatManager.DamageDecreaseRate);
            finalDamageDealt /= 100;
        }

        if (finalDamageDealt <= 0)
        {
            finalDamageDealt = 1;
        }
        Debug.Log($"가한 사람 {characterCausingDamage?.name}, 받은 사람 {character.name}, 데미지 : {finalDamageDealt}");
        character.characterNetworkManager.currentHp.Value -= finalDamageDealt;

        // 어그로에 대한 로직이 수치에서 거리로 바꼈기 때문에 주석 처리함.
        character.characterCombatManager.poiseResetTimer = character.characterCombatManager.defaultPoiseResetTime;

        // // NOTE: AGGRO : DMG (a) + POISE (1 - a), what is alpha?
        // if (character.characterGroup == CharacterType.Monster)
        // {
        //     if ((character as AICharacterManager).monsterType == MonsterType.Boss)
        //     {
        //         int AggroValue = finalDamageDealt + (int)poiseDamage;

        //         if (AggroValue > 0)
        //         {
        //             character.GetComponent<AIAggroManager>().SetAggro(characterCausingDamage.NetworkObjectId, AggroValue);
        //         }
        //         //character.GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue
        //     }
        // }
    }


    /// <summary>
    /// 피격 각도에 따라 애니메이션을 달리 시행한다.
    /// </summary>
    /// <param name="character"></param>
    private void PlayDirectionalBaseDamageAnimation(CharacterManager character)
    {
        if (!character.IsOwner) return;

        if (character.isDead.Value) return;

        if (IsCharacterPartiallyInvulnerable(character))
            return;

        if (player?.playerCombatManager.SuperArmorCount > 0)
        {
            player.playerCombatManager.SuperArmorCount--;
            return;
        }

        damageAnimation = string.Empty;

        bool isPerfomingAction = false;

        switch (character.characterGroup)
        {
            // 플레이어가 받는 공격은 아직 라이트와 헤비 뿐.
            // 라이트 공격은 ping animation 실행
            // 헤비 공격은 날아감
            case CharacterType.Player:
                // 맞은 공격이 헤비어택이면 그냥 날라감.

                if (attackType == AttackType.HeavyAttack)
                {
                    // 캐릭터 방향을 돌리고
                    // 뒤로 날라가는 애니메이션을 실행시킨다.
                    if (characterCausingDamage != null)
                    {
                        Vector3 direction = characterCausingDamage.transform.position - character.transform.position;
                        direction.y = 0; // 수평 회전만 고려
                        (character.characterLocomotionManager as PlayerLocomotionManager).PlayerRotationToWardsInputDirection(direction);
                        // (character.characterLocomotionManager as PlayerLocomotionManager).PlayerRotationToWardsInputDirection(characterCausingDamage.transform.position - character.transform.position);
                    }
                    else
                    {
                        Vector3 direction = contactPoint - character.transform.position;
                        direction.y = 0; // 수평 회전만 고려
                        (character.characterLocomotionManager as PlayerLocomotionManager).PlayerRotationToWardsInputDirection(direction);
                    }


                    character.characterAnimatorManager.lastDamageAnimationPlayed = damageAnimation;
                    character.characterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Hit, "HeavyHit_01", true, transitionNomalizedTime: 0f);
                    character.characterNetworkManager.isAttacking.Value = false;
                    disableCollider = true;

                    return;
                }
                // 맞은 공격이 라이트 어택인데 내가 공격중이었으면 모션 캔슬함. 이때 내가 하려던 
                if (character.characterNetworkManager.isAttacking.Value)
                {
                    if (character.characterAnimatorManager.currentAnimationType == AnimationType.SpecialAttack)
                        return;
                    else
                        DecideHitAnimation(character, true);
                    character.characterAnimatorManager.lastDamageAnimationPlayed = damageAnimation;
                    character.characterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Hit, damageAnimation, true);

                    character.characterNetworkManager.isAttacking.Value = false;
                    disableCollider = true;
                    return;
                }
                else if (character.characterNetworkManager.isAttacking.Value == false)
                {
                    DecideHitAnimation(character, false);
                    character.characterAnimatorManager.lastDamageAnimationPlayed = damageAnimation;
                    if (character.characterNetworkManager.isGaurding.Value == true)
                    {
                        character.characterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Hit, targetAnimation: damageAnimation,
                        isPerformingAction: isPerfomingAction, applyRootmotion: false);
                    }
                    else
                    {
                        character.characterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Hit, targetAnimation: damageAnimation,
                        isPerformingAction: isPerfomingAction, applyRootmotion: false, canRotate: true, canMove: true);
                    }
                    return;
                }
                break;

            case CharacterType.Monster:
                // 보스는 피격 애니메이션이 없고 stance break만 존재함.
                // if (character.TryGetComponent(out AICharacterManager aiCharacter) && aiCharacter.monsterType == MonsterType.Boss)
                // {
                //     if (aiCharacter.aiCharacterCombatManager.isGroggy == true) return;
                //     // 플레이어의 공격이 강공격이면 날려보냄. 
                //     if (attackType == AttackType.HeavyAttack)
                //     {
                //         Vector3 directionToTarget = (characterCausingDamage.transform.position - character.transform.position).normalized;
                //         directionToTarget.y = 0; // 수평 회전만 고려

                //         if (directionToTarget != Vector3.zero)
                //         {
                //             Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                //             character.transform.rotation = targetRotation;
                //         }

                //         character.characterAnimatorManager.lastDamageAnimationPlayed = damageAnimation;
                //         character.characterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Hit, "HeavyHit", true);

                //         character.GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("State", EnemyState.Hit);

                //         disableCollider = true;
                //     }
                //     else
                //     {
                //         DecideHitAnimation(character, true);
                //         character.characterAnimatorManager.lastDamageAnimationPlayed = damageAnimation;
                //         character.characterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Hit, damageAnimation, true);

                //         character.GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("State", EnemyState.Hit);

                //         disableCollider = true;
                //     }
                // }
                break;
        }
    }

    private bool IsCharacterPartiallyInvulnerable(CharacterManager character)
    {
        return character.characterNetworkManager.isPartiallyInvulnerable.Value;
        // if (character.characterGroup == CharacterType.Monster)
        // {
        //     AIBossCharacterManager boss = character as AIBossCharacterManager;
        //     if (boss == null) return false;

        //     return boss.bossNetworkManager.isPartiallyInvulnerable.Value;
        // }

        // return false;
    }

    private void DisableDamageCollider(CharacterManager character)
    {
        if (disableCollider)
        {
            character.characterCombatManager.CloseAllDamageCollider();
        }
    }

    private void DecideHitAnimation(CharacterManager character, bool perfomingAction)
    {
        isPerfomingAction = perfomingAction;
        if (perfomingAction)
        {
            // 우측 뒤
            if (angleHitFrom >= 145 && angleHitFrom <= 180)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forward_Medium_Damage);
            }
            // 좌측 뒤
            else if (angleHitFrom <= -145 && angleHitFrom >= -180)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forward_Medium_Damage);
            }
            // 앞
            else if (angleHitFrom >= -45 && angleHitFrom <= 45)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.backward_Medium_Damage);
            }
            // 좌측
            else if (angleHitFrom >= -144 && angleHitFrom <= -45)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.left_Medium_Damage);
            }
            // 우측
            else if (angleHitFrom >= 45 && angleHitFrom <= 144)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.right_Medium_Damage);
            }
        }
        else
        {
            if (angleHitFrom >= 145 && angleHitFrom <= 180)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forward_Light_Damage);
            }
            // 좌측 뒤
            else if (angleHitFrom <= -145 && angleHitFrom >= -180)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forward_Light_Damage);
            }
            // 앞
            else if (angleHitFrom >= -45 && angleHitFrom <= 45)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.backward_Light_Damage);
            }
            // 좌측
            else if (angleHitFrom >= -144 && angleHitFrom <= -45)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.left_Light_Damage);
            }
            // 우측
            else if (angleHitFrom >= 45 && angleHitFrom <= 144)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.right_Light_Damage);
            }
        }
    }
}
