using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


/// <summary>
/// 매 씬에 하나씩 존재하는 UI 매니저.
/// 애초에 로비씬에서부터 얘를 만들어주면 되지 않나? 생각이 들기도 함.
/// 
/// </summary>
public class HUD_UIManager : MonoBehaviour
{
    // 얘가 싱글턴이면 안 됨. 
    public static HUD_UIManager instance;

    [HideInInspector] public CombatUIManager combatUIManager;
    [HideInInspector] public PopUpUIManager popUpUIManager;
    [HideInInspector] public SettingUIManager settingUIManager;
    [HideInInspector] public ScreenUIManager screenUIManager;

    [Header("UI Flags")]
    public bool menuWindowIsOpen = false; // 인벤토리 창, 장비 창, 강화 창 등등.
    public bool popUpWindowIsOpen = false; // 

    public void CloseInGameUIWindow()
    {
        settingUIManager.gameObject.SetActive(false);
    }

    public bool IsActiveInGameUIWindow()
    {
        return settingUIManager.gameObject.activeSelf;
    }

    public void OpenInGameUIWindow()
    {
        settingUIManager.gameObject.SetActive(true);
    }

    public void ShowSkillDescUIWindow(PlayerClass characterClass)
    {
        if (combatUIManager.skillDescImage.gameObject.activeSelf)
        {
            combatUIManager.skillDescImage.gameObject.SetActive(false);
        }
        else
        {
            combatUIManager.skillDescImage.gameObject.SetActive(true);
            combatUIManager.skillDescImage.sprite = combatUIManager.skillDescImageList[(int)characterClass];
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

        combatUIManager = GetComponentInChildren<CombatUIManager>();
        popUpUIManager = GetComponentInChildren<PopUpUIManager>();
        screenUIManager = GetComponentInChildren<ScreenUIManager>();
        settingUIManager = GetComponentInChildren<SettingUIManager>(true);
    }

}
