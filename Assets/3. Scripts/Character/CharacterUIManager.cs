using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 몬스터 UI 매니저를 주로 담당함. 
public class CharacterUIManager : MonoBehaviour
{
    [Header("UI")]
    public bool hasFloatingHPBar = true;
    public UI_Character_HP_Bar characterHPBar;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    public void OnHPChanged(int oldValue, int newValue)
    {
        characterHPBar.oldHealthValue = oldValue;
        characterHPBar.SetStat(newValue);
    }
}
