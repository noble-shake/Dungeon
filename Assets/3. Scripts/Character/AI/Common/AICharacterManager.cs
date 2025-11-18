using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Behavior;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class AICharacterManager : CharacterManager
{
    [Header("Chraracter Name")]
    public string characterName = "";

    [Header("Combat Type")]
    public MonsterType monsterType = MonsterType.None;
    public GameObject vinePrefab;

    int vertical;
    int normal;
    int rare;

    [HideInInspector] public AICharacterNetworkManager aiCharacterNetworkManager;
    [HideInInspector] public AICharacterLocomotionManager aiCharacterLocomotionManager;
    [HideInInspector] public AICharacterCombatManager aiCharacterCombatManager;
    [HideInInspector] public AICharacterAnimatorManager aiCharacterAnimatorManager;
    [HideInInspector] public AICharacterEquipmentManager aiCharacterEquipmentManager;
    [HideInInspector] public AICharacterInventoryManager aiCharacterInventoryManager;

    [HideInInspector] public BehaviorGraphAgent behaviorGraphAgent;

    [Header("Navmesh Agent")]
    public NavMeshAgent navMeshAgent;

    [Header("Debug Menu")]
    public bool reviveButton = false;
    [SerializeField] protected bool isSinkable = true;

    protected override void Awake()
    {
        base.Awake();

        aiCharacterNetworkManager = GetComponent<AICharacterNetworkManager>();
        aiCharacterCombatManager = GetComponent<AICharacterCombatManager>();
        aiCharacterLocomotionManager = GetComponent<AICharacterLocomotionManager>();
        aiCharacterAnimatorManager = GetComponent<AICharacterAnimatorManager>();
        aiCharacterEquipmentManager = GetComponent<AICharacterEquipmentManager>();
        aiCharacterInventoryManager = GetComponent<AICharacterInventoryManager>();

        behaviorGraphAgent = GetComponent<BehaviorGraphAgent>();

        navMeshAgent = GetComponentInChildren<NavMeshAgent>();

        vertical = Animator.StringToHash("Vertical");
        normal = Animator.StringToHash("Normal");
        rare = Animator.StringToHash("Rare");

        switch (monsterType)
        {
            case MonsterType.Normal:
            case MonsterType.Elite:
            case MonsterType.Legendary:
            case MonsterType.Rare:
                animator.SetFloat(vertical, 0.5f);
                animator.SetBool(normal, true);
                break;
            // animator.SetFloat(vertical, 1f);
            // animator.SetBool(rare, true);
            // break;
            // animator.SetFloat(vertical, 1f);
            // break;

            default:
                break;
        }


    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            // 상태를 복사해서 쓰는 이유는, 원본 데이터 변경으로 발생하는 미정의 동작을 방지하기 위해서임.

            CheckBeforeEnterPlayMode();

            aiCharacterNetworkManager.maxHp.OnValueChanged += aiCharacterNetworkManager.SetNewMaxHealthValue;
        }

        aiCharacterNetworkManager.isStanceBroken.OnValueChanged += aiCharacterNetworkManager.OnStanceBroken;

        aiCharacterNetworkManager.currentHp.OnValueChanged += aiCharacterNetworkManager.CheckHP;
    }

    protected override void Update()
    {
        // base.Update();
        animator.SetBool("IsGrounded", characterLocomotionManager.isGrounded);
        if (IsOwner)
        {
            characterNetworkManager.networkPosition.Value = transform.position;
            characterNetworkManager.networkRotation.Value = transform.rotation;
        }
        else
        {

            // 현재 위치에서 목표 위치까지의 방향 벡터
            Vector3 targetPosition = aiCharacterNetworkManager.networkPosition.Value;
            Vector3 moveDirection = targetPosition - transform.position;
            characterController.Move(moveDirection);
            transform.rotation = aiCharacterNetworkManager.networkRotation.Value;
        }

        if (reviveButton)
        {
            reviveButton = false;
            ReviveCharacter();
        }

    }

    public override void ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        base.ProcessDeathEvent(manuallySelectDeathAnimation);

        behaviorGraphAgent.BlackboardReference.SetVariableValue("State", EnemyState.Dead);

        if (isSinkable)
        {
            StartCoroutine(SinkIntoGround());
        }
    }

    private IEnumerator SinkIntoGround()
    {
        // 3초 대기
        yield return new WaitForSeconds(3f);

        // 땅속으로 녹아내리는 이펙트
        Vector3 sinkPosition = new Vector3(transform.position.x, transform.position.y - 2f, transform.position.z);
        transform.DOMove(sinkPosition, 4f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            // 캐릭터 비활성화 또는 제거
            gameObject.SetActive(false);
        });
    }


    // 현재 상태의 로직을 실행한다.


    public void CheckBeforeEnterPlayMode()
    {
        // if (CheckCanAttack() == false)
        // {
        //     Debug.LogError($"AI 캐릭터 {gameObject.name} 의 공격 사정거리 변경이 필요합니다.");
        //     EditorApplication.ExitPlaymode();
        // }
    }

    public override void ReviveCharacter()
    {
        base.ReviveCharacter();
        if (IsOwner)
        {
            isDead.Value = false;
            aiCharacterNetworkManager.currentHp.Value = aiCharacterNetworkManager.maxHp.Value;
            aiCharacterNetworkManager.currentStamina.Value = aiCharacterNetworkManager.maxStamina.Value;
            characterController.enabled = true;

            aiCharacterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Etc, "Empty", true);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (characterUIManager.hasFloatingHPBar)
            characterNetworkManager.currentHp.OnValueChanged += characterUIManager.OnHPChanged;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (characterUIManager.hasFloatingHPBar)
            characterNetworkManager.currentHp.OnValueChanged -= characterUIManager.OnHPChanged;
    }

    [ServerRpc]
    public void HitFromVineShotServerRpc()
    {
        HitFromVineShotClientRpc();
    }
    [ClientRpc]
    public void HitFromVineShotClientRpc()
    {
        StartCoroutine(VineHitEffect());
    }

    IEnumerator VineHitEffect(float time = 5f)
    {
        animator.speed = 0f;
        yield return new WaitForSeconds(time);
        animator.speed = 1f;
    }

}
