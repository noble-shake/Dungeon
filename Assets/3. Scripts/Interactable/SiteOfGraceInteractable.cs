using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class SiteOfGraceInteractable : Interactable
{
    [Header("Site of Grace Info")]
    [SerializeField] int siteOfGraceID;
    // 이거 쓰기 권한을 Owner로만 한정하는 게 맞나?
    public NetworkVariable<bool> isActivated = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("VFX")]
    [SerializeField] GameObject activatedParticles;

    [Header("Interaction Text")]
    [SerializeField] string unactivatedInteractionText = "Restore Site of Grace";
    [SerializeField] string actiavtedInteractionText = "Rest";

    
    protected override void Start()
    {
        base.Start();

        // IsOwner는 OnNetworkSpawn이후에 사용하는 것이 바람직함.
        // if (IsOwner)
        // {
        //     if (WorldSaveGameManager.instance.currentCharacterData.sitesOfGrace.ContainsKey(siteOfGraceID))
        //     {
        //         isActivated.Value = WorldSaveGameManager.instance.currentCharacterData.sitesOfGrace[siteOfGraceID];
        //     }
        //     else
        //     {
        //         isActivated.Value = false;
        //     }
        // }

        if (isActivated.Value)
        {
            interactableText = actiavtedInteractionText;
        }
        else
        {
            interactableText = unactivatedInteractionText;
        }
    }

    private void RestoreSiteOfGrace(PlayerManager player)
    {
        isActivated.Value = true;
        // 세이브 데이터에 활성화된 축복장소 저장
        if (WorldSaveGameManager.instance.currentCharacterData.sitesOfGrace.ContainsKey(siteOfGraceID))
            WorldSaveGameManager.instance.currentCharacterData.sitesOfGrace.Remove(siteOfGraceID);

        WorldSaveGameManager.instance.currentCharacterData.sitesOfGrace.Add(siteOfGraceID, true);

        // 애니메이션 실행
        player.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Etc,"Activate_01", true);
        // 팝업창 띄우기
        HUD_UIManager.instance.popUpUIManager.SendRpcMessagePopUp("Site Of Grace Restored.");

        StartCoroutine(WaitForAnimationAndPopUpthenRestoreCollider());
        // 축복 활성화
    }



    private void RestAtSiteOfGrace(PlayerManager player)
    {
        Debug.Log("휴식중");
        interactableCollider.enabled = true; // 일시적으로 콜라이더를 재활성화하고 
        player.playerNetworkManager.currentHp.Value = player.playerNetworkManager.maxHp.Value;
        player.playerNetworkManager.currentStamina.Value = player.playerNetworkManager.maxStamina.Value;

        // 플라스크 충전
        // 캐릭터의 무브 퀘스트 강제
        // 캐릭터 또는 몬스터의 위치 초기화
        WorldAIManager.instance.ResetAllCharacters();
    }

    private IEnumerator WaitForAnimationAndPopUpthenRestoreCollider()
    {
        yield return new WaitForSeconds(2);
        interactableCollider.enabled = true;
    }

    public override void Interact(PlayerManager player)
    {
        base.Interact(player);

        if (!isActivated.Value)
        {
            RestoreSiteOfGrace(player);
        }
        else
        {
            RestAtSiteOfGrace(player);
        }
    }

    private void OnIsActivatedChanged(bool oldStatus, bool newStatus)
    {
        if (isActivated.Value)
        {
            activatedParticles.SetActive(true);
            interactableText = actiavtedInteractionText;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            if (WorldSaveGameManager.instance.currentCharacterData.sitesOfGrace.ContainsKey(siteOfGraceID))
            {
                isActivated.Value = WorldSaveGameManager.instance.currentCharacterData.sitesOfGrace[siteOfGraceID];
            }
            else
            {
                isActivated.Value = false;
            }
        }
        else
        {
            OnIsActivatedChanged(false, isActivated.Value);
        }

        isActivated.OnValueChanged += OnIsActivatedChanged;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        isActivated.OnValueChanged -= OnIsActivatedChanged;
    }


}
