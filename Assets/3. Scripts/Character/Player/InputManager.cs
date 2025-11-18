using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public bool playerInputLimit = false;
    public static InputManager instance;
    public PlayerManager player;
    public LobbySettingManager lobbySettingManager;
    public GameManager gameManager;

    public PlayerControls playerControls;

    [Header("Player Movement Input")]
    public Vector2 movementInput;
    public float verticalInput;
    public float horizontalInput;
    public float moveAmount;

    [Header("Camera Movement Input")]
    [SerializeField] public Vector2 cameraInput;
    [SerializeField] public float cameraSensitivility;
    [SerializeField] bool cameraChangeInput;

    [Header("Player Actions Input")]
    [SerializeField] bool shiftShortInput = false; // 쉬프트 키 짧게
    [SerializeField] bool shiftLongInput = false; // 쉬프트 키 길게
    [SerializeField] bool shiftLongInputValid = false; // 쉬프트 키 길게 유효한지. 쉬프트 키를 길게 눌렀다가 뗄 때를 위함.
    [SerializeField] bool spaceBarShortInput = false; // 스페이스 키 짧게
    [SerializeField] bool spaceBarLongInput = false; // 스페이스 키 길게
    [SerializeField] bool leftClickInput = false; // 좌클릭
    [SerializeField] bool leftClickLongInput = false; // 좌클릭
    [SerializeField] bool leftClickLongValid = false; // 좌클릭
    [SerializeField] bool rightClickInput = false; // 우클릭
    // [SerializeField] bool holdRightClickInput = false;
    [SerializeField] bool switchRightWeaponInput = false;
    [SerializeField] bool switchLeftWeaponInput = false;
    [SerializeField] bool interactInput = false; // E 키 



    [Header("Qued Inputs")]
    private bool inputQueIsActive = false;
    [SerializeField] float defaultQueInputTime = 0.35f;
    [SerializeField] float queInputTimer = 0;
    [SerializeField] bool queLeftClickInput = false;
    [SerializeField] bool queRightClickInput = false;
    [SerializeField] bool queSpaceBarInput = false;

    [Header("Lock On Input")]
    [SerializeField] bool lockOnInput = false; // F키
    [SerializeField] bool lockOnAllyInput = false; // F키 오래 누를 때
    [SerializeField] bool lockOnLeftInput = false; // Q키
    [SerializeField] bool lockOnRightInput = false; // E키
    [SerializeField] bool autoRelockOnWhenTargetDead = true; // 타겟이 죽었을 때 근처 대상을 자동을 리타겟 한다.

    [Header("UI 관련 모음")]
    public UIControls uiControls;
    [SerializeField] bool leftNextInput = false;
    [SerializeField] bool rightNextInput = false;
    [SerializeField] bool selectInput = false;

    [SerializeField] bool escInput = false;
    [SerializeField] bool f1Input = false;


    [SerializeField] bool hold = false;
    [SerializeField] float holdTime = 0f;

    public bool active = false;
    private bool rightClickBlock = false;
    private bool spacebarBlock = false;
    private bool rInputBlock = false;
    private bool qInputBlock = false;
    private bool shiftInputBlock = false;
    public Coroutine parryingInputCoroutine;

    public float HoldTime { get => holdTime; set => holdTime = value; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 맨 처음 씬이 활성화될 때도 호출됨. 
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

    }

    private void Update()
    {
        if (player != null && !playerInputLimit)
            HandleAllInputs();

        HandleAllUIInputs();
        InputHoldCheck();
    }

    private void HandleAllUIInputs()
    {
        // if (player != null)
        //     return;
        HandleSkillDescInput();
        HandleSideNextInput();
        HandleSelectInput();
        HandleEscInput();
    }

    // ESC를 누르면 상황별로 다름.
    // 1. 부팅 씬에서 UI 제거
    // 2. 로비 씬에서 나가기 버튼 확인?
    // 3. 인게임에서 인게임 메뉴창 활성화

    private void HandleEscInput()
    {
        if (escInput)
        {
            escInput = false;

            Debug.Log("ESC 키 입력됨");

            // 현재 씬 이름을 얻어온다.
            string currentSceneName = SceneManager.GetActiveScene().name;
            if (currentSceneName.StartsWith("Dungeon"))
            {
                // 설정 UI창이 닫혀있다면 연다.
                if (HUD_UIManager.instance.IsActiveInGameUIWindow() == false)
                {
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                    active = false;
                    HUD_UIManager.instance.OpenInGameUIWindow();
                }
                else
                {
                    // 설정 UI창이 열려있다면 닫는다.
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    active = true;
                    HUD_UIManager.instance.CloseInGameUIWindow();
                }
                return;
            }

            // if (AudioControlManager.Instance != null)
            // {
            //     if (AudioControlManager.Instance.gameObject.activeSelf)
            //     {
            //         playerControls.PlayerActions.Enable();
            //         playerControls.PlayerCamera.Enable();
            //         playerControls.PlayerMovement.Enable();

            //         AudioControlManager.Instance.gameObject.SetActive(false);

            //         if (GameManager.Instance != null)
            //         {
            //             Cursor.lockState = CursorLockMode.Locked;
            //             Cursor.visible = false;
            //             active = true;
            //         }
            //     }
            //     else
            //     {
            //         movementInput = Vector2.zero;
            //         cameraInput = Vector2.zero;
            //         playerControls.PlayerActions.Disable();
            //         playerControls.PlayerCamera.Disable();
            //         playerControls.PlayerMovement.Disable();

            //         AudioControlManager.Instance.gameObject.SetActive(true);
            //         Cursor.lockState = CursorLockMode.None;
            //         Cursor.visible = true;
            //         active = false;
            //     }
            // }
            // else
            {
                if (Cursor.lockState == CursorLockMode.None)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    active = true;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    active = false;

                    // playerControls.Disable();
                }
            }
        }
    }

    private void HandleSkillDescInput()
    {
        if (player == null)
        {
            return;
        }
        if (f1Input)
        {
            f1Input = false;

            Debug.Log("ESC 키 입력됨");

            // 현재 씬 이름을 얻어온다.
            string currentSceneName = SceneManager.GetActiveScene().name;
            if (currentSceneName.StartsWith("Dungeon"))
            {
                HUD_UIManager.instance.ShowSkillDescUIWindow(player.characterClass);
            }
        }
    }

    private void HandleSelectInput()
    {
        if (selectInput)
        {
            selectInput = false;

            if (lobbySettingManager == null)
                return;

            // lobbySettingManager.GetReady();

        }
    }

    private void HandleSideNextInput()
    {
        if (leftNextInput)
        {
            leftNextInput = false;

            if (lobbySettingManager == null)
                return;

            lobbySettingManager.LeftNext();
        }

        if (rightNextInput)
        {
            rightNextInput = false;

            if (lobbySettingManager == null)
                return;

            lobbySettingManager.rightNext();
        }
    }

    private void HandleAllInputs()
    {
        if (active == false)
        {
            return;
        }
        if (player == null)
        {
            return;
        }
        HandleCameraChange();
        // 플레이어의 죽음을 미리 체크해주고 함수 실행 자체를 거르지 않는 이유
        // 1. 인풋이 들어왔을 때 그 인풋이 걸려있다가 플레이어가 부활하게 될 때 발동되는 경우가 있음.

        if (gameManager == null)
        {
            return;
        }

        HandleLockOnInput(); // 적 캐릭터 조준 입력, F키를 짧게 누를 때 발동
        HandleLockOnAllyInput(); // 아군 캐릭터 조준 입력, F키를 길게 누를 때 발동 
        UpdateLockOn(); // 락온 상태에서 일어나야 할 일들을 관리
        HandleLockOnSwitchTargetInput(); // 조준 대상 변경 입력
        // HandleCameraMovementInput(); // 카메라 이동 입력

        HandleMovementInput(); // 이동 입력
        // HandleJumpInput();

        HandleLeftClickInput(); // 좌클릭 입력
        HandleLeftLongInputPerformed(); // 좌클릭 입력
        HandleLeftLongInputCanceled(); // 좌클릭 입력
        HandleRightClickInput(); // 우클릭 입력
        HandleQuedInputs(); // 입력 큐잉 담당 

        HandleSpaceBarShortInput(); // 캐릭터별 회피 액션 입력
        HandleSpaceBarLongInput(); // 캐릭터별 버프 발동

        HandleShiftShortInput(); // 쉬프트 짧게 입력
        HandleShiftLongInputPressed(); // 쉬프트 입력

        HandleSwtichLeftWeaponInput();

        HandleSwtichRightWeaponInput();

        HandleInteractionInput();
    }

    private void HandleCameraChange()
    {
        if (player == null)
            return;

        if (player.isDead.Value == false)
        {
            return;
        }
        if (cameraChangeInput)
        {
            cameraChangeInput = false;
            if (player.isDead.Value == false)
                return;

            GameManager.Instance.cameraChange.CameraChange();
        }

    }

    private void OnEnable()
    {
        InputBootingConnect();
        //InputConnect();

        //if (uiControls == null)
        //{

        //    uiControls = new UIControls();
        //    uiControls.UINavigation.LeftNext.performed += context => leftNextInput = true;
        //    uiControls.UINavigation.RightNext.performed += context => rightNextInput = true;
        //    uiControls.UINavigation.Select.performed += context => selectInput = true;

        //}
        //uiControls.Enable();
    }

    // ESC를 눌렀을 때 

    public void InputBootingConnect()
    {
        playerControls = new PlayerControls();
        playerControls.UI.Next.performed += context => leftNextInput = true;
        playerControls.UI.AnyKey.performed += context => { };
        playerControls.UI.LeftClick.performed += context => selectInput = true;
        playerControls.UI.RightClick.performed += context => rightNextInput = true;
        playerControls.UI.Esc.performed += context => escInput = true;
        playerControls.UI.F1.performed += context => f1Input = true;

        playerControls.UI.Enable();
    }

    public void InputConnect()
    {
        //if (playerControls == null)
        //{
        playerControls = new PlayerControls();

        playerControls.PlayerCamera.PlayerCameraChange.performed += context => cameraChangeInput = true;

        playerControls.PlayerMovement.Movement.performed += context => movementInput = context.ReadValue<Vector2>();
        playerControls.PlayerCamera.Movement.performed += context => cameraInput = context.ReadValue<Vector2>() * cameraSensitivility * Time.deltaTime;
        playerControls.PlayerActions.SpecialAction01.performed += context => shiftShortInput = true;
        playerControls.PlayerActions.SpecialAction02.performed += context => shiftLongInput = true;
        playerControls.PlayerActions.SpecialAction02.canceled += context => HandleShiftLongInputCanceled();
        playerControls.PlayerActions.Evade.performed += context => spaceBarShortInput = true;
        playerControls.PlayerActions.Buff.performed += context => spaceBarLongInput = true;
        playerControls.PlayerActions.Interact.performed += context => interactInput = true;
        playerControls.PlayerActions.Interact.canceled += context => interactInput = true;

        // playerControls.PlayerActions.Block.canceled += context => player.playerNetworkManager.isBlocking.Value = false;

        // 공격 입력
        playerControls.PlayerActions.LeftClick.performed += context => leftClickInput = true;
        //playerControls.PlayerActions.LeftClickLong.performed += context => leftClickLongInput = true;
        //playerControls.PlayerActions.LeftClickLong.canceled += context => HandleLeftLongInputCanceled();
        playerControls.PlayerActions.RightClick.performed += context => rightClickInput = true;
        // playerControls.PlayerActions.HoldRightClick.performed += context => holdRightClickInput = true;
        // playerControls.PlayerActions.HoldRightClick.canceled += context => holdRightClickInput = false;

        // 락온 입력
        playerControls.PlayerActions.LockOn.performed += context => lockOnInput = true;
        playerControls.PlayerActions.LockOnAllly.performed += context => lockOnAllyInput = true;
        playerControls.PlayerActions.SeekLeftLockOnTarget.performed += context => lockOnLeftInput = true;
        playerControls.PlayerActions.SeekRightLockOnTarget.performed += context => lockOnRightInput = true;

        //무기 교체 입력
        playerControls.PlayerActions.SwitchRightWeapon.performed += context => switchRightWeaponInput = true;
        playerControls.PlayerActions.SwitchLeftWeapon.performed += context => switchLeftWeaponInput = true;

        // 큐잉 인풋
        playerControls.PlayerActions.QueLeftClick.performed += context => QueInput(ref queLeftClickInput);
        // 우클릭 큐잉하니까 패링이 연속으로 일어나는 버그가 있음. 꺼도 돼서 끔. 꺼야 제대로 동작함. 
        // playerControls.PlayerActions.QueRightClick.performed += context => QueInput(ref queRightClickInput);
        // 스페이스 바 큐잉하니까 구르기가 너무 연달아 일어남.
        // playerControls.PlayerActions.QueSpaceBar.performed += context => QueInput(ref queSpaceBarInput);
        playerControls.UI.Esc.performed += context => escInput = true;
        playerControls.UI.Next.performed += context => leftNextInput = true;
        playerControls.UI.AnyKey.performed += context => { };
        playerControls.UI.LeftClick.performed += context => selectInput = true;
        playerControls.UI.RightClick.performed += context => rightNextInput = true;

        playerControls.PlayerCamera.Look.performed += context => context.ReadValue<Vector2>();
        //}

        playerControls.Enable();
    }

    private void QueInput(ref bool quedInput)
    {
        // 다른 큐잉의 대상이 되는 입력값들도 false로 초기화 해주어야 함.
        // 이유는 간단한데, 가장 최근에 입력된 값으로 동작이 실행되어야 하기 때문.
        // 약공, 강공 모두 큐잉중이라면 둘 중 하나만 처리해야 함.
        queLeftClickInput = false;
        queRightClickInput = false;
        queSpaceBarInput = false;

        if (player == null) return;

        if (player.isPerformingAction || player.playerNetworkManager.isJumping.Value)
        {
            quedInput = true;
            queInputTimer = defaultQueInputTime;
            inputQueIsActive = true;
        }
    }

    private void HandleQuedInputs()
    {
        if (player.isDead.Value)
        {
            return;
        }
        if (inputQueIsActive)
        {
            if (queInputTimer > 0)
            {
                queInputTimer -= Time.deltaTime;
                ProcessQuedInput();
            }
            else
            {
                queLeftClickInput = false;
                queRightClickInput = false;
                queSpaceBarInput = false;

                inputQueIsActive = false;
                queInputTimer = 0;
            }
        }
    }

    private void HandleInteractionInput()
    {
        if (player.isDead.Value)
        {
            interactInput = false;
            return;
        }
        if (interactInput)
        {
            interactInput = false;
            if (player.playerInteractionManager.IsInteracting) // 상호작용 중이라면 취소
            {
                player.playerInteractionManager.CancelInteracting();
            }
            else // 상호작용 중이 아니라면 상호작용 시도
            {
                player.playerInteractionManager.AttempToInteract();
            }
        }
    }

    // 예를 들어서, 구르기 하고 있었음.

    // 근데 구르기 할 때는 공격을 못 함 

    // 구르기가 끝나갈 때 공격을 했음 클릭을 했음.

    // 그러면 구르기가 끝나자마자 공격이 실행됨.

    // 콤보공격을 한다면 ? 

    // leftclick leftclick leftclick leftclick

    // 어떤 공격이냐에 따름. 근데? 방금 든 생각인데 패링은 필요 없지 않나? 

    // left click <- essential 
    // right click (parrying) <- essential?  

    private void ProcessQuedInput()
    {
        if (player.isDead.Value) return;
        if (queLeftClickInput) leftClickInput = true;
        if (queRightClickInput) rightClickInput = true;
        if (queSpaceBarInput) spaceBarShortInput = true;
    }

    private void HandleMovementInput()
    {
        if (player.isDead.Value)
        {
            moveAmount = 0;
            player.playerNetworkManager.isMoving.Value = false;
            return;
        }
        if (player == null)
            return;

        if (player.rainArrowShotSystem != null)
        {
            if (player.rainArrowShotSystem.isShotReady)
            {
                moveAmount = 0;
                player.playerNetworkManager.isMoving.Value = false;
                return;

            }
        }

        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
        moveAmount = Mathf.Clamp01(Math.Abs(horizontalInput) + Math.Abs(verticalInput));

        if (moveAmount > 0 && moveAmount <= 0.5)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5f && moveAmount <= 1)
        {
            moveAmount = 1f;
        }


        if (moveAmount != 0)
        {
            player.playerNetworkManager.isMoving.Value = true;
        }
        else
        {
            player.playerNetworkManager.isMoving.Value = false;
        }

        // 수평 인풋을 0으로 설정해놓는 이유는, 상대방이 조준이 안 된 상태에선 무조건 앞으로만 향해야 되기 때문.
        // if (!player.playerNetworkManager.isLockedOn.Value || player.playerNetworkManager.isSprinting.Value)
        // {
        //     player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);
        // }
        // else
        // {
        //     player.playerAnimatorManager.UpdateAnimatorMovementParameters(horizontalInput, verticalInput, player.playerNetworkManager.isSprinting.Value);
        // }
    }

    private void HandleShiftShortInput()
    {
        if (qInputBlock)
        {
            shiftShortInput = false;
            return;
        }

        if (shiftShortInput)
        {
            shiftShortInput = false;

            WeaponAction specialAttack = null;
            Weapon weapon = null;


            switch (player.CharacterClass)
            {
                case PlayerClass.Knight:
                    player.playerNetworkManager.SetCharacterActionHand(true); // 오른손 사용중

                    // 가드중이지 않았다면 가드를 발동한다. 
                    specialAttack = player.playerInventoryManager.currentRightHandWeapon.specialAttackAction_01;
                    weapon = player.playerInventoryManager.currentRightHandWeapon;
                    player.playerCombatManager.PerformWeaponBasedAction(specialAttack, weapon);
                    // if (!player.playerNetworkManager.isGaurding.Value)
                    // {

                    //     specialAttack = player.playerInventoryManager.currentRightHandWeapon.specialAttackAction_01;
                    //     weapon = player.playerInventoryManager.currentRightHandWeapon;
                    //     player.playerCombatManager.PerformWeaponBasedAction(specialAttack, weapon);
                    // }
                    // else
                    // {
                    //     // 가드중이었다면, 가드를 해제한다. 이 때 네트워크 변수에 걸려있는 델리게이트로 인해
                    //     // 클라이언트에 가드 해제 트리거가 활성화 된다. 
                    //     player.playerNetworkManager.isGaurding.Value = false;
                    // }
                    break;

                case PlayerClass.Warrior:
                    if (player.playerNetworkManager.isLockedOn.Value)
                    {
                        player.playerNetworkManager.SetCharacterActionHand(true); // 오른손 사용중

                        specialAttack = player.playerInventoryManager.currentRightHandWeapon.specialAttackAction_01;
                        weapon = player.playerInventoryManager.currentRightHandWeapon;

                        player.playerCombatManager.PerformWeaponBasedAction(specialAttack, weapon);
                        // player.riseAndDive.divePosition = player.playerCombatManager.currentTarget.transform.position;
                    }
                    else // 락온 중이 아니라면 
                    {
                        player.playerNetworkManager.SetCharacterActionHand(true); // 오른손 사용중

                        specialAttack = player.playerInventoryManager.currentRightHandWeapon.specialAttackAction_01;
                        weapon = player.playerInventoryManager.currentRightHandWeapon;

                        player.playerCombatManager.PerformWeaponBasedAction(specialAttack, weapon);
                    }


                    break;

                case PlayerClass.Thief:

                    player.playerNetworkManager.SetCharacterActionHand(true); // 오른손 사용중

                    specialAttack = player.playerInventoryManager.currentRightHandWeapon.specialAttackAction_01;
                    weapon = player.playerInventoryManager.currentRightHandWeapon;

                    player.playerCombatManager.PerformWeaponBasedAction(specialAttack, weapon);

                    break;

                case PlayerClass.Archer:
                    // if (player.playerNetworkManager.isLockedOn.Value) return;

                    Vector3 targetDirection = Vector3.zero;
                    targetDirection = Camera.main.transform.forward;
                    targetDirection.Normalize();
                    targetDirection.y = 0;

                    if (targetDirection == Vector3.zero)
                        targetDirection = transform.forward;

                    Debug.Log("how much run?");
                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                    player.transform.rotation = targetRotation;

                    player.playerNetworkManager.SetCharacterActionHand(false);

                    specialAttack = player.playerInventoryManager.currentRightHandWeapon.specialAttackAction_01;
                    weapon = player.playerInventoryManager.currentRightHandWeapon;

                    player.playerCombatManager.PerformWeaponBasedAction(specialAttack, weapon);

                    break;
                default:
                    Debug.LogError("캐릭터의 클래스가 설정되지 않았습니다.");
                    break;

            }
        }
    }

    private void HandleShiftLongInputPressed()
    {
        if (shiftInputBlock)
        {
            shiftLongInput = false;
            return;
        }

        if (player.isDead.Value)
        {
            shiftLongInput = false;
            return;
        }
        if (shiftLongInput)
        {
            // 얘가 왜 있더라? 
            shiftLongInputValid = true;
            shiftLongInput = false;

            WeaponAction specialAttack = null;
            Weapon weapon = null;

            switch (player.CharacterClass)
            {
                case PlayerClass.Knight:
                    {
                        // 누군갈 조준 안하고 있다면, 플레이어만 조준할 수 있게끔 설정을 변경. 
                        if (player.playerNetworkManager.isLockedOn.Value == false)
                            CameraController.instance.SetTargetType(CharacterType.Player);
                        // 누군갈 이미 조준하고 있다면 아무 일 없음. 
                    }
                    break;


                // 락온 타겟을 사람으로 조정한다. 


                case PlayerClass.Warrior:
                    player.playerNetworkManager.SetCharacterActionHand(true); // 오른손 사용중


                    player.playerNetworkManager.isChargingAttack.Value = true;
                    hold = true;
                    HoldTime = 0f;

                    break;

                case PlayerClass.Thief:

                    break;

                case PlayerClass.Archer:
                    Vector3 targetDirection = Vector3.zero;
                    targetDirection = Camera.main.transform.forward;
                    targetDirection.Normalize();
                    targetDirection.y = 0;

                    if (targetDirection == Vector3.zero)
                        targetDirection = transform.forward;

                    Debug.Log("how much run?");
                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                    player.transform.rotation = targetRotation;
                    player.rainArrowShotSystem.OnShotReady();
                    break;
                default:
                    Debug.LogError("캐릭터의 클래스가 설정되지 않았습니다.");
                    break;

            }
        }
    }

    private void HandleLeftLongInputPerformed()
    {
        if (player.isDead.Value)
        {
            shiftLongInput = false;
            return;
        }



    }

    private void HandleLeftLongInputCanceled()
    {
        //if (leftClickLongValid == false) return;
        //leftClickLongValid = false;
        WeaponAction specialAttack = null;
        Weapon weapon = null;


        switch (player.CharacterClass)
        {
            case PlayerClass.Archer:
                if (player.playerNetworkManager.isLockedOn.Value)
                {
                    // shot.
                }
                break;
            default:
                break;
        }
    }

    private void HandleShiftLongInputCanceled()
    {
        if (shiftLongInputValid == false) return;
        shiftLongInputValid = false;
        WeaponAction specialAttack = null;
        Weapon weapon = null;
        switch (player.CharacterClass)
        {
            case PlayerClass.Knight:
                // 만약 플레이어가 락온 중이라면 바로 도약을 실행한다.
                if (player.playerNetworkManager.isLockedOn.Value)
                {
                    player.playerNetworkManager.SetCharacterActionHand(true); // 오른손 사용중

                    specialAttack = player.playerInventoryManager.currentRightHandWeapon.specialAttackAction_02;
                    weapon = player.playerInventoryManager.currentRightHandWeapon;

                    player.playerCombatManager.PerformWeaponBasedAction(specialAttack, weapon);
                    player.riseAndDive.divePosition = player.playerCombatManager.currentTarget.transform.position;
                }
                else // 락온 중이 아니라면 
                {
                    player.playerNetworkManager.SetCharacterActionHand(true); // 오른손 사용중

                    specialAttack = player.playerInventoryManager.currentRightHandWeapon.specialAttackAction_02;
                    weapon = player.playerInventoryManager.currentRightHandWeapon;

                    player.playerCombatManager.PerformWeaponBasedAction(specialAttack, weapon);
                    HoldTime = 0f;
                }
                break;

            case PlayerClass.Warrior:
                player.playerNetworkManager.SetCharacterActionHand(true); // 오른손 사용중

                specialAttack = player.playerInventoryManager.currentRightHandWeapon.specialAttackAction_02;
                weapon = player.playerInventoryManager.currentRightHandWeapon;
                player.playerNetworkManager.isChargingAttack.Value = false;

                player.playerCombatManager.PerformWeaponBasedAction(specialAttack, weapon);


                //specialAttack = player.playerInventoryManager.currentRightHandWeapon.specialAttackAction_02;
                //weapon = player.playerInventoryManager.currentRightHandWeapon;

                //player.playerCombatManager.PerformWeaponBasedAction(specialAttack, weapon);

                break;

            case PlayerClass.Thief:
                player.playerNetworkManager.SetCharacterActionHand(true); // 오른손 사용중


                specialAttack = player.playerInventoryManager.currentRightHandWeapon.specialAttackAction_02;
                weapon = player.playerInventoryManager.currentRightHandWeapon;

                player.playerCombatManager.PerformWeaponBasedAction(specialAttack, weapon);

                break;

            case PlayerClass.Archer:
                if (player.rainArrowShotSystem.isShotReady)
                {
                    player.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.SpecialAttack, "Main_Special_Attack_02_End", true, canRotate: true, canMove: false);

                }
                break;
            default:
                Debug.LogError("캐릭터의 클래스가 설정되지 않았습니다.");
                break;

        }

        CameraController.instance.SetTargetType(CharacterType.Monster);
    }

    private void HandleSpaceBarLongInput()
    {
        if (rInputBlock)
        {
            spaceBarLongInput = false;
            return;
        }
        if (spaceBarLongInput)
        {
            spaceBarLongInput = false;

            player.playerNetworkManager.SetCharacterActionHand(true); // 오른손 사용중

            WeaponAction buffAction = player.playerInventoryManager.currentRightHandWeapon.buffAction;
            Weapon rightHandWeapon = player.playerInventoryManager.currentRightHandWeapon;

            player.playerCombatManager.PerformWeaponBasedAction(buffAction, rightHandWeapon);

            // player.playerLocomotionManager.AttempToPerformJump();

        }
    }

    private void HandleSpaceBarShortInput()
    {
        if (spacebarBlock)
        {
            spaceBarShortInput = false;
            return;
        }
        if (player.isDead.Value)
        {
            spaceBarShortInput = false;
            return;
        }
        if (spaceBarShortInput)
        {
            spaceBarShortInput = false;
            player.playerNetworkManager.SetCharacterActionHand(true); // 오른손 사용중

            WeaponAction evadeAction = player.playerInventoryManager.currentRightHandWeapon.evadeAction;
            Weapon rightHandWeapon = player.playerInventoryManager.currentRightHandWeapon;

            player.playerCombatManager.PerformWeaponBasedAction(evadeAction, rightHandWeapon);
        }
    }

    // 메인 무기의 공격을 담당한다.
    private void HandleLeftClickInput()
    {

        if (player.isDead.Value)
        {
            leftClickInput = false;
            return;
        }
        if (player.TryGetComponent<AimShotSystem>(out AimShotSystem aimShot) && player.playerNetworkManager.isLockedOn.Value)
        {
            return;
        }

        if (leftClickInput)
        {
            queLeftClickInput = false;
            leftClickInput = false;
            player.playerNetworkManager.SetCharacterActionHand(true); // 오른손 사용중중

            // 오른손에 쥐어진 무기가 주무기임. 
            WeaponAction leftClickAction = player.playerInventoryManager.currentRightHandWeapon.lightAttackAction;
            Weapon rightHandWeapon = player.playerInventoryManager.currentRightHandWeapon;

            if (player.characterClass == PlayerClass.Archer)
            {
                Vector3 targetDirection = Vector3.zero;
                targetDirection = Camera.main.transform.forward;
                targetDirection.Normalize();
                targetDirection.y = 0;

                if (targetDirection == Vector3.zero)
                    targetDirection = transform.forward;

                Debug.Log("how much run?");
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                player.transform.rotation = targetRotation;
            }
            player.playerCombatManager.PerformWeaponBasedAction(leftClickAction, rightHandWeapon);
        }
    }

    private void HandleRightClickInput()
    {
        if (rightClickBlock)
        {
            rightClickInput = false;
            return;
        }
        if (player.isDead.Value)
        {
            rightClickInput = false;
            return;
        }
        if (rightClickInput)
        {
            rightClickInput = false;

            WeaponAction heavyAttack = null;
            Weapon mainWeapon = null;
            switch (player.CharacterClass)
            {
                case PlayerClass.Knight:
                case PlayerClass.Thief:
                case PlayerClass.Warrior:
                case PlayerClass.Archer:
                    {
                        // player.GetComponent<HeavyAttackAction_Parrying>().Parrying();
                        player.playerNetworkManager.SetCharacterActionHand(false); // 왼손 사용중
                        heavyAttack = player.playerInventoryManager.currentRightHandWeapon.heavyAttackAction;
                        mainWeapon = player.playerInventoryManager.currentRightHandWeapon;
                        player.playerCombatManager.PerformWeaponBasedAction(heavyAttack, mainWeapon);
                    }
                    break;

                default:
                    Debug.LogError("캐릭터의 클래스가 설정되지 않았습니다.");
                    break;

            }
        }
    }

    // NOTE : 우클릭이 차지공격일 때 사용되었다가 현재 사용되지 않고 있음. by 곰탕맨
    private void HandleChargeRightClickInput()
    {
        if (player.isPerformingAction)
        {
            if (player.playerNetworkManager.isUsingRightHand.Value)
            {
                // player.playerNetworkManager.isChargingAttack.Value = holdRightClickInput;
            }
        }
    }

    private void HandleLockOnInput()
    {
        if (player.isDead.Value)
        {
            lockOnInput = false;
            return;
        }

        // 맨 처음 락온 할 때
        if (lockOnInput && !player.playerNetworkManager.isLockedOn.Value)
        {
            lockOnInput = false;

            if (player.characterClass == PlayerClass.Archer)
            {
                CameraController.instance.ClearLockOnTargets();
                player.playerCombatManager.SetAimMode();
                //player.GetComponent<AimShotSystem>().AimReady();
                //player.playerNetworkManager.isLockedOn.Value = true;
            }
            else
            {
                CameraController.instance.ClearLockOnTargets();
                if (CameraController.instance.FindLockOnTargets())
                {
                    player.playerCombatManager.SetTarget(CameraController.instance.nearestLockOnTarget);
                    player.playerNetworkManager.isLockedOn.Value = true;
                }
                return;
            }


        }

        // 락온된 상태에서 다시 락온 인풋이 들어왔을 때때
        if (lockOnInput && player.playerNetworkManager.isLockedOn.Value)
        {
            lockOnInput = false;
            CameraController.instance.ClearLockOnTargets();
            if (player.characterClass == PlayerClass.Archer)
            {
                player.playerCombatManager.SetAimMode();
                player.animator.SetTrigger("AimCancel");
            }
            else
            {
                player.playerCombatManager.SetTarget(null);
            }

            player.playerNetworkManager.isLockedOn.Value = false;
            return;
        }

    }

    // 아군 조준
    private void HandleLockOnAllyInput()
    {
        if (player.isDead.Value)
        {
            lockOnAllyInput = false;
            return;
        }
        // 맨 처음 락온 할 때
        if (lockOnAllyInput)
        {
            lockOnAllyInput = false;
            if (!player.playerNetworkManager.isLockedOn.Value)
            {

                CameraController.instance.SetTargetType(CharacterType.Player);
                CameraController.instance.ClearLockOnTargets();
                if (CameraController.instance.FindLockOnTargets())
                {
                    player.playerCombatManager.SetTarget(CameraController.instance.nearestLockOnTarget);
                    player.playerNetworkManager.isLockedOn.Value = true;
                }
                CameraController.instance.SetTargetType(CharacterType.Monster);
            }
            else
            {
                CameraController.instance.ClearLockOnTargets();
                CameraController.instance.SetTargetType(CharacterType.Monster);
                player.playerCombatManager.SetTarget(null);
                player.playerNetworkManager.isLockedOn.Value = false;
                return;

            }
        }

    }

    private void UpdateLockOn()
    {
        if (player.isDead.Value)
        {
            return;
        }

        if (player.CharacterClass == PlayerClass.Archer) return;

        // 락온 모드가 켜져있을 때
        if (player.playerNetworkManager.isLockedOn.Value)
        {
            // 락온 대상과의 거리에 따라 카메라의 높이를 동적으로 조절한다. 
            CameraController.instance.UpdateCameraHeight(Vector3.Distance(player.playerCombatManager.currentTarget.transform.position, player.transform.position));

            // 상대방이 죽으면 락온을 해제해야 한다.
            if (player.playerCombatManager.currentTarget.isDead.Value)
            {
                if (autoRelockOnWhenTargetDead)
                {
                    // if (lockOnCoroutine != null)
                    //     StopCoroutine(lockOnCoroutine);

                    // lockOnCoroutine = StartCoroutine(PlayerCamera.instance.WaitThenFindNewTarget());

                    // PlayerCamera.instance.ClearLockOnTargets();
                    // PlayerCamera.instance.FindLockOnTargets();

                    CameraController.instance.ClearLockOnTargets();
                    CameraController.instance.FindLockOnTargets();

                    // 락온 대상이 있다면
                    if (CameraController.instance.nearestLockOnTarget != null)
                    {
                        // 타겟 설정
                        player.playerCombatManager.SetTarget(CameraController.instance.nearestLockOnTarget);
                        player.playerNetworkManager.isLockedOn.Value = true;
                    }
                    else // 락온 대상이 없다면 락온 해제.
                    {
                        player.playerCombatManager.SetTarget(null);
                        player.playerNetworkManager.isLockedOn.Value = false;
                    }
                }
                else
                {
                    CameraController.instance.ClearLockOnTargets();
                    player.playerCombatManager.SetTarget(null);
                    player.playerNetworkManager.isLockedOn.Value = false;
                }
            }
            // 락온 중임에도 재락온 하는 이유는 이동으로 인해 최대 락온 사정거리를 벗어날 수 있기 때문.
        }

    }

    private void HandleLockOnSwitchTargetInput()
    {
        if (player.isDead.Value)
        {
            lockOnLeftInput = false;
            return;
        }
        if (lockOnLeftInput)
        {
            lockOnLeftInput = false;

            if (player.playerNetworkManager.isLockedOn.Value)
            {
                //         PlayerCamera.instance.FindLockOnTargets();

                //         if (PlayerCamera.instance.leftLockOnTarget != null)
                //         {
                //             player.playerCombatManager.SetTarget(PlayerCamera.instance.leftLockOnTarget);
                //         }
                //     }
            }

            // if (lockOnRightInput)
            // {
            //     lockOnRightInput = false;

            //     if (player.playerNetworkManager.isLockedOn.Value)
            //     {
            //         PlayerCamera.instance.FindLockOnTargets();

            //         if (PlayerCamera.instance.rightLockOnTarget != null)
            //         {
            //             player.playerCombatManager.SetTarget(PlayerCamera.instance.rightLockOnTarget);
        }
    }

    private void HandleSwtichRightWeaponInput()
    {
        if (player.isDead.Value)
        {
            switchRightWeaponInput = false;
            return;
        }
        if (switchRightWeaponInput)
        {
            switchRightWeaponInput = false;
            player.playerEquipmentManager.SwitchRightWeapon();
        }
    }
    private void HandleSwtichLeftWeaponInput()
    {
        if (player.isDead.Value)
        {
            switchLeftWeaponInput = false;
            return;
        }
        if (switchLeftWeaponInput)
        {
            switchLeftWeaponInput = false;
            player.playerEquipmentManager.SwitchLeftWeapon();
        }
    }


    private void OnApplicationFocus(bool hasFocus)
    {
        if (enabled)
        {
            // if (hasFocus)
            // {
            //     Cursor.lockState = CursorLockMode.Locked;
            //     Cursor.visible = false;
            // }
            // else
            // {
            //     Cursor.lockState = CursorLockMode.None;
            //     Cursor.visible = true;
            // }
        }
    }

    private void InputHoldCheck()
    {
        if (hold == true)
        {
            HoldTime += Time.deltaTime;
        }
    }

    public void BlockLInput()
    {
        // 추가적인 입력을 막음
        playerInputLimit = true;

        // 이미 들어와 있는 벡터 값들을 천천히 0으로 감소
        StartCoroutine(SmoothInputReduction());
    }

    public void UnBlockInput()
    {
        // 추가적인 입력을 허용
        playerInputLimit = false;
    }

    private IEnumerator SmoothInputReduction()
    {
        float duration = 1f; // 1초 동안 감소
        float elapsedTime = 0f;

        Vector2 initialMovementInput = movementInput;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Lerp를 사용하여 movementInput과 cameraInput을 0으로 감소
            movementInput = Vector2.Lerp(initialMovementInput, Vector2.zero, elapsedTime / duration);

            // Update 관련 변수들
            verticalInput = Mathf.Lerp(verticalInput, 0, elapsedTime / duration);
            horizontalInput = Mathf.Lerp(horizontalInput, 0, elapsedTime / duration);
            moveAmount = Mathf.Lerp(moveAmount, 0, elapsedTime / duration);

            yield return null;
        }

        // 최종적으로 값을 0으로 설정
        movementInput = Vector2.zero;
        verticalInput = 0f;
        horizontalInput = 0f;
        moveAmount = 0f;
    }

    public void BlockForSeconds(InputType inputType, float seconds)
    {
        HUD_UIManager.instance.combatUIManager.CoolDownProcess(inputType, seconds);
        switch (inputType)
        {
            case InputType.Shift:
                StartCoroutine(BlockShiftForSeconds(seconds));
                break;
            case InputType.Q:
                StartCoroutine(BlockQForSeconds(seconds));
                break;
            case InputType.R:
                StartCoroutine(BlockRForSeconds(seconds));
                break;
            case InputType.Space:
                StartCoroutine(BlockSpacebarForSeconds(seconds));
                break;
            case InputType.RightClick:
                // 패링 성공 시 초기화해줘야함.
                parryingInputCoroutine = StartCoroutine(BlockRightClickForSeconds(seconds));
                break;
            default:
                break;
        }

    }

    private IEnumerator BlockShiftForSeconds(float seconds)
    {
        shiftInputBlock = true;
        yield return new WaitForSeconds(seconds);
        shiftInputBlock = false;
    }
    private IEnumerator BlockQForSeconds(float seconds)
    {
        qInputBlock = true;
        yield return new WaitForSeconds(seconds);
        qInputBlock = false;
    }
    private IEnumerator BlockRForSeconds(float seconds)
    {
        rInputBlock = true;
        yield return new WaitForSeconds(seconds);
        rInputBlock = false;
    }
    private IEnumerator BlockSpacebarForSeconds(float seconds)
    {
        spacebarBlock = true;
        yield return new WaitForSeconds(seconds);
        spacebarBlock = false;
    }
    private IEnumerator BlockRightClickForSeconds(float seconds)
    {
        rightClickBlock = true;
        yield return new WaitForSeconds(seconds);
        rightClickBlock = false;
    }

    public void UnBlockRightClick()
    {
        rightClickBlock = false;
        StopCoroutine(parryingInputCoroutine);
        Debug.Log("여기에서 1차로 쿨타임 초기화");
        HUD_UIManager.instance.combatUIManager.CompleteCoolDownRightClick();
    }

    public void UnBlockSpacebar()
    {
        spacebarBlock = false;
        Debug.Log("여기에서 1차로 쿨타임 초기화");
        HUD_UIManager.instance.combatUIManager.CompleteCoolDownRightClick();
    }

}
