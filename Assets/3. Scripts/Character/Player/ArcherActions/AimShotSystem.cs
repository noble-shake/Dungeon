using Cysharp.Threading.Tasks;
using System;
using Unity.Netcode;
using UnityEngine;

public class AimShotSystem : NetworkBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] int AimChance;
    [SerializeField] Canvas AimUI;

    [SerializeField] DamageCollider damageCollider;
    [SerializeField] GameObject kickVFX;
    [SerializeField] float vfxDistance = 1f;

    // 애니메이션 이벤트로 걸어주는 함수
    // 충격파 이펙트 + 0.2초간 데미지콜라이더 활성화 
    public void KickCounter()
    {
        WaitAndDisableDamageCollider(0.2f).Forget();
    }

    public async UniTaskVoid WaitAndDisableDamageCollider(float duration)
    {
        damageCollider.EnableDamageCollider();
        kickVFX.gameObject.SetActive(true);
        await UniTask.WaitForSeconds(duration);
        damageCollider.DisableDamageCollider();
        await UniTask.WaitForSeconds(duration);
        kickVFX.gameObject.SetActive(false);
    }

    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        //Instantiate(AimUI);
    }

    //HandleLeftLongInputCanceled

    public void AimReady()
    {
        AimChance = 3;
        playerManager.animator.SetBool("AimModeParam", true);
        playerManager.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, "AimMode", false, canMove: true, canRotate: true);
    }

    public void OnAimShot()
    {
        AimChance--;

        // Trace와는 별개로 Ray를 쏜다.
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2));
        Vector3 ScreenToPoint = ray.GetPoint(20f);

        Vector3 playPos = playerManager.playerEquipmentManager.LeftWeaponManager.transform.position;
        RaycastHit[] hitInfo = Physics.RaycastAll(playPos, ScreenToPoint - playPos, 20f); // damagable

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
                    if (monster != null) playerManager.playerEquipmentManager.LeftWeaponManager?.damageCollider.RayTrigger(monster);
                }
            }
        }

        Debug.DrawRay(playPos, ScreenToPoint - playPos, Color.blue, 3f);

        if (AimChance == 0)
        { 
            
        }
    }

}
