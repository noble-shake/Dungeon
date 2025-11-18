using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LevelSelectUI : MonoBehaviour
{
    [SerializeField] private DungeonUIController currentDungeon;
    [SerializeField] private DungeonUIController spareDungeon;
    private CanvasGroup tempCanvasGroup;
    private DungeonUIController tempDungeon;
    CanvasGroup currentCanvasGroup;
    CanvasGroup spareCanvasGroup;

    public float testPixelMoveAmount = 500f;
    public float testTime = 1f;

    private int currentDungeonIndex = 0;
    private int dungeonCount = -1;


    private void Awake()
    {
        dungeonCount = WorldDungeonManager.instance.GetDungeonCount();

        currentCanvasGroup = currentDungeon.GetComponent<CanvasGroup>();
        spareCanvasGroup = spareDungeon.GetComponent<CanvasGroup>();

        currentCanvasGroup.alpha = 1;
        spareCanvasGroup.alpha = 0;

        SetDungeonInfoOnUI(currentDungeon, WorldDungeonManager.instance.GetDungeonObjectByIndex(currentDungeonIndex));
    }

    // 1. 현재 UI는 오른쪽으로 밀고
    // 2. 스페어 UI는 왼쪽에서 오른쪽으로 뽑아온다.

    // 3. 현재 UI는 페이드 아웃 
    // 4. 스페어 UI는 페이드 인 
    public void MovePreviousDungeon()
    {
        // if (dungeonCount == 1) return;
        currentDungeonIndex -= 1;
        currentDungeonIndex += dungeonCount;
        currentDungeonIndex %= dungeonCount;
        DungeonObject dungeon = WorldDungeonManager.instance.GetDungeonObjectByIndex(currentDungeonIndex);


        // 오른쪽으로 이동
        RectTransform currentRectTransform = currentDungeon.GetComponent<RectTransform>();
        RectTransform spareRectTransform = spareDungeon.GetComponent<RectTransform>();
        spareRectTransform.anchoredPosition = new Vector3(currentRectTransform.anchoredPosition.x - testPixelMoveAmount
        , currentRectTransform.anchoredPosition.y);
        currentRectTransform.DOAnchorPosX(currentRectTransform.anchoredPosition.x + testPixelMoveAmount, testTime);
        // 페이드 아웃
        currentCanvasGroup.DOFade(0f, testTime).SetEase(Ease.OutCirc);

        spareDungeon.SetDataFromDungeonObject(dungeon);
        spareRectTransform.DOAnchorPosX(spareRectTransform.anchoredPosition.x + testPixelMoveAmount, testTime);

        spareCanvasGroup.DOFade(1f, testTime).SetEase(Ease.OutCirc);

        tempDungeon = currentDungeon;
        currentDungeon = spareDungeon;
        spareDungeon = tempDungeon;

        tempCanvasGroup = currentCanvasGroup;
        currentCanvasGroup = spareCanvasGroup;
        spareCanvasGroup = tempCanvasGroup;


        Debug.Log("클릭은 됐음");
    }

    public void MoveNextDungeon()
    {
        // if (dungeonCount == 1) return;
        currentDungeonIndex += 1;
        currentDungeonIndex %= dungeonCount;
        DungeonObject dungeon = WorldDungeonManager.instance.GetDungeonObjectByIndex(currentDungeonIndex);

        RectTransform currentRectTransform = currentDungeon.GetComponent<RectTransform>();
        RectTransform spareRectTransform = spareDungeon.GetComponent<RectTransform>();
        spareRectTransform.anchoredPosition = new Vector3(currentRectTransform.anchoredPosition.x + testPixelMoveAmount
        , currentRectTransform.anchoredPosition.y);

        spareDungeon.SetDataFromDungeonObject(dungeon);
        currentRectTransform.DOAnchorPosX(currentRectTransform.anchoredPosition.x - testPixelMoveAmount, testTime);
        // 현재 UI를 서서히 어둡게 (페이드 아웃)한다.
        currentCanvasGroup.DOFade(0f, testTime).SetEase(Ease.OutCirc);



        spareRectTransform.DOAnchorPosX(spareRectTransform.anchoredPosition.x - testPixelMoveAmount, testTime);

        spareCanvasGroup.DOFade(1f, testTime).SetEase(Ease.OutCirc);

        tempDungeon = currentDungeon;
        currentDungeon = spareDungeon;
        spareDungeon = tempDungeon;

        tempCanvasGroup = currentCanvasGroup;
        currentCanvasGroup = spareCanvasGroup;
        spareCanvasGroup = tempCanvasGroup;
        // spareDungeon.

    }

    private void SetDungeonInfoOnUI(DungeonUIController dungeonUIController, DungeonObject dungeon)
    {
        dungeonUIController.SetDataFromDungeonObject(dungeon);
    }

    // 화면 암전 기능이 없다.
    // 시작 버튼에 달려 있다.  
    public void GameStart()
    {
        LobbyParameters lobbyParameters = null;
        GameNetworkManager.Instance.StartAsHost(lobbyParameters, currentDungeon.dungeonSceneName);
    }

}
