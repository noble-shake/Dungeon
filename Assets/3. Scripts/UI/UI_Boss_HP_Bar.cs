using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Boss_StatBar : UI_StatBar
{
    [SerializeField] AIBossCharacterManager boss;

    public void EnableBossBar(AIBossCharacterManager boss)
    {
        this.boss = boss;
        this.boss.aiCharacterNetworkManager.currentHp.OnValueChanged += OnBossHPChanged;
        base.SetMaxStat(this.boss.characterNetworkManager.maxHp.Value);
        base.SetStat(this.boss.aiCharacterNetworkManager.currentHp.Value);
        GetComponentInChildren<TextMeshProUGUI>().text = this.boss.bossName;
    }

    private void OnBossHPChanged(int oldValue, int newValue)
    {
        SetStat(newValue);

        if (newValue <= 0)
        {
            // StartCoroutine()
        }
    }

}
