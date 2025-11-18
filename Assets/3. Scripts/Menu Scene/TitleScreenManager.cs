using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


// NOTE : Select() 함수는 게임 패드를 사용할 때만 지원.
/// <summary>
/// 타이틀 화면의 UI와 그로 인한 상호작용 함수 등을 담당한다.
/// </summary>
public class TitleScreenManager : MonoBehaviour
{
    public static TitleScreenManager instance;

    [Header("Menus")]
    [SerializeField] GameObject titleScreenMainMenu;
    [SerializeField] GameObject titleScreenLoadMenu;

    [Header("Buttons")]

    [SerializeField] Button mainMenuNewGameButton;
    [SerializeField] Button loadMenuReturnButton;
    [SerializeField] Button mainMenuLoadGameButton;
    [SerializeField] Button deleteCharacterPopUpConfirmButton;

    [Header("Pop ups")]
    [SerializeField] GameObject noCharacterSlotsPopup;
    [SerializeField] Button noCharacterSlotsOkayButton;
    [SerializeField] GameObject deleteCharacterSlotPopUp;

    [Header("Character Slots")]
    public CharacterSlot currentSelectedCharacterSlot = CharacterSlot.NO_SLOT;
    public int currentSelectedCharacterSlotIndex = -1;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // TODO : 시작부터 호스트 클라이언트를 정하고 시작하는 건 무리가 있을 듯. 
    // 애시당초 로비라는 개념이 있으니... 어차피 나중에 갈아 엎을 것이니 냅둡시다.
    public void StartNetworkAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    // UI Button
    public void StartNewGame()
    {
        WorldSaveGameManager.instance.AttempToCreateNewGame();
        // StartCoroutine(WorldSaveGameManager.instance.LoadWorldScene());
    }

    public void OpenLoadGameMenu()
    {
        titleScreenMainMenu.SetActive(false);
        titleScreenLoadMenu.SetActive(true);
        // loadMenuReturnButton.Select();
    }

    public void CloseLoadGameMenu()
    {
        titleScreenLoadMenu.SetActive(false);
        titleScreenMainMenu.SetActive(true);

        // mainMenuLoadGameButton.Select();
    }

    public void DisplayNoFreeCharacterSlotsPopup()
    {
        noCharacterSlotsPopup.SetActive(true);
        // noCharacterSlotsOkayButton.Select();
    }

    public void CloseNoFreeCharacterSlotsPopUp()
    {
        noCharacterSlotsPopup.SetActive(false);
        // mainMenuNewGameButton.Select();
    }

    public void SelectedCharacterSlot(CharacterSlot characterSlot, int characterSlotIndex)
    {
        currentSelectedCharacterSlot = characterSlot;
        currentSelectedCharacterSlotIndex = characterSlotIndex;

    }

    public void SelectNoSlot()
    {
        currentSelectedCharacterSlot = CharacterSlot.NO_SLOT;
    }

    public void AttempToDeleteCharacterSlot()
    {
        if (currentSelectedCharacterSlot != CharacterSlot.NO_SLOT)
        {
            deleteCharacterSlotPopUp.SetActive(true);
            // deleteCharacterPopUpConfirmButton.Select();
        }
    }

    public void DeleteCharacterSlot()
    {
        print(currentSelectedCharacterSlot);
        deleteCharacterSlotPopUp.SetActive(false);
        WorldSaveGameManager.instance.DeleteGame(currentSelectedCharacterSlotIndex);

        // 로드 창을 껐다 킴으로써 삭제된 데이터를 제외한 슬롯들을 재배치한다.
        titleScreenLoadMenu.SetActive(false);
        titleScreenLoadMenu.SetActive(true);
    }

    public void CloseDeleteCharacterPopUp()
    {
        deleteCharacterSlotPopUp.SetActive(false);
        // loadMenuReturnButton.Select();
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 플레이 모드 종료
#else
        Application.Quit(); // 실제 빌드에서 게임 종료
#endif
    }
}
