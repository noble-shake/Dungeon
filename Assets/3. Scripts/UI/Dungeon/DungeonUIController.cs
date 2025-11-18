using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUIController : MonoBehaviour
{
    public int dungeonIndex;
    public TextMeshProUGUI dungeonName;
    public Image bossIcon;
    public TextMeshProUGUI bossNickname;
    public TextMeshProUGUI bossName;

    public TextMeshProUGUI hpDesc;
    public TextMeshProUGUI damageDesc;
    public TextMeshProUGUI speedDesc;
    public TextMeshProUGUI additionalDesc;

    public TextMeshProUGUI dungeonDescription;
    public string dungeonSceneName;

    public void SetDataFromDungeonObject(DungeonObject dungeonObject)
    {
        dungeonIndex = dungeonObject.dungeonIndex;
        dungeonName.text = dungeonObject.dungeonName;
        bossIcon.sprite = dungeonObject.bossIcon;
        bossNickname.text = dungeonObject.bossNickname;
        bossName.text = dungeonObject.bossName;
        hpDesc.text = dungeonObject.hpDesc;
        damageDesc.text = dungeonObject.damageDesc;
        speedDesc.text = dungeonObject.speedDesc;
        additionalDesc.text = dungeonObject.additionalDesc;
        dungeonSceneName = dungeonObject.dungeonSceneName;
    }
}
