using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Rendering;

public class PlayerCombatManager : CharacterCombatManager
{
    PlayerManager player;
    public Weapon currentWeaponBeingUsed;
    public Transform parryingEffectTransform;

    [Header("Flags")]
    public bool canComboWithMainHandWeapon = false;
    public bool animationEventLock = false;

    public float stiffTimeTest;

    public bool autoAttackArrange = false;
    [Header("봉인")]
    [SerializeField] private int hitCountToRelase = 10;
    [SerializeField] private GameObject sealVFXPrefab;
    private GameObject sealVFX;

    private WeaponTrailController weaponTrailController;

    [HideInInspector] public WeaponAction currentWeaponAction;

    public int HitCountToRelase { get => hitCountToRelase; set => hitCountToRelase = value; }

    [Header("피격 무적 설정")]
    // 데미지도 안 받는 무적
    private int invulnerableCount = 0;
    public int InvulnerableCount
    {
        get => invulnerableCount;
        set => ProcessInvulnerableCount(value);
    }
    // 데미지는 받는 무적
    // private int superArmorCount = 0;
    // public int SuperArmorCount
    // {
    //     get => superArmorCount;
    //     set => superArmorCount = value;
    // }
    // public float AttackIncreaseRate { get => attackIncreaseRate; set => attackIncreaseRate = value; }
    // public float DamageDecreaseRate { get => damageDecreaseRate; set => damageDecreaseRate = value; }

    // [Header("강화 버프 설정")]
    // [SerializeField] private float attackIncreaseRate = 0f; // 공격력 증가율율
    // [SerializeField] private float damageDecreaseRate = 0f; // 방어력 증가율
    // [SerializeField] private float enchanctTime = 0; // 버프 지속시간

    private void ProcessInvulnerableCount(int value)
    {
        invulnerableCount = value;
        if (invulnerableCount == 0)
        {
            player.ultimateAttackAction.RemoveShieldServerRpc();
        }
    }

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();

