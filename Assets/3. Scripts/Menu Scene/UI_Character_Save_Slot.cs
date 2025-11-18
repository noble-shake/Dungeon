using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Character_Save_Slot : MonoBehaviour
{
    SaveFileDataWriter saveFileWriter;

    [Header("Game Slot")]
    public CharacterSlot characterSlot;
    [SerializeField] int characterSlotIndex = -1; // 인스펙터 창에서 초기화 되었음.

    [Header("Character Info")]
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI timePlayed;

    private void OnEnable()
    {
        StartCoroutine(WaitUntilSaveDataLoaded());
    }

    private IEnumerator WaitUntilSaveDataLoaded()
    {
        while (true)
        {
            if (WorldSaveGameManager.instance.readyForSlotInitialization == false)
            {
                yield return null;
            }
            else
                break;
        }
        LoadSaveSlot();
    }

    // FIXED : 스위치 문으로 모든 캐릭터 슬롯의 경우를 일일히 써주다가, enum 타입 기반과 slotindex로 알아서 데이터 불러올 수 있게끔 전환함.
    private void LoadSaveSlot()
    {
        saveFileWriter = new SaveFileDataWriter();
        saveFileWriter.saveDataDirectoryPath = Application.persistentDataPath;

        saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotIndex(characterSlotIndex);
        if (saveFileWriter.IfFileExists())
        {
            characterName.text = WorldSaveGameManager.instance.saveDataList[characterSlotIndex - 1].characterName;
        }
        else
        {
            gameObject.SetActive(false);
        }
        return;
    }

    // 세이브 데이터 목록창에서 세이브 데이터를 클릭하면 발동 된다.
    public void LoadGameFromCharacterSlot()
    {
        WorldSaveGameManager.instance.currentCharacterSlotBeingUsed = (CharacterSlot)characterSlotIndex;
        WorldSaveGameManager.instance.LoadGame();
    }

    // 세이브 데이터 목록창에서 슬롯 등에 마우스를 갖다댈 때 발동 된다. 조건은 Event Trigger의 Pointer Enter
    public void SelectCurrentSlot()
    {
        TitleScreenManager.instance.SelectedCharacterSlot(characterSlot, characterSlotIndex);
    }

}
