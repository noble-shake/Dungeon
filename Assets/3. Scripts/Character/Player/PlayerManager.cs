using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using DG.Tweening;
using System;
using Unity.Netcode.Transports.UTP;
using TeleportFX;

// 기본적으로 World Network Manager 프리팹의 Network Object의 플레이어 항목에 등록되어 있는 player 프리팹은 릴레이 서버에 접속할 때 자동으로 생성된다. 
public class PlayerManager : CharacterManager
{
    private ulong playerObjectID;
    public ulong PlayerID { get; set; }

    [Header("DEBUG MENU")]
    [SerializeField] bool respawnCharacter = false;
    [SerializeField] bool killCharacterButton = false;
    [SerializeField] bool swtichRightWeapon = false;

    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector] public PlayerNetworkManager playerNetworkManager;
    [HideInInspector] public PlayerStatsManager playerStatsManager;
    [HideInInspector] public PlayerInventoryManager playerInventoryManager;
    [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;
    [HideInInspector] public PlayerCombatManager playerCombatManager;
    [HideInInspector] public PlayerInteractionManager playerInteractionManager;
    [HideInInspector] public SpecialAttackAction_RiseAndDive riseAndDive;
    [HideInInspector] public SpecialAttackAction_SpearAndBackSlash spearAndBackSlash;
    [HideInInspector] public SpecialAttackAction_Dash dashAction;
    [HideInInspector] public HeavyAttackAction_Parrying parryingAction;
    [HideInInspector] public Shockwave shockwave;
    [HideInInspector] public UltimateAttackAction ultimateAttackAction;
    [HideInInspector] public PlayerEmitter playerSoundManager;

    [HideInInspector] public RainArrowShotSystem rainArrowShotSystem;
    [HideInInspector] public MultipleShotSystem multipleShotSystem;

    [SerializeField] public int initialHealthLv = 5;
    [SerializeField] public int initialStaminaLv = 9;
    [SerializeField] public PlayerClass characterClass;

    [Header("Audio Controller")]
    [HideInInspector] public PlayerEmitter playerEmitter;


    public PlayerClass CharacterClass { get => characterClass; set => characterClass = value; }
    // 카메라 세팅 
    [HideInInspector] public GameObject lockOnFollowTransform;
    [HideInInspector] public GameObject casualFollowTransform;

    public Coroutine coroutineInProcess;

    [Header("맵 이동 시 이펙트")]
    public ParticleSystem dungeonEnterEffect;

    protected override void Awake()
    {
        base.Awake();

        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerNetworkManager = GetComponent<PlayerNetworkManager>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
        playerInventoryManager = GetComponent<PlayerInventoryManager>();
        playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        playerCombatManager = GetComponent<PlayerCombatManager>();
        playerInteractionManager = GetComponentInChildren<PlayerInteractionManager>();
        TryGetComponent<PlayerEmitter>(out playerSoundManager);
        parryingAction = GetComponent<HeavyAttackAction_Parrying>();
        shockwave = GetComponent<Shockwave>();
        ultimateAttackAction = GetComponent<UltimateAttackAction>();
        rainArrowShotSystem = GetComponent<RainArrowShotSystem>();

        playerEmitter = GetComponent<PlayerEmitter>();

        switch (characterClass)
        {
            case PlayerClass.Knight:
                riseAndDive = GetComponent<SpecialAttackAction_RiseAndDive>();
                break;
            case PlayerClass.Warrior:
                dashAction = GetComponent<SpecialAttackAction_Dash>();
                spearAndBackSlash = GetComponent<SpecialAttackAction_SpearAndBackSlash>();
                break;
            case PlayerClass.Archer:

                break;
            case PlayerClass.Thief:
                riseAndDive = GetComponent<SpecialAttackAction_RiseAndDive>();
                dashAction = GetComponent<SpecialAttackAction_Dash>();
                // riseAndDive = GetComponent<SpecialAttackAction_RiseAndDive>();
                break;
        }
        InputManager.instance.InputConnect();

    }

    // Start is called before the first frame update
    protected override void Start()
    {

    }

    protected override void Update()
    {
        base.Update();

        // Debug.Log("현재 RTT는 " + NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>().GetCurrentRtt(OwnerClientId));

        playerLocomotionManager.HandleAllMovement();


        DebugMenu();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void LateUpdate()
    {
        if (!IsOwner) return;

        base.LateUpdate();

        // PlayerCamera.instance.HandleAllCameraActions();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

    }

    protected override void OnDisable()
    {
        base.OnDisable();

    }

    // 네트워크 변수를 사용하는 이유는, 상태를 공유함으로써 네트워크 상에 특정 이벤트를 전파하기 위해서기도 하지만
    // OnValueChanged를 통해 값이 변화할 때 로컬 내부에서 특정 이벤트를 쉽게 걸 수 있기 때문이기도 함.

    // OnNetworkSpawn은 일종의 네트워크 오브젝트의 awake라고 봐도 됨.
    // 
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();


        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;

        if (IsOwner)
        {
            CameraController.instance.InitializeCamera(this);

            // PlayerCamera.instance.player = this;
            InputManager.instance.player = this;
            InputManager.instance.active = true;
            // DOVirtual.DelayedCall(1f, () => InputManager.instance.active = true);

            // 체력 레벨이 변했을 시 거기에 맞게 최대 체력 설정
            playerNetworkManager.maxHp.OnValueChanged += playerNetworkManager.SetNewMaxHealthValue;
            // 체력바 UI 작업
            playerNetworkManager.maxHp.OnValueChanged += playerNetworkManager.SetNewMaxHealthValueOnUIBar;

            playerNetworkManager.currentHp.OnValueChanged += HUD_UIManager.instance.combatUIManager.SetNewHealthValue; // 체력이 변화할 때 HUD UI 업데이트 해줌.
            if (Camera.main.TryGetComponent<DungeonAudioManager>(out DungeonAudioManager DungeonAudioInstance))
            {
                playerNetworkManager.currentHp.OnValueChanged += DungeonAudioInstance.SFXAdjustPlayerHP;
            }


            playerNetworkManager.maxHp.Value = characterCombatManager.hp;

            // 캐릭터의 클래스에 맞게 문양을 바꿈.
            HUD_UIManager.instance.combatUIManager.SetClassIcon(characterClass);

            // 스태미나 로직 주석 처리
            // playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer; // 스태미너 재생 관련 타이머를 초기화함. 
            // playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue; // 스태미나가 변화할 때 HUD UI 업데이트 해줌.
            // playerNetworkManager.enduracne.Value = initialStaminaLv;
            // 지구력 레벨이 변했을 시 거기에 맞춰 최대 스태미나 설정
            // playerNetworkManager.enduracne.OnValueChanged += playerNetworkManager.SetNewMaxStaminaValue;
            // playerNetworkManager.enduracne.OnValueChanged += playerNetworkManager.SetNewMaxStaminaValueOnUIBar;
            // playerNetworkManager.enduracne.Value = playerStatsManager.endurance;

            // 네트워크 스폰이 됐다는 뜻은 씬 완료를 넘어서 캐릭 준비가 됐다는 뜻. 개별적으로 화면 페이드인 처리를 시작한다.
            // 화면이 페이드 아웃 되지 않았으면 아무 상관 없다.
            // StartCoroutine(SceneLoadManager.Instance.FadeIn(1f));
        }
        else
        {
            // 자기 이외의 사람들에게만 보인다.
            // 플레이어가 데미지를 입을 시, 나한테는 HP Bar가 뜨지 않지만, 다른 사람들에게는 뜬다.
            // characterNetworkManager.currentHp.OnValueChanged += characterUIManager.OnHPChanged; // HP 변화 시 stat bar의 업데이트를 위해 사용
            HUD_UIManager.instance.combatUIManager.AddCompanion(this);
        }

        playerNetworkManager.isSealed.OnValueChanged += playerNetworkManager.OnIsSealedChanged;

        playerNetworkManager.currentHp.OnValueChanged += playerNetworkManager.CheckHP; // HP가 0 미만으로 떨어져서 죽음 이벤트를 발생시키는데 사용

        playerNetworkManager.isLockedOn.OnValueChanged += playerNetworkManager.OnIsLockedOnChanged; // 캐릭터의 타게팅 상태를 공유함.
        playerNetworkManager.currentTargetNetworkObjectID.OnValueChanged += playerNetworkManager.OnLockOnTargetIDChange; // 현재 타게팅된 오브젝트 아이디를 공유함.

        playerNetworkManager.currentRightHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentRightHandWeaponIDChange; // 오른손에 들고 있는 장비의 ID를 공유함.
        playerNetworkManager.currentLeftHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentLeftHandWeaponIDChange; // 왼손에 들고 있는 장비의 ID를 공유함.
        playerNetworkManager.currentWeaponBeingUsed.OnValueChanged += playerNetworkManager.OnCurrentWeaponBeingUsedIDChange; // 현재 사용하고 있는 장비의 ID를 공유함. 


        playerNetworkManager.isAttacking.OnValueChanged += playerNetworkManager.OnIsAttackingChanged; // 공격 중인지 상태를 공유함. 
        playerNetworkManager.isGaurding.OnValueChanged += playerNetworkManager.OnIsGaurdingChanged; // 방어 중인지 상태를 공유함.
        playerNetworkManager.isParrying.OnValueChanged += playerNetworkManager.OnIsParryingChanged; // 패링 중인지 상태를 공유함. 
        playerNetworkManager.isInteracting.OnValueChanged += playerNetworkManager.OnIsInteractingChanged; // 상호작용 중인지 상태를 공유함.

        // NOTE: Used by noble, Warrior Special Move 02
        playerNetworkManager.isChargingAttack.OnValueChanged += playerNetworkManager.OnIsChargingattackChanged; // 모으기 공격중인지 상태를 공유함.

        // 캐릭터가 생겼을 때, 자기 발 밑에 소환 이펙트를 생성.
        // ParticleSystem dungeonEnterEffectInstance = Instantiate(dungeonEnterEffect, transform.position + Vector3.up * 0.1f, Quaternion.identity);
        // dungeonEnterEffectInstance.Play();

        if (Camera.main.TryGetComponent<DungeonAudioManager>(out DungeonAudioManager dungeonAudio))
        {
            dungeonAudio.PlayerMaxHP = playerNetworkManager.maxHp.Value;
        }
    }


    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;

        if (IsOwner)
        {
            playerNetworkManager.maxHp.OnValueChanged -= playerNetworkManager.SetNewMaxHealthValue;

            playerNetworkManager.currentHp.OnValueChanged -= HUD_UIManager.instance.combatUIManager.SetNewHealthValue;
            // 스태미나 로직 주석 처리 
            // playerNetworkManager.enduracne.OnValueChanged -= playerNetworkManager.SetNewMaxStaminaValue;
            // playerNetworkManager.currentStamina.OnValueChanged -= PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
            // playerNetworkManager.currentStamina.OnValueChanged -= playerStatsManager.ResetStaminaRegenTimer;
        }
        else
        {
            HUD_UIManager.instance.combatUIManager.RemoveCompanion(this);
            // characterNetworkManager.currentHp.OnValueChanged -= characterUIManager.OnHPChanged;
        }

        playerNetworkManager.currentHp.OnValueChanged -= playerNetworkManager.CheckHP;

        playerNetworkManager.isLockedOn.OnValueChanged -= playerNetworkManager.OnIsLockedOnChanged;
        playerNetworkManager.currentTargetNetworkObjectID.OnValueChanged -= playerNetworkManager.OnLockOnTargetIDChange;

        playerNetworkManager.currentRightHandWeaponID.OnValueChanged -= playerNetworkManager.OnCurrentRightHandWeaponIDChange;
        playerNetworkManager.currentLeftHandWeaponID.OnValueChanged -= playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
        playerNetworkManager.currentWeaponBeingUsed.OnValueChanged -= playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;
        playerNetworkManager.isGaurding.OnValueChanged -= playerNetworkManager.OnIsGaurdingChanged;


        playerNetworkManager.isChargingAttack.OnValueChanged -= playerNetworkManager.OnIsChargingattackChanged;
    }

    // 아무리 봐도 이 함수가 별로 의미가 없는데...
    private void OnClientConnectedCallback(ulong clientID)
    {
        if (!IsHost) return;
        // 현재 접속 리스트를 관리해야 됨
        WorldGameSessionManager.instance.AddPlayerToActivePlayersList(this);

        foreach (var player in WorldGameSessionManager.instance.players)
        {
            if (player != this)
            {
                player.LoadOtherPlayerCharacterWhenJoiningServer();
            }
        }
    }

    public override void ProcessDeathEvent(bool manuallSelectDeathAnimation = false)
    {
        base.ProcessDeathEvent(manuallSelectDeathAnimation);

        if (playerSoundManager != null) playerSoundManager.SFXDead();

        if (IsOwner)
        {
            // 죽었을 때 락온을 해제함.
            if (playerNetworkManager.isLockedOn.Value)
            {
                CameraController.instance.ClearLockOnTargets();
                playerCombatManager.SetTarget(null);
                playerNetworkManager.isLockedOn.Value = false;
            }
            // 다른 사람을 관전하기 위해 카메라 환경을 초기화한다.
            GameManager.Instance.cameraChange.Initialize();
            HUD_UIManager.instance.popUpUIManager.SendYouDiedPopUp(scriptColor: ScriptColor.Red);
        }
    }

    public override void ReviveCharacter()
    {
        base.ReviveCharacter();

        if (IsOwner)
        {
            isDead.Value = false;
            playerNetworkManager.currentHp.Value = playerNetworkManager.maxHp.Value;
            playerNetworkManager.currentStamina.Value = playerNetworkManager.maxStamina.Value;

            // inputmanager를 비활성화해주는 방안도 고려 가능 하지만 시체의 콜라이더를 꺼주기 위해 이렇게 한 듯? 
            GetComponent<CharacterController>().enabled = true;

            // playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Etc, "Empty", false, transitionNomalizedTime: 0f);
            playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Etc, "Revive", false, transitionNomalizedTime: 0f);

            // 부활할 땐 다시 카메라를 고정시킴. 

            CameraController.instance.ChangeCameraFollowToSelf();
        }
    }

    public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
    {
        currentCharacterData.sceneIndex = SceneManager.GetActiveScene().buildIndex;
        currentCharacterData.characterName = playerNetworkManager.characterName.Value.ToString();
        currentCharacterData.xPosition = transform.position.x;
        currentCharacterData.yPosition = transform.position.y;
        currentCharacterData.zPosition = transform.position.z;

        currentCharacterData.vitality = playerNetworkManager.maxHp.Value;
        currentCharacterData.endurance = playerNetworkManager.maxStamina.Value;

        currentCharacterData.currentHealth = playerNetworkManager.currentHp.Value;
        currentCharacterData.currentStamina = playerNetworkManager.currentStamina.Value;

        // currentCharacterData.bossesAwakened 

    }

    public void LoadGameDataFromCurrentCharacterData(ref CharacterSaveData currentCharacterData)
    {
        // playerNetworkManager.characterName.Value = currentCharacterData.characterName;
        // Vector3 myPosition = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition, currentCharacterData.zPosition);
        // transform.position = myPosition;

        // playerNetworkManager.vitality.Value = currentCharacterData.vitality;
        // playerNetworkManager.enduracne.Value = currentCharacterData.endurance;

        // playerNetworkManager.maxHp.Value = playerStatsManager.CalculateHealthBasedOnVitalityLevel(currentCharacterData.vitality);
        // playerNetworkManager.currentHp.Value = currentCharacterData.currentHealth;

        // // playerNetworkManager.maxStamina.Value = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(currentCharacterData.endurance);
        // // playerNetworkManager.currentStamina.Value = currentCharacterData.currentStamina;

        // PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);
        // PlayerUIManager.instance.playerUIHudManager.SetMaxHealthValue(playerNetworkManager.maxHp.Value);
    }

    // 늦게 접속한 클라이언트가 다른 캐릭터 관련 정보들을 읽고 네트워크 상 동기화를 고의로 유발함.
    private void LoadOtherPlayerCharacterWhenJoiningServer()
    {
        // 다른 캐릭터들의 장비 정보 갱신
        playerNetworkManager.OnCurrentRightHandWeaponIDChange(0, playerNetworkManager.currentRightHandWeaponID.Value);
        playerNetworkManager.OnCurrentLeftHandWeaponIDChange(0, playerNetworkManager.currentLeftHandWeaponID.Value);

        // 다른 캐릭터의 방어 상태 갱신
        if (playerNetworkManager.isGaurding.Value == true)
            playerNetworkManager.OnIsGaurdingChanged(false, playerNetworkManager.isGaurding.Value);


        // 다른 캐릭터가 락온 상태인지 업데이트
        if (playerNetworkManager.isLockedOn.Value)
        {
            playerNetworkManager.OnLockOnTargetIDChange(0, playerNetworkManager.currentTargetNetworkObjectID.Value);
        }
    }


    // 나중에 지울 것.
    private void DebugMenu()
    {
        if (respawnCharacter)
        {
            respawnCharacter = false;
            ReviveCharacter();
        }
        if (killCharacterButton)
        {
            killCharacterButton = false;
            KillCharacter();
        }
        if (swtichRightWeapon)
        {
            swtichRightWeapon = false;
            playerEquipmentManager.SwitchRightWeapon();
        }
    }

    public void KillCharacter()
    {
        playerNetworkManager.currentHp.Value = 0;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReviveServerRpc()
    {
        ReviveClientRpc();
    }
    [ClientRpc]
    private void ReviveClientRpc()
    {
        ReviveCharacter();
    }

    public void CoroutineStop()
    {
        if (coroutineInProcess != null)
        {
            StopCoroutine(coroutineInProcess);
            coroutineInProcess = null;
        }
    }

    [ServerRpc]
    public void AppearEffectServerRpc()
    {
        AppearEffectClientRpc();
    }

    [ClientRpc]
    public void AppearEffectClientRpc()
    {
        TryGetComponent<KriptoFX_Teleportation>(out KriptoFX_Teleportation tele);
        if (tele != null)
        {
            tele.TeleportationState = KriptoFX_Teleportation.TeleportationStateEnum.Appear;
            tele.enabled = true;
        }

    }

    [ServerRpc(RequireOwnership = false)]
    public void DisappearEffectServerRpc()
    {
        DisappearEffectClientRpc();
    }

    [ClientRpc]
    public void DisappearEffectClientRpc()
    {
        TryGetComponent<KriptoFX_Teleportation>(out KriptoFX_Teleportation tele);
        if (tele != null)
        {
            tele.TeleportationState = KriptoFX_Teleportation.TeleportationStateEnum.Disappear;
            tele.enabled = true;
        }
    }

    public bool EmitterCheck()
    {
        if (IsOwner) return true;

        return false;
    }



}
