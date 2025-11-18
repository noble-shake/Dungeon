using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class CharacterManager : NetworkBehaviour
{
    [Header("Status")]
    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [HideInInspector] public CharacterNetworkManager characterNetworkManager;
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Animator animator;
    [HideInInspector] public CharacterEffectManager characterEffectManager;
    [HideInInspector] public CharacterAnimatorManager characterAnimatorManager;
    [HideInInspector] public CharacterCombatManager characterCombatManager;
    [HideInInspector] public CharacterSoundFXManager characterSoundFXManager;
    [HideInInspector] public CharacterLocomotionManager characterLocomotionManager;
    [HideInInspector] public CharacterStatsManager characterStatsManager;
    [HideInInspector] public CharacterUIManager characterUIManager;
    [HideInInspector] public CharacterEquipmentManager characterEquipmentManager;
    [HideInInspector] public new Rigidbody rigidbody;

    [Header("Character Group")]
    public CharacterType characterGroup;
    // public CharacterType 

    [Header("Flags")]
    public bool isPerformingAction = false; // 애니메이션 실행 도중에 캔슬 될 수 있는지 없는지를 나타낸다. 
    // public bool isJumping = false;

    protected virtual void Awake()
    {
        // DontDestroyOnLoad(gameObject);

        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        characterNetworkManager = GetComponent<CharacterNetworkManager>();
        characterEffectManager = GetComponent<CharacterEffectManager>();
        characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
        characterStatsManager = GetComponent<CharacterStatsManager>();
        characterCombatManager = GetComponent<CharacterCombatManager>();
        characterSoundFXManager = GetComponent<CharacterSoundFXManager>();
        characterLocomotionManager = GetComponent<CharacterLocomotionManager>();
        characterEquipmentManager = GetComponent<CharacterEquipmentManager>();
        characterUIManager = GetComponent<CharacterUIManager>();
        rigidbody = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        IgnoreMyOwnColliders();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        animator.SetBool("IsGrounded", characterLocomotionManager.isGrounded);
        if (IsOwner)
        {
            characterNetworkManager.networkPosition.Value = transform.position;
            characterNetworkManager.networkRotation.Value = transform.rotation;
        }
        else
        {
            Vector3 targetPosition = characterNetworkManager.networkPosition.Value;

            // 현재 위치에서 목표 위치까지의 방향 벡터
            Vector3 moveDirection = targetPosition - transform.position;

            // CharacterController를 이용한 이동
            // characterController.Move(moveDirection * Time.deltaTime / characterNetworkManager.networkPositionSmoothTime);
            // characterController.Move(moveDirection * Time.deltaTime);
            characterController.Move(moveDirection);

            // transform.rotation = Quaternion.Slerp(transform.rotation, characterNetworkManager.networkRotation.Value, characterNetworkManager.networkRotationSmoothTime);
            transform.rotation = characterNetworkManager.networkRotation.Value;
            // rigidbody.MoveRotation(characterNetworkManager.networkRotation.Value);
        }
    }

    protected virtual void FixedUpdate()
    {

    }

    protected virtual void LateUpdate()
    {

    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {

    }

    // OnEnable
    // OnDisable

    // 플레이어 오브젝트 A 오리지널
    // 맨 처음에 곰탕맨의 컴퓨터에 플레이어오브젝트A가 있지만 혼자 서버에 들어갔음.
    // 노블님이 뒤늦게 서버에 들어갔음.
    // 노블님의 컴퓨터에도 플레이어 오브젝트 A(복제본)가 있습니다.

    // 네트워크에 B라는 오브젝트 있으면

    // 클라이언트들은 그 B의 복제본을 가지고 있음. 

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // 밑에 델리게이트 걸어놓음에도 값을 세팅해주는 이유는, 맨 처음 호스트에 접속했을 때의 상태 초기화를 해주기 위함임.
        animator.SetBool("isMoving", characterNetworkManager.isMoving.Value);

        // 
        characterNetworkManager.OnIsActiveChanged(false, characterNetworkManager.isActive.Value);

        characterNetworkManager.isMoving.OnValueChanged += characterNetworkManager.OnIsMovingChanged;
        characterNetworkManager.isActive.OnValueChanged += characterNetworkManager.OnIsActiveChanged;

        if (IsOwner)
        {
            // 체력 레벨이 변했을 시 거기에 맞게 최대 체력 설정
            // characterNetworkManager.vitality.OnValueChanged += characterNetworkManager.
            // ;
            // 지구력 레벨이 변했을 시 거기에 맞춰 최대 스태미나 설정
            // characterNetworkManager.enduracne.OnValueChanged += characterNetworkManager.SetNewMaxStaminaValue;

            // Debug.Log("캐릭터의 초기 스탯을 설정합니다.");
            // Debug.Log($"체력 : {characterStatsManager.vatality}, 스태미나 : {characterStatsManager.endurance}");

            // characterNetworkManager.vitality.Value = characterStatsManager.vatality;
            // characterNetworkManager.enduracne.Value = characterStatsManager.endurance;
        }
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        characterNetworkManager.isActive.OnValueChanged -= characterNetworkManager.OnIsActiveChanged;
        characterNetworkManager.isMoving.OnValueChanged -= characterNetworkManager.OnIsMovingChanged;
    }

    public virtual void ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        if (IsOwner)
        {
            characterNetworkManager.currentHp.Value = 0;
            isDead.Value = true;
            if (!manuallySelectDeathAnimation)
            {
                characterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Hit, "Dead_01", true, transitionNomalizedTime: 0f);
                // CharacterController characterController = GetComponent<CharacterController>();
                // characterController.enabled = false;

                characterCombatManager.CloseAllDamageCollider();
            }
        }
        (characterLocomotionManager as PlayerLocomotionManager)?.EnableGravitySettings();

    }

    public virtual void ReviveCharacter()
    {

    }

    protected virtual void IgnoreMyOwnColliders()
    {
        Collider charcterControllCollider = GetComponent<Collider>();
        Collider[] damageableCharacterColliders = GetComponentsInChildren<Collider>();
        List<Collider> ignoreColliders = new List<Collider>();

        foreach (var collider in damageableCharacterColliders)
        {
            ignoreColliders.Add(collider);
        }

        ignoreColliders.Add(characterController);

        foreach (var collider in ignoreColliders)
        {
            foreach (var otherCollider in ignoreColliders)
            {
                Physics.IgnoreCollision(collider, otherCollider, true);
            }
        }
    }
}
