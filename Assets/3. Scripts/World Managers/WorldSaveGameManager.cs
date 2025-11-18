using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// 새 게임 시작 혹은 불러오기 할 때 캐릭터 생성/불러오기 에 관여한다.
/// 
/// </summary>
public class WorldSaveGameManager : MonoBehaviour
{
    public static WorldSaveGameManager instance;

    // [HideInInspector] 
    public PlayerManager player;

    [Header("SAVE/LOAD")]
    [SerializeField] bool saveGame;
    [SerializeField] bool loadGame;


    [Header("World Scene Index")]
    [SerializeField] private int worldSceneIndex = 1;

    [Header("Save Data Writer")]
    private SaveFileDataWriter saveFileDataWriter;

    [Header("Current Character Data")]
    public CharacterSlot currentCharacterSlotBeingUsed;
    private int curretSlotIndex = -1;
    public CharacterSaveData currentCharacterData;
    private string saveFileName;

    [Header("Character Slots")]
    public List<CharacterSaveData> saveDataList = new List<CharacterSaveData>();
    public CharacterSaveData characterSlot01;
    public CharacterSaveData characterSlot02;
    public CharacterSaveData characterSlot03;
    public CharacterSaveData characterSlot04;
    public CharacterSaveData characterSlot05;
    public CharacterSaveData characterSlot06;
    public CharacterSaveData characterSlot07;
    public CharacterSaveData characterSlot08;
    public CharacterSaveData characterSlot09;
    public CharacterSaveData characterSlot10;
    private int characterSlotCount = 10;

    [HideInInspector]
    public bool readyForSlotInitialization = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            saveDataList.Add(characterSlot01);
            saveDataList.Add(characterSlot02);
            saveDataList.Add(characterSlot03);
            saveDataList.Add(characterSlot04);
            saveDataList.Add(characterSlot05);
            saveDataList.Add(characterSlot06);
            saveDataList.Add(characterSlot07);
            saveDataList.Add(characterSlot08);
            saveDataList.Add(characterSlot09);
            saveDataList.Add(characterSlot10);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        LoadAllCharacterProfiles();
    }

    private void Update()
    {
        if (saveGame)
        {
            saveGame = false;
            SaveGame();
        }

        if (loadGame)
        {
            loadGame = false;
            LoadGame();
        }
    }

    // 네트워크 매니저의 씬 동기화를 사용하지 않은 함수
    // public IEnumerator LoadWorldScene()
    // {
    //     AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);


    //     // AsyncOperation loadOperation = SceneManager.LoadSceneAsync(currentCharacterData.sceneIndex);


    //     player.LoadGameDataFromCurrentCharacterData(ref currentCharacterData);
    //     yield return null;
    // }

    public string DecideCharacterFileNameBasedOnCharacterSlotIndex(int index)
    {
        string baseName = "characterSLot_";
        return baseName + $"{index:D2}";
    }


    /// <summary>
    /// 새 게임을 만들 수 있는지 체크하고 가능하다면 남는 세이브칸에 캐릭터를 생성한다.
    /// </summary>
    public void AttempToCreateNewGame()
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        for (int slotIndex = 1; slotIndex <= characterSlotCount; slotIndex++)
        {
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotIndex(slotIndex);
            // 슬롯 자리가 남아있다면 새게임 시작
            if (!saveFileDataWriter.IfFileExists())
            {
                currentCharacterSlotBeingUsed = (CharacterSlot)slotIndex;
                curretSlotIndex = slotIndex;
                currentCharacterData = new CharacterSaveData();
                Debug.Log($"{slotIndex}번째 슬롯에 새 캐릭터를 삽입합니다.");
                Debug.Log("새 게임을 시작합니다.");
                NewGame();
                return;
            }
        }
        Debug.Log("새 게임을 위한 슬롯이 남아있지 않습니다.");
        TitleScreenManager.instance.DisplayNoFreeCharacterSlotsPopup();

    }

    /// <summary>
    /// 
    /// </summary>
    private void NewGame()
    {
        // player.playerNetworkManager.vitality.Value = 15;
        // player.playerNetworkManager.enduracne.Value = 10;

        SaveGame();
        // StartCoroutine(LoadWorldScene());
        LoadSceneAndCharacter(worldSceneIndex);
    }

    public void LoadGame()
    {
        saveFileName = DecideCharacterFileNameBasedOnCharacterSlotIndex((int)currentCharacterSlotBeingUsed);

        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = saveFileName;
        currentCharacterData = saveFileDataWriter.LoadSaveFile();

        // StartCoroutine(LoadWorldScene());
        LoadSceneAndCharacter(worldSceneIndex);
    }

    public void SaveGame()
    {
        saveFileName = DecideCharacterFileNameBasedOnCharacterSlotIndex((int)currentCharacterSlotBeingUsed);

        saveFileDataWriter = new SaveFileDataWriter();

        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = saveFileName;

        player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);

        saveFileDataWriter.CreateNewCharacterSaveFile(currentCharacterData);
    }

    public void DeleteGame(int characterSlotIndex)
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotIndex(characterSlotIndex);

        saveFileDataWriter.DeleteSaveFile();
    }

    // 네트워크 매니저의 씬 동기화를 사용한 함수
    /// <summary>
    /// 캐릭터 데이터와 씬을 로드한다.
    /// </summary>
    /// <param name="buildIndex"></param>
    public void LoadSceneAndCharacter(int buildIndex)
    {
        string worldScene = SceneUtility.GetScenePathByBuildIndex(buildIndex);
        NetworkManager.Singleton.SceneManager.LoadScene(worldScene, LoadSceneMode.Single);

        player.LoadGameDataFromCurrentCharacterData(ref currentCharacterData);
    }

    private void LoadAllCharacterProfiles()
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

        for (int slotIndex = 1; slotIndex <= characterSlotCount; slotIndex++)
        {
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotIndex(slotIndex);
            saveDataList[slotIndex - 1] = saveFileDataWriter.LoadSaveFile();
        }
        readyForSlotInitialization = true;
    }

    public int GetWorldSceneIndex()
    {
        return worldSceneIndex;
    }

}
