using System.Collections;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.VisualScripting;
using UnityEngine;

public class AICharacterCombatManager : CharacterCombatManager
{
    protected AICharacterManager aiCharacter;


    [Header("어그로 애니메이션")]
    public AICharacterAttackAction alertAction;

    [Header("Action Recovery")]
    public float actionRecoveryTime = 0;

    [Header("Pivot")]
    public bool enablePivot = true;

    [Header("Target Information")]
    public float distanceFromTarget;
    public float viewableAngle;
    public Vector3 targetDirection;

    [Header("Detection")]
    public float detectionRadius = 15;
    public float minimumFOV = -35;
    public float maximumFOV = 35;
    public bool onDebuging = true;

    [Header("Attack Rotation Speed")]
    public float attackRotationSpeed = 25f;

    [Header("Stance Settings")]
    public float maxGroggy = 10;
    public float currentGroggy;
    [SerializeField] float groggyRegeneration = 15;
    [SerializeField] bool ignoreStanceBreak = false;

    [Header("Stance Timer")]
    [SerializeField] float groggyRegenerationTimer = 0;
    [SerializeField] float defaultRegenerationTime = 15;
    private float tickTimer = 0;
    public bool isGroggy = false;
    public AICharacterAttackAction currentAttack;
    public AICharacterAttackAction comboAttack;

    [Header("Original Position")]
    public Vector3 originPosition;


    [Header("공격 애니메이션")]
    [SerializeField]
    [Tooltip("인스펙터 창에 빈 칸으로 있으면 안 됨")]
    protected List<AICharacterAttackAction> attackList = new List<AICharacterAttackAction>();
    protected List<AICharacterAttackAction> possibleList = new List<AICharacterAttackAction>();

    protected override void Awake()
    {
        base.Awake();

        aiCharacter = GetComponent<AICharacterManager>();
        lockOnTransform = GetComponentInChildren<LockOnTransform>().transform;

        currentGroggy = maxGroggy;
    }

    protected override void Update()
    {
        base.Update();
        HandleActionRecovery();
        // 그로기 게이지 회복 삭제로 인한 주석 처리.
        // HandleStanceBreak();
    }

    private void HandleStanceBreak()
    {
        if (!aiCharacter.IsOwner) return;

        if (aiCharacter.isDead.Value) return;

        if (isGroggy) return;

        if (groggyRegenerationTimer > 0)
            groggyRegenerationTimer -= Time.deltaTime;
        else
        {
            groggyRegenerationTimer = 0;

            if (currentGroggy < maxGroggy)
            {
                tickTimer += Time.deltaTime;
                if (tickTimer >= 1)
                {
                    tickTimer = 0;
                    currentGroggy += groggyRegeneration;
                }
            }
            else
                currentGroggy = maxGroggy;
        }

        if (currentGroggy <= 0)
        {
            // 해당 로직은 확정적으로 그로기를 넣을 수 있는 스킬의 존재를 예상하고 만든 거라 현재는 비활성화 처리. 
            // DamageIntensity damageIntensity = WorldUtilityManager.instance.GetDamageIntensityBasedOnPoiseDamage(previousPoiseDamageTaken);

            // if (damageIntensity == DamageIntensity.Deadly)
            // {
            //     currentGroggy = 1; // 다음 공격에 바로 스탠스 브레이크가 되도록.
            //     return;
            // }

            currentGroggy = maxGroggy;

            if (ignoreStanceBreak) return;

            // 상태가 변경되며 그로기 애니메이션이 즉발로 실행 됨. 
            if (aiCharacter.GetComponent<BehaviorGraphAgent>().BlackboardReference.GetVariableValue("State", out EnemyState enemyState))
            {
                aiCharacter.GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("State", EnemyState.StanceBreak);
            }
            if (aiCharacter.GetComponent<BehaviorGraphAgent>().BlackboardReference.GetVariableValue("ImprovedEnemyState", out ImprovedEnemyState improvedEnemyState))
            {
                aiCharacter.GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("ImprovedEnemyState", ImprovedEnemyState.Groggy);
            }
            // aiCharacter.ChangeAnimationPlayState();
            // aiCharacter.characterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Hit, "Stance_Break_01", true, transitionNomalizedTime: 0f);
            // aiCharacter.aiCharacterNetworkManager.isStanceBroken.Value = true;

        }
    }

    public void ProcessGroggyDamage(int groggyDamage)
    {
        if (isGroggy) return;

        groggyRegenerationTimer = defaultRegenerationTime;
        currentGroggy -= groggyDamage;
    }

