using System;
using Unity.Netcode;
using UnityEngine;

public class EmblemPattern : NetworkBehaviour
{
    public int patternCount = 0;
    public int patternChance = 0;
    public int[] patternChanceForNetwork;

    public bool patternStarted = false;
    public bool emblemSettingReady = false;

    public int selectedClassIndex = -1;
    public bool isGroggyTime = false;
    public PlayerManager selectedPlayer;

    [ServerRpc]
    public void StartPatternServerRpc()
    {
        patternCount = 0;
        patternChance = patternChanceForNetwork[GameManager.Instance.connectedPlayerList.Count - 1];
        StartPatternClientRpc();
    }

    [ClientRpc]
    private void StartPatternClientRpc()
    {
        Initialize();
    }

    // 살아있는 플레이어 하나를 정한다
    // 그 플레이어의 맞는 엠블럼을 정한다.
    // UI 매니저에게 넘긴다.
    public void PopUpEmblemUI()
    {
        int num = 0;
        PlayerManager randomPlayer = null;
        while (true)
        {
            randomPlayer = GameManager.Instance.GetRandomPlayer();
            if (randomPlayer.isDead.Value)
            {
                num++;
                continue;
            }
            if (num == 4)
            {
                Debug.Log("산 사람을 찾을 수 없습니다.");
                break;
            }
            if (randomPlayer != null)
            {
                break;
            }
        }
        PlayerClass playerClass = randomPlayer.CharacterClass;
        selectedPlayer = randomPlayer;
        selectedClassIndex = (int)playerClass;
        // 화면 가운데에 엠블럼을 띄운다.
        // 그 뒤 엠블럼이 4개로 나뉘며 있어야될 자리로 이동한다. 이 과정은 두트윈으로 진행된다.
        // TODO : RPC로 진행해야 함 이거 

        PopUpEmblemServerRpc((int)playerClass);
    }

    [ServerRpc]
    private void PopUpEmblemServerRpc(int playerClass)
    {
        PopUpEmblemClientRpc(playerClass);
    }

    [ClientRpc]
    private void PopUpEmblemClientRpc(int playerClass)
    {
        HUD_UIManager.instance.combatUIManager.EmblemPatternProcess(playerClass);
    }

    public bool CheckActivatedEmblem()
    {
        return HUD_UIManager.instance.combatUIManager.CheckActivatedEmblem();
    }

    // 플레이어가 도중에 나가서 기믹을 수행할 수 없으면? 

    public bool CheckLeftChance()
    {
        return patternChance > 0;
    }

    public void Initialize()
    {
        patternStarted = true;
        Debug.Log("엠블럼 패턴 시작했습니다. " + patternChance);
    }

    // 아이콘이 있으면 없애고
    // 아이콘이 없으면 2초 그로기? -> 이거를 어떻게 비해이비어 그래프로 못 옮겨주나? 

    public void RemoveIcon(int characterClass)
    {
        if (HUD_UIManager.instance.combatUIManager.IfActivatedIconExists())
        {
            RemoveIconServerRpc();
            if (!HUD_UIManager.instance.combatUIManager.IfActivatedIconExists())
            {
                isGroggyTime = true;
                emblemSettingReady = false;
            }
        }
    }

    [ServerRpc]
    private void RemoveIconServerRpc()
    {
        RemoveIconClientRpc();
    }

    [ClientRpc]
    private void RemoveIconClientRpc()
    {
        HUD_UIManager.instance.combatUIManager.RemoveIcon();
    }

    public bool CheckValidClass(int characterClass)
    {
        return selectedClassIndex == characterClass;
    }

    [ServerRpc]
    public void EndPatterServerRpc()
    {
        if (IsServer)
        {
            EndPatternClientRpc();
        }
    }

    [ClientRpc]
    private void EndPatternClientRpc()
    {
        patternStarted = false;
    }
}
