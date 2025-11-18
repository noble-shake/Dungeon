using System;
using Unity.Behavior;
using UnityEngine;


/// <summary>
/// 피격 시 발생해야 되는 일들과 거기에 필요한 정보를 포함한다.
/// </summary>
[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Inevitable Hit Damage")]
public class TakeInevitableHitEffect : DamageEffect
{

    private bool isPerfomingAction;
    private bool disableCollider;
    public override void ProcessEffect(CharacterManager damagedCharacter)
    {
        if (damagedCharacter.isDead.Value) return;

        disableCollider = false;

        switch (damagedCharacter.characterGroup)
        {
            case CharacterType.Player:
                (damagedCharacter as PlayerManager).playerCombatManager.canComboWithMainHandWeapon = false;
                break;
        }

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


    public override void ProcessEffectOnServer(CharacterManager character)
    {

        if (character.isDead.Value) return;

        DisableDamageCollider(character);

    }

    public override void ProcessEffectOnLocal(CharacterManager character)
    {

        if (character.isDead.Value) return;

        PlayDamageSFX(character);

        PlayDamageVFX(character);

        DisableDamageCollider(character);

    }

    // 현재는 Enemy 에게만 적용 됨 -> 얘를 플레이어에게도 적용시켜주면 어떨까? 여러 대 맞으면 ㅈ 되는 걸로
    private void CalculateGroggyDamage(CharacterManager character)
    {
        if (!character.IsOwner) return;

        if (IsCharacterPartiallyInvulnerable(character))
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
        finalDamageDealt = Mathf.RoundToInt(physicalDamage);

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

        damageAnimation = string.Empty;

        bool isPerfomingAction = false;

        if (character.TryGetComponent(out PlayerManager player))
        {
            player.CoroutineStop();
        }

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
                        (character.characterLocomotionManager as PlayerLocomotionManager).PlayerRotationToWardsInputDirection(characterCausingDamage.transform.position - character.transform.position);
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