    // 대기 상태일 때, 시야 안에 들어오는 타겟을 찾는다.
    public void FindTargetViaLineOfSight(AICharacterManager aiCharacter)
    {
        if (currentTarget != null) return; // 이미 타겟이 있다면 리턴.


        // 주위의 캐릭터들을 다 찾음.
        Collider[] colliders = Physics.OverlapSphere(aiCharacter.transform.position, detectionRadius, WorldUtilityManager.instance.GetCharacterLayers());


        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager targetCharacter = colliders[i].transform.GetComponent<CharacterManager>();

            // 대상 조건 체크
            if (targetCharacter == null) continue;
            if (targetCharacter == aiCharacter) continue;
            if (targetCharacter.isDead.Value) continue;

            if (WorldUtilityManager.instance.CanIDamageThisTarget(character.characterGroup, targetCharacter.characterGroup))
            {
                Vector3 targetDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                float angleOfPotentialTarget = Vector3.Angle(targetDirection, aiCharacter.transform.forward);

                // 대상이 시야 안에 들어와 있는지 체크
                if (angleOfPotentialTarget > minimumFOV && angleOfPotentialTarget < maximumFOV)
                {
                    // 대상과 본인 사이에 장애물이 있는지 없는지 체크
                    if (Physics.Linecast(aiCharacter.characterCombatManager.lockOnTransform.position, targetCharacter.characterCombatManager.lockOnTransform.position,
                            WorldUtilityManager.instance.GetEnvironmentLayers()))
                    {
                        Debug.DrawLine(aiCharacter.characterCombatManager.lockOnTransform.position, targetCharacter.characterCombatManager.lockOnTransform.position);
                        Debug.Log("Blocked");
                    }
                    else // 장애물이 없다면, 타겟으로 설정.
                    {
                        targetDirection = targetCharacter.transform.position - transform.position;
                        viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(transform, targetDirection);
                        aiCharacter.characterCombatManager.SetTarget(targetCharacter);

                        // if (enablePivot) // 그리고 대상을 향해 회전.
                        // PivotTowardsTarget(aiCharacter);
                    }
                }
            }

        }

    }

    public virtual void PivotTowardsTarget(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isPerformingAction) return;

        if (viewableAngle >= 20 && viewableAngle < 60)
        {
            aiCharacter.characterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, "Turn_R45", true);
        }
        else if (viewableAngle <= -20 && viewableAngle > -60)
        {
            aiCharacter.characterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, "Turn_L45", true);
        }
        else if (viewableAngle >= 60 && viewableAngle < 110)
        {
            aiCharacter.characterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, "Turn_R90", true);
        }
        else if (viewableAngle <= -60 && viewableAngle > -110)
        {
            aiCharacter.characterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, "Turn_L90", true);
        }
        else if (viewableAngle >= 110 && viewableAngle < 145)
        {
            aiCharacter.characterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, "Turn_R135", true);
        }
        else if (viewableAngle <= -110 && viewableAngle > -145)
        {
            aiCharacter.characterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, "Turn_L135", true);
        }
        else if (viewableAngle >= 145 && viewableAngle < 180)
        {
            aiCharacter.characterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, "Turn_R180", true);
        }
        else if (viewableAngle <= -145 && viewableAngle > -180)
        {
            aiCharacter.characterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, "Turn_L180", true);
        }
    }



    public void HandleActionRecovery()
    {
        if (actionRecoveryTime > 0)
        {
            if (!aiCharacter.isPerformingAction)
            {
                actionRecoveryTime -= Time.deltaTime;
            }
        }
    }

    // 

    public virtual AICharacterAttackAction GetPossibleAttack()
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

    public override void EnableCanDoCombo()
    {
        canDoCombo = true;
    }

    public override void DisableCanDoCombo()
    {
        canDoCombo = false;
    }

    /// <summary>
    /// 해당 함수는 콜라이더를 가지고 있는 스크립트에서 오버라이드 해야 한다.
    /// </summary>
    /// <param name="damage"></param>
    public virtual void SetDamageOnCollider(float damage) { }

    public bool IsTargetFound()
    {
        if (currentTarget != null)
        {
            return true;
        }
        return false;
    }

    public override void SetStanceBroken()
    {
        aiCharacter.aiCharacterNetworkManager.isStanceBroken.Value = true;
    }

    public override void ResetStanceBroken()
    {
        aiCharacter.aiCharacterNetworkManager.isStanceBroken.Value = false;
    }

    ///
    ///  
    public void SetCurrentActionByGroup(AttackGroup attackGroup)
    {
        // List<AICharacterAttackAction> possibleAttackList = new List<AICharacterAttackAction>();
        // foreach (var attack in attackList)
        // {
        //     if (attack.attackGroup == attackGroup)
        //     {
        //         attack.attackWeight = 0;
        //         possibleAttackList.Add(attack);
        //     }
        // }

        // Debug.Log("Possible Attack Count : " + possibleAttackList.Count);
        // Debug.Log("Attack Group : " + attackGroup.ToString());
        // int randomIndex = Random.Range(0, possibleAttackList.Count);
        // currentAttack = possibleAttackList[randomIndex];

        List<AICharacterAttackAction> possibleAttackList = new List<AICharacterAttackAction>();
        int totalWeight = 0;

        // 가능한 공격 리스트를 필터링하고 가중치 합산
        foreach (var attack in attackList)
        {
            if (attack.attackGroup == attackGroup)
            {
                totalWeight += attack.attackWeight;
                possibleAttackList.Add(attack);
            }
        }

        Debug.Log("Possible Attack Count: " + possibleAttackList.Count);
        Debug.Log("Attack Group: " + attackGroup.ToString());

        if (possibleAttackList.Count == 0)
        {
            Debug.LogWarning("No possible attacks found for the given attack group.");
            return;
        }

        // 랜덤 값을 생성하여 가중치 기반으로 공격 선택
        int randomValue = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;

        foreach (var attack in possibleAttackList)
        {
            cumulativeWeight += attack.attackWeight;
            if (randomValue <= cumulativeWeight)
            {
                currentAttack = attack;
                Debug.Log("Selected Attack: " + currentAttack.name);
                return;
            }
        }
    }
}