        weaponTrailController = GetComponent<WeaponTrailController>();

    }


    // 버그 : 락온 상태에서 봉인 됐을 때 이상함. 카메라가 고정 됨. 

    // 버그 : 죽었을 때 입력값이 남아있음. 남아있지 않게 했는데도. 그래서 자꾸 인풋매니저에 남아 있음. 


    public void PerformWeaponBasedAction(WeaponAction action, Weapon weapon)
    {
        if (player.IsOwner)
        {
            // 로컬에서의 캐릭터가 무기 애니메이션을 시작함.
            action.AttempToPerformAction(player, weapon);

            // 다른 클라이언트의 내 캐릭터가 무기 애니메이션을 실행하게끔 RPC 전파함.
            player.playerNetworkManager.NotifyTheServerOfWeaponActionServerRpc(OwnerClientId, action.actionID, weapon.itemID);
        }
    }

    // 이 함수를 굳이 애니메이션 이벤트로 걸지 않고, 애니메이션 플레이 함수에 걸어주자.  
    public virtual void DrainStaminaBasedOnAttack()
    {
        if (!player.IsOwner) return;

        if (currentWeaponBeingUsed == null) return;

        float staminaDeducted = 0;

        switch (currentAttackType)
        {
            // TODO : 추후에 다른 기준을 세워야 함.
            // case AttackType.BackstepAttack01:
            //     staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.backstepAttackStaminaCostMultiplier;
            //     break;
            default:
                break;

        }

        player.playerNetworkManager.currentStamina.Value -= Mathf.RoundToInt(staminaDeducted);
    }

    public override void SetTarget(CharacterManager newTarget)
    {
        base.SetTarget(newTarget);

        if (player.IsOwner)
        {
            if (newTarget != null)
            {
                // 캐릭터를 타겟쪽으로 순간 회전 시켜줄까? 
                // player.playerLocomotionManager.PlayerRotationToWardsInputDirection(currentTarget.transform, 0f);
                CameraController.instance.LockOn(true);
            }
            else
                CameraController.instance.LockOn(false);

            // PlayerCamera.instance.SetLockCameraHeight();
        }
    }

    public void SetAimMode()
    {
        if (player.IsOwner)
        {
            CameraController.instance.AimOn();
        }
    }


    // 애니메이션에 삽입되어 있음.
    public override void EnableCanDoCombo()
    {
        if (player.playerNetworkManager.isUsingRightHand.Value)
        {
            player.playerCombatManager.canComboWithMainHandWeapon = true;
        }
    }
    public void EnableCanDoComboWithLock()
    {
        if (animationEventLock) return;
        if (player.playerNetworkManager.isUsingRightHand.Value)
        {
            player.playerCombatManager.canComboWithMainHandWeapon = true;
        }
    }

    public override void DisableCanDoCombo()
    {
        player.playerCombatManager.canComboWithMainHandWeapon = false;
    }

    public override void EnableCanDoEvade()
    {
        player.playerCombatManager.canDoEvade = true;
    }

    public override void DisableCanDoEvade()
    {
        player.playerCombatManager.canDoEvade = false;
    }

    public override void EnableCanDoDoubleDash()
    {
        player.playerCombatManager.canDoDoubleDash = true;
    }

    public override void DisableCanDoDoubleDash()
    {
        player.playerCombatManager.canDoDoubleDash = false;
    }

    public override void InitializeLastPlayedAnimation()
    {
        lastAttackAnimationPerformed = "";
    }


    public void OpenDamageCollider()
    {

        player.playerEquipmentManager.RightWeaponManager?.damageCollider.EnableDamageCollider();

        if (weaponTrailController != null) weaponTrailController.EnableTrail();
    }

    public void CloseDamageCollider()
    {
        player.playerEquipmentManager.RightWeaponManager?.damageCollider.DisableDamageCollider();

        if (weaponTrailController != null) weaponTrailController.DisableTrail();
    }
    Vector3 ScreenToPoint;
    Vector3 playPos;
    bool cast;

    protected override void Update()
    {
        base.Update();
        // HitTrace();
    }

    public void HitTrace()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2));
        Vector3 ScreenToRayPoint = ray.GetPoint(20f);
        playPos = player.playerEquipmentManager.LeftWeaponManager.transform.position;
        RaycastHit[] hitInfo = Physics.RaycastAll(playPos, ScreenToRayPoint - playPos, 20f, 7); // damagable

        foreach (var hit in hitInfo)
        {
            if (hit.collider == null)
            {

                continue;

            }
            else
            {
                if (hit.collider.gameObject.layer == LayerMask.GetMask("DamagableCollider")) ;
                {
                    // UI Activate.
                }


            }

        }


    }

    public void OpenHitscanCaster()
    {
        if (!IsOwner) return;
        Vector3 targetDirection = Vector3.zero;
        targetDirection = Camera.main.transform.forward;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
            targetDirection = transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2));
        ScreenToPoint = ray.GetPoint(20f);
        HitScanServerRpc(ScreenToPoint, targetRotation);


    }

    [ServerRpc]
    public void HitScanServerRpc(Vector3 Direction, Quaternion Rot)
    {
        HitScanClientRpc(Direction, Rot);
    }

    [ClientRpc]
    public void HitScanClientRpc(Vector3 Direction, Quaternion Rot)
    {
        player.transform.rotation = Rot;
        // Trace와는 별개로 Ray를 쏜다.

        Vector3 playPos = player.playerEquipmentManager.LeftWeaponManager.transform.position;
        RaycastHit[] hitInfo = Physics.RaycastAll(playPos, Direction - playPos, 20f); // damagable

        ShotVFX shotProjectile = Instantiate(GetComponent<MultipleShotSystem>().shotVFX);
        shotProjectile.transform.position = playPos;
        shotProjectile.ArrowSpeed = 100f;
        shotProjectile.transform.rotation = Quaternion.LookRotation(Direction - playPos);


        foreach (var hit in hitInfo)
        {
            Debug.Log(hit.collider.gameObject.name);
            Debug.Log(hit.collider.gameObject.layer);
            if (hit.collider == null)
            {

                continue;

            }
            else
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Damagable Character"))
                {
                    AICharacterManager monster = hit.collider.gameObject.GetComponentInParent<AICharacterManager>();
                    if (monster != null) player.playerEquipmentManager.LeftWeaponManager?.damageCollider.RayTrigger(monster);
                }


            }

        }

        Debug.DrawRay(playPos, ScreenToPoint - playPos, Color.blue, 3f);
        cast = true;
    }

    public void CloseHitscanCaster()
    {
        player.playerEquipmentManager.LeftWeaponManager?.damageCollider.DisableDamageCollider();
        cast = false;
    }

    public void OpenOffDamageCollider()
    {
        player.playerEquipmentManager.LeftWeaponManager?.damageCollider.EnableDamageCollider();

        if (weaponTrailController != null) weaponTrailController.EnableOffTrail();
    }

    public void CloseOffDamageCollider()
    {
        player.playerEquipmentManager.LeftWeaponManager?.damageCollider.DisableDamageCollider();

        if (weaponTrailController != null) weaponTrailController.DisableOffTrail();
    }

    // 해당 함수는 애니메이션 이벤트 내에서 데미지를 설정할 때 사용 됨.
    public void SetDamageOnAnimation(int index)
    {
        int damage = currentWeaponAction.damagePairs[index].damage;
        int groggyDamage = currentWeaponAction.damagePairs[index].groggyDamage;
        SetDamageOnDamageCollider(damage, groggyDamage);
    }

    // 해당 함수는 애니메이터에서 단타 공격의 데미지를 설정할 때 사용 됨.
    public void SetDamageOnDamageCollider(float damage, float groggyDamage)
    {
        player.playerEquipmentManager.RightWeaponManager?.SetWeaponDamage(player, player.playerInventoryManager.currentRightHandWeapon, damage, groggyDamage);
        player.playerEquipmentManager.LeftWeaponManager?.SetWeaponDamage(player, player.playerInventoryManager.currentLeftHandWeapon, damage, groggyDamage);
        player.shockwave?.SetShockWaveDamage(damage, groggyDamage);
    }

    public override void CloseAllDamageCollider()
    {
        CloseOffDamageCollider();
        CloseDamageCollider();

        if (weaponTrailController == null) return;

        weaponTrailController.DisableTrail();
        weaponTrailController.DisableOffTrail();
    }

    public override void OpenAllDamageCollider()
    {
        OpenDamageCollider();
        OpenOffDamageCollider();

        if (weaponTrailController == null) return;

        weaponTrailController.EnableTrail();
        weaponTrailController.EnableOffTrail();
    }

    public void GetStiff(float stopDuration)
    {
        StartCoroutine(HitStiffness(stopDuration));
    }

    public IEnumerator HitStiffness(float stopDuration)
    {
        character.animator.speed = 0;
        yield return new WaitForSeconds(stopDuration);
        character.animator.speed = 1;
    }

    public override void EnableTrailRenderer()
    {
        base.EnableTrailRenderer();

    }

    public override void DisableTrailRenderer()
    {
        base.DisableTrailRenderer();

    }

    //  네트워크 변수 isSealed true 처리 -> 봉인 시 특정 함수 실행 -> 봉인에 필요한 타격횟수 설정, 플레이어 입력 제한, 사슬 VFX 생성,캐릭터 그룹을 monster로 변경
    // damage collider에서 몬스터를 때릴 때 TakeSealHitEffect로 처리하게끔 수정
    // 봉인이 풀릴 때까지 남은 타격 횟수가 0이 되는 순간 봉인 해제 
    // isSealed false처리 -> 플레이어 입력 제한 해제, 사슬 VFX 제거, 캐릭터 그룹을 player로 변경, 
    [ServerRpc(RequireOwnership = false)]
    public void GetSealedServerRpc(string message)
    {
        // int players = NetworkManager.Singleton.ConnectedClientsList.Count;
        GetSealedClientRpc(message);
    }

    [ClientRpc]
    private void GetSealedClientRpc(string message, int neededHitCount = 1)
    {
        if (IsOwner)
        {
            Debug.Log("이 플레이어는 봉인 되었습니다.");
            HUD_UIManager.instance.popUpUIManager.SendRpcMessagePopUp(message);
            // 플레이어가 검기에 의해서만 타격을 입게끔 처리를 해주는 역할. 따로 델리게이트는 걸려있지 않다. 
            player.playerNetworkManager.isPartiallyInvulnerable.Value = true;


            player.playerCombatManager.SetHitCountToRelease(neededHitCount);
        }

    }

    public void SetHitCountToRelease(int neededHitCount)
    {
        HitCountToRelase = neededHitCount;
        // switch (players)
        // {
        //     case 1:
        //         Debug.LogError("플레이어가 1명인데 봉인 프로세스가 진행될 리 없음.");
        //         // HitCountToRelase = ;
        //         break;
        //     case 2:
        //         HitCountToRelase = 13;
        //         break;
        //     case 3:
        //         HitCountToRelase = 20;
        //         break;
        //     case 4:
        //         HitCountToRelase = 30;
        //         break;
        // }
    }

    public void GenerateSealVFX()
    {
        // sealVFX = ObjectPoolManager.Singleton.GetObject(sealVFXPrefab, player.transform.position + Vector3.up, Quaternion.Euler(110, 0, 0));
        sealVFX = ObjectPoolManager.Singleton.GetObject(ObjectPoolType.Seal, 0);
        sealVFX.transform.position = player.transform.position + Vector3.up;
        sealVFX.transform.SetParent(player.transform);
        sealVFX.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
    }

    public void DegenerateSealVFX()
    {
        if (sealVFX != null)
        {
            GameObject sealPrefab = ObjectPoolManager.Singleton.GetPrefab(ObjectPoolType.Seal, 0);
            ObjectPoolManager.Singleton.ReturnObject(sealVFX, sealPrefab);
        }
    }

    public void SetInvulnerableCount(int count)
    {
        InvulnerableCount = count;
    }



}
