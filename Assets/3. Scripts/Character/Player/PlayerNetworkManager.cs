using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class PlayerNetworkManager : CharacterNetworkManager
{
    private PlayerManager player;
    public NetworkVariable<FixedString64Bytes> characterName = new NetworkVariable<FixedString64Bytes>("Character", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Equipment")]
    public NetworkVariable<int> currentWeaponBeingUsed = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> currentRightHandWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> currentLeftHandWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isUsingRightHand = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isUsingLeftHand = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isInteracting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isSealed = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    /// <summary>
    /// true면 오른손, false면 왼손을 사용하는 걸로 설정한다.
    /// </summary>
    /// <param name="rightHandedAction"></param>
    public void SetCharacterActionHand(bool rightHandedAction)
    {
        if (rightHandedAction)
        {
            isUsingLeftHand.Value = false;
            isUsingRightHand.Value = true;
        }
        else
        {
            isUsingRightHand.Value = false;
            isUsingLeftHand.Value = true;
        }
    }
    //  네트워크 변수 isSealed true 처리 -> 봉인 시 특정 함수 실행 -> 봉인에 필요한 타격횟수 설정, 플레이어 입력 제한, 사슬 VFX 생성,캐릭터 그룹을 monster로 변경

    [ServerRpc]
    public void SetIsSealedServerRpc(bool condition)
    {
        if (IsServer)
        {
            this.isSealed.Value = condition;
        }
    }
    public void OnIsSealedChanged(bool previousValue, bool newValue)
    {
        if (newValue == true)
        {
            if (IsOwner)
            {
                // 플레이어의 입력을 막았는데? 아직 남아있던 인풋값이 적용이 되서 달리는 듯? 
                // 그럼 인풋값도 아예 막아주자. 
                player.playerLocomotionManager.MakePlayerUncontrollerable();
                // // 봉인 되었을 땐 락온을 풀어준다.
                // CameraController.instance.ClearLockOnTargets();
                // player.playerCombatManager.SetTarget(null);
                // player.playerNetworkManager.isLockedOn.Value = false;
            }
            // 플레이어의 입력 제한, 움직임 제한, 애니메이션 제한을 걸어준다.
            player.characterGroup = CharacterType.Monster;

            player.playerCombatManager.GenerateSealVFX();
        }
        else if (newValue == false)
        {
            if (IsOwner)
                player.playerLocomotionManager.MakePlayerControllerable();
            player.characterGroup = CharacterType.Player;
            player.playerCombatManager.DegenerateSealVFX();
            // player.animator.SetTrigger("Unsealed");
        }
    }

    public void SetNewMaxHealthValueOnUIBar(int oldValue, int newValue)
    {
        HUD_UIManager.instance.combatUIManager.SetMaxHealthValue(maxHp.Value);
    }

    public void SetNewMaxStaminaValueOnUIBar(int oldValue, int newValue)
    {
        HUD_UIManager.instance.combatUIManager.SetMaxStaminaValue(maxHp.Value);
    }


    public void OnCurrentLeftHandWeaponIDChange(int oldID, int newID)
    {
        Weapon newWeapon = WorldItemDatabase.instance.GetWeaponByID(newID);
        player.playerInventoryManager.currentLeftHandWeapon = newWeapon;
        player.playerEquipmentManager.LoadLeftWeapon();
        player.playerEquipmentManager.LoadRightWeapon();

        if (player.IsOwner)
        {
            HUD_UIManager.instance.combatUIManager.SetLeftWeaponQuickSlotIcon(newID);
        }
    }

    public void OnCurrentRightHandWeaponIDChange(int oldID, int newID)
    {
        Weapon newWeapon = WorldItemDatabase.instance.GetWeaponByID(newID);
        player.playerInventoryManager.currentRightHandWeapon = newWeapon;
        player.playerEquipmentManager.LoadRightWeapon();
        player.playerEquipmentManager.LoadRightWeapon();

        if (player.IsOwner)
        {
            HUD_UIManager.instance.combatUIManager.SetRightWeaponQuickSlotIcon(newID);
        }
    }

    // 여기서 의문. 한 손의 무기만을 바꿀 때 어떻게 되는거지?
    public void OnCurrentWeaponBeingUsedIDChange(int oldID, int newID)
    {
        Weapon newWeapon = WorldItemDatabase.instance.GetWeaponByID(newID);
        player.playerCombatManager.currentWeaponBeingUsed = newWeapon;

        if (player.IsOwner) return;

        // NOTE : 이 부분에서 현재 사용중인 무기에 딸린 애니메이터 오버라이드 컨트롤러로 애니메이터 컨트롤러를 변경함. 
        if (player.playerCombatManager.currentWeaponBeingUsed != null)
        {
            // player.playerAnimatorManager.UpdateAnimatorController(player.playerCombatManager.currentWeaponBeingUsed.weaponAnimator);
        }
    }

    public override void OnIsAttackingChanged(bool oldStatus, bool newStatus)
    {
        base.OnIsAttackingChanged(oldStatus, newStatus);
    }

    public override void OnIsGaurdingChanged(bool oldStatus, bool newStatus)
    {
        if (newStatus == false)
        {
            player.animator.SetTrigger("TransitionStart");
            Debug.Log("난가?");
            // player.animator.SetTrigger("CancelGaurd");
        }
    }

    // 처음에 performaction을 통해서 가드 올리는 애니메이션을 수행
    // 자동으로 애니메이터 전이조건에 따라서 가드를 올리고 있는 상태까지 수행
    // 모든 네트워크상에 animator 트리거를 발동시켜서 다음 단계로 넘어가게끔 조치를 취해줌. 

    // 아이템 액션
    [ServerRpc]
    public void NotifyTheServerOfWeaponActionServerRpc(ulong clientID, int actionID, int weaponID)
    {
        if (IsServer)
        {
            NotifyTheServerOfWeaponActionClientRpc(clientID, actionID, weaponID);
        }
    }

    [ClientRpc]
    public void NotifyTheServerOfWeaponActionClientRpc(ulong clientID, int actionID, int weaponID)
    {
        if (clientID != NetworkManager.Singleton.LocalClientId)
        {
            PerformWeaponBasedAction(actionID, weaponID);
        }
    }

    private void PerformWeaponBasedAction(int actionID, int weaponID)
    {
        WeaponAction weaponAction = WorldActionManager.instance.GetWeaponItemActionByID(actionID);

        if (weaponAction != null)
        {
            weaponAction.AttempToPerformAction(player, WorldItemDatabase.instance.GetWeaponByID(weaponID));
        }
        else
        {
            Debug.LogError("Action in null, cannot be performed.");
        }
    }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();


    }

    public void OnIsInteractingChanged(bool previousValue, bool newValue)
    {
        if (newValue == false)
        {
            // 무기 활성화
            player.playerEquipmentManager.EnableWeapon();
            player.animator.SetTrigger("TransitionStart");
        }
        else if (newValue == true)
        {
            if (player.IsOwner)
            {
                player.playerInteractionManager.PlayInteractionAnimation();
            }
            // 무기 비활성화 
            player.playerEquipmentManager.DisableWeapon();

        }
    }
}
