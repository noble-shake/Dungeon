using System;
using UnityEngine;

public class CompanionInfo : MonoBehaviour
{

    [SerializeField] private Transform classIconGroup;
    [SerializeField] private UI_StatBar hpBar;
    [SerializeField] private ulong networkObjectId;
    // 나중에 플레이어 이름 동기화도 해줘야 함. 
    [SerializeField] private string playerName;

    public ulong NetworkObjectId { get => networkObjectId; set => networkObjectId = value; }

    public void Initialize(PlayerManager playerManager)
    {
        NetworkObjectId = playerManager.playerNetworkManager.NetworkObjectId;

        foreach (Transform child in classIconGroup)
        {
            child.gameObject.SetActive(false);
        }
        // 클래스에 맞는 아이콘 활성화
        int classIconIndex = (int)playerManager.characterClass;
        classIconGroup.GetChild(classIconIndex).gameObject.SetActive(true);

        // 남의 캐릭터의 체력과 UI 체력바 연동
        playerManager.playerNetworkManager.maxHp.OnValueChanged += (previousValue, newValue) => hpBar.SetMaxStat(newValue);
        playerManager.playerNetworkManager.currentHp.OnValueChanged += (previousValue, newValue) => hpBar.SetStat(newValue);

        hpBar.SetMaxStat(playerManager.playerCombatManager.hp);
        // hpBar.SetStat(playerManager.playerNetworkManager.currentHp.Value);
    }

}
