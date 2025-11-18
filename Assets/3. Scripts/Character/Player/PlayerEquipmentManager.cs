using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerEquipmentManager : CharacterEquipmentManager
{
    PlayerManager player;
    public WeaponModelInstantiationSlot rightHandSlot;
    public WeaponModelInstantiationSlot leftHandWeaponSlot;
    public WeaponModelInstantiationSlot leftHandShieldSlot;

    [SerializeField] WeaponManager rightWeaponManager;
    [SerializeField] WeaponManager leftWeaponManager;

    public GameObject rightHandWeaponModel;

    public GameObject leftHandWeaponModel;

    public WeaponManager RightWeaponManager { get => rightWeaponManager; private set => rightWeaponManager = value; }
    public WeaponManager LeftWeaponManager { get => leftWeaponManager; private set => leftWeaponManager = value; }

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();

        // InitializeWeaponSlots();
        LoadWeaponsOnBothHands();
    }

    protected override void Start()
    {
        base.Start();

    }

    private void InitializeWeaponSlots()
    {
        WeaponModelInstantiationSlot[] weaponSlots = GetComponentsInChildren<WeaponModelInstantiationSlot>();

        foreach (var weaponSlot in weaponSlots)
        {
            if (weaponSlot.weaponSlot == WeaponModelSlot.RightHandWeaponSlot)
            {
                rightHandSlot = weaponSlot;
            }
            else if (weaponSlot.weaponSlot == WeaponModelSlot.LeftHandWeaponSlot)
            {
                leftHandWeaponSlot = weaponSlot;
            }
            else if (weaponSlot.weaponSlot == WeaponModelSlot.LeftHandShieldSlot)
            {
                leftHandShieldSlot = weaponSlot;
            }
        }
    }

    public void SwitchRightWeapon()
    {
        // if (!player.IsOwner) return;

        // player.playerAnimatorManager.PlayTargetActionAnimation("Swap_Right_Weapon_01", false, false, true, true);

        // WeaponItem selectedWeapon = null;

        // player.playerInventoryManager.rightHandWeaponIndex += 1;

        // if (player.playerInventoryManager.rightHandWeaponIndex < 0 || player.playerInventoryManager.rightHandWeaponIndex > 2)
        // {
        //     player.playerInventoryManager.rightHandWeaponIndex = 0;

        //     float weaponCount = 0;
        //     WeaponItem firstWeapon = null;
        //     int firstWeaponPosition = 0;

        //     for (int i = 0; i < player.playerInventoryManager.weaponsInRightHandSlots.Length; i++)
        //     {
        //         if (player.playerInventoryManager.weaponsInRightHandSlots[i].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
        //         {
        //             weaponCount += 1;
        //             if (firstWeapon == null)
        //             {
        //                 firstWeapon = player.playerInventoryManager.weaponsInRightHandSlots[i];
        //                 firstWeaponPosition = i;
        //             }
        //         }
        //     }

        //     if (weaponCount <= 1)
        //     {
        //         player.playerInventoryManager.rightHandWeaponIndex = -1;
        //         selectedWeapon = WorldItemDatabase.instance.unarmedWeapon;
        //         player.playerNetworkManager.currentRightHandWeaponID.Value = selectedWeapon.itemID;
        //     }
        //     else
        //     {
        //         player.playerInventoryManager.rightHandWeaponIndex = firstWeaponPosition;
        //         player.playerNetworkManager.currentRightHandWeaponID.Value = firstWeapon.itemID;
        //     }

        //     return;
        // }

        // foreach (WeaponItem weapon in player.playerInventoryManager.weaponsInRightHandSlots)
        // {
        //     if (player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
        //     {
        //         selectedWeapon = player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex];

        //         player.playerNetworkManager.currentRightHandWeaponID.Value = player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex].itemID;
        //         return;
        //     }
        // }

        // if (selectedWeapon == null && player.playerInventoryManager.rightHandWeaponIndex <= 2)
        // {
        //     SwitchRightWeapon();
        // }
    }
    public void SwitchLeftWeapon()
    {
        // if (!player.IsOwner) return;

        // player.playerAnimatorManager.PlayTargetActionAnimation("Swap_Left_Weapon_01", false, false, true, true);

        // WeaponItem selectedWeapon = null;

        // player.playerInventoryManager.leftHandWeaponIndex += 1;

        // if (player.playerInventoryManager.leftHandWeaponIndex < 0 || player.playerInventoryManager.leftHandWeaponIndex > 2)
        // {
        //     player.playerInventoryManager.leftHandWeaponIndex = 0;

        //     float weaponCount = 0;
        //     WeaponItem firstWeapon = null;
        //     int firstWeaponPosition = 0;

        //     for (int i = 0; i < player.playerInventoryManager.weaponsInLeftHandSlots.Length; i++)
        //     {
        //         if (player.playerInventoryManager.weaponsInLeftHandSlots[i].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
        //         {
        //             weaponCount += 1;
        //             if (firstWeapon == null)
        //             {
        //                 firstWeapon = player.playerInventoryManager.weaponsInLeftHandSlots[i];
        //                 firstWeaponPosition = i;
        //             }
        //         }
        //     }

        //     if (weaponCount <= 1)
        //     {
        //         player.playerInventoryManager.leftHandWeaponIndex = -1;
        //         selectedWeapon = WorldItemDatabase.instance.unarmedWeapon;
        //         player.playerNetworkManager.currentLeftHandWeaponID.Value = selectedWeapon.itemID;
        //     }
        //     else
        //     {
        //         player.playerInventoryManager.leftHandWeaponIndex = firstWeaponPosition;
        //         player.playerNetworkManager.currentLeftHandWeaponID.Value = firstWeapon.itemID;
        //     }

        //     return;
        // }

        // foreach (WeaponItem weapon in player.playerInventoryManager.weaponsInLeftHandSlots)
        // {
        //     if (player.playerInventoryManager.weaponsInLeftHandSlots[player.playerInventoryManager.leftHandWeaponIndex].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
        //     {
        //         selectedWeapon = player.playerInventoryManager.weaponsInLeftHandSlots[player.playerInventoryManager.leftHandWeaponIndex];

        //         player.playerNetworkManager.currentLeftHandWeaponID.Value = player.playerInventoryManager.weaponsInLeftHandSlots[player.playerInventoryManager.leftHandWeaponIndex].itemID;
        //         return;
        //     }
        // }

        // if (selectedWeapon == null && player.playerInventoryManager.leftHandWeaponIndex <= 2)
        // {
        //     SwitchLeftWeapon();
        // }
    }

    public void LoadWeaponsOnBothHands()
    {
        LoadRightWeapon();
        LoadLeftWeapon();
    }

    public void LoadRightWeapon()
    {
        if (player.playerInventoryManager.currentRightHandWeapon != null)
        {
            if (rightHandSlot != null) rightHandSlot.UnLoadWeapon();

            rightHandWeaponModel = Instantiate(player.playerInventoryManager.currentRightHandWeapon.weaponModel);
            rightHandSlot.LoadWeapon(rightHandWeaponModel);
            RightWeaponManager = rightHandWeaponModel.GetComponent<WeaponManager>();
            RightWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentRightHandWeapon, 0, 0);

            if (RightWeaponManager.weaponTrail != null)
                GetComponent<WeaponTrailController>().RightParticle = RightWeaponManager.weaponTrail.GetComponent<ParticleController>();

            // player.playerAnimatorManager.UpdateAnimatorController(player.playerInventoryManager.currentRightHandWeapon.weaponAnimator);
        }
    }

    public void LoadLeftWeapon()
    {
        if (player.playerInventoryManager.currentLeftHandWeapon != null)
        {
            if (leftHandWeaponSlot != null) leftHandWeaponSlot.UnLoadWeapon();

            if (leftHandShieldSlot != null) leftHandShieldSlot.UnLoadWeapon();


            leftHandWeaponModel = Instantiate(player.playerInventoryManager.currentLeftHandWeapon.weaponModel);

            switch (player.playerInventoryManager.currentLeftHandWeapon.weaponModelType)
            {
                case WeaponModelType.Weapon:
                    leftHandWeaponSlot.LoadWeapon(leftHandWeaponModel);
                    break;
                case WeaponModelType.Shield:
                    leftHandShieldSlot.LoadWeapon(leftHandWeaponModel);
                    break;
                default:
                    break;

            }
            LeftWeaponManager = leftHandWeaponModel.GetComponent<WeaponManager>();
            LeftWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentLeftHandWeapon, 0, 0);

            if (leftWeaponManager.weaponTrail != null)
                GetComponent<WeaponTrailController>().LeftParticle = LeftWeaponManager.weaponTrail.GetComponent<ParticleController>();

        }
    }

    public void EnableWeapon()
    {
        Debug.Log("무기 모델링 활성화");
        if (leftHandWeaponModel != null) leftHandWeaponModel.SetActive(true);
        if (rightHandWeaponModel != null) rightHandWeaponModel.SetActive(true);
    }

    public void DisableWeapon()
    {
        Debug.Log("무기 모델링 비활성화");
        if (leftHandWeaponModel != null) leftHandWeaponModel.SetActive(false);
        if (rightHandWeaponModel != null) rightHandWeaponModel.SetActive(false);
        // leftHandWeaponModel?.SetActive(false);
        // rightHandWeaponModel?.SetActive(false);
    }

}
