using DG.Tweening;
using DTT.AreaOfEffectRegions;
using System.Collections.Generic;
using UnityEngine;

public class TornadoPatternSettingManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static TornadoPatternSettingManager instance { get; private set; }
    public GameObject verticalStart;
    public GameObject verticalEnd;

    public GameObject horizontalStart;
    public GameObject horizontalEnd;
    public bool draw = false;

    public LineIndexData lineIndexData;

    public Transform magicWallTransform;

    public Transform magicCircleTransformList;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        if (draw)
        {
            draw = false;
            DrawLine();
        }
    }

    public int pairCount = 40;
    public int segmentCount = 1; // 몇 등분할지 (n)

    // public void DrawLine()
    // {
    //     int startCount = startLine.transform.childCount;
    //     int endCount = endLine.transform.childCount;

    //     if (startCount == 0 || endCount == 0) return;


    //     for (int i = 0; i < pairCount; i++)
    //     {
    //         int startRandomIndex = Random.Range(0, startCount);
    //         int endRandomIndex = Random.Range(0, endCount);
    //         Transform startChild = startLine.transform.GetChild(startRandomIndex);
    //         Transform endChild = endLine.transform.GetChild(endRandomIndex);

    //         Debug.DrawLine(startChild.position, endChild.position, Color.red, 3f);
    //     }
    // }
    public void DrawLine()
    {
        lineIndexData.Clear();
        int startCount = verticalStart.transform.childCount;
        int endCount = verticalEnd.transform.childCount;

        if (startCount == 0 || endCount == 0 || segmentCount <= 0) return;

        int pairsPerSegment = Mathf.Max(1, pairCount / segmentCount); // 각 구역에서 뽑을 개수

        for (int segment = 0; segment < segmentCount; segment++)
        {
            // 각 구역의 시작과 끝 인덱스 계산
            int startSegmentStart = (startCount * segment) / segmentCount;
            int startSegmentEnd = (startCount * (segment + 1)) / segmentCount;

            int endSegmentStart = (endCount * segment) / segmentCount;
            int endSegmentEnd = (endCount * (segment + 1)) / segmentCount;

            for (int i = 0; i < pairsPerSegment; i++)
            {
                // 각 구역 내에서 랜덤 인덱스 선택
                int startRandomIndex = Random.Range(startSegmentStart, startSegmentEnd);
                int endRandomIndex = Random.Range(endSegmentStart, endSegmentEnd);

                lineIndexData.verticalStartIndices.Add(startRandomIndex);
                lineIndexData.verticalEndIndices.Add(endRandomIndex);

                Transform startChild = verticalStart.transform.GetChild(startRandomIndex);
                Transform endChild = verticalEnd.transform.GetChild(endRandomIndex);

                Debug.DrawLine(startChild.position, endChild.position, Color.red, 3f);
            }
        }

        int startCountH = horizontalStart.transform.childCount;
        int endCountH = horizontalEnd.transform.childCount;

        if (startCount == 0 || endCount == 0 || segmentCount <= 0) return;


        for (int segment = 0; segment < segmentCount; segment++)
        {
            // 각 구역의 시작과 끝 인덱스 계산
            int startSegmentStart = (startCountH * segment) / segmentCount;
            int startSegmentEnd = (startCountH * (segment + 1)) / segmentCount;

            int endSegmentStart = (endCountH * segment) / segmentCount;
            int endSegmentEnd = (endCountH * (segment + 1)) / segmentCount;

            for (int i = 0; i < pairsPerSegment; i++)
            {
                // 각 구역 내에서 랜덤 인덱스 선택
                int startRandomIndex = Random.Range(startSegmentStart, startSegmentEnd);
                int endRandomIndex = Random.Range(endSegmentStart, endSegmentEnd);

                lineIndexData.horizontalStartIndices.Add(startRandomIndex);
                lineIndexData.horizontalEndIndices.Add(endRandomIndex);

                Transform startChild = horizontalStart.transform.GetChild(startRandomIndex);
                Transform endChild = horizontalEnd.transform.GetChild(endRandomIndex);

                Debug.DrawLine(startChild.position, endChild.position, Color.red, 3f);
            }
        }
        ShowIndicatorFromSavedData(horizontalStart, horizontalEnd, lineIndexData.horizontalStartIndices, lineIndexData.horizontalEndIndices);
        ShootTornadoFromSavedData(horizontalStart, horizontalEnd, lineIndexData.horizontalStartIndices, lineIndexData.horizontalEndIndices);
        ShowIndicatorFromSavedData(verticalStart, verticalEnd, lineIndexData.verticalStartIndices, lineIndexData.verticalEndIndices);
        ShootTornadoFromSavedData(verticalStart, verticalEnd, lineIndexData.verticalStartIndices, lineIndexData.verticalEndIndices);
    }
    private void ShowIndicatorFromSavedData(GameObject startObj, GameObject endObj, List<int> startIndexList, List<int> endIndexList)
    {
        if (startObj == null || endObj == null || startIndexList.Count == 0 || endIndexList.Count == 0)
        {
            Debug.LogWarning("저장된 인덱스 데이터가 없습니다.");
            return;
        }

        for (int i = 0; i < startIndexList.Count; i++)
        {
            int startIndex = startIndexList[i];
            int endIndex = endIndexList[i];

            if (startIndex >= startObj.transform.childCount || endIndex >= endObj.transform.childCount) continue;

            Transform startChild = startObj.transform.GetChild(startIndex);
            Transform endChild = endObj.transform.GetChild(endIndex);
        }

        for (int i = 0; i < startIndexList.Count; i++)
        {
            int startIndex = startIndexList[i];
            int endIndex = endIndexList[i];

            if (startIndex >= startObj.transform.childCount || endIndex >= endObj.transform.childCount) continue;

            Transform startChild = startObj.transform.GetChild(startIndex);
            Transform endChild = endObj.transform.GetChild(endIndex);

            GameObject indicatorVFXPrefab = ObjectPoolManager.Singleton.GetPrefab(ObjectPoolType.FirstBoss, 12);
            GameObject indicatorVFX = ObjectPoolManager.Singleton.GetObject(ObjectPoolType.FirstBoss, 12);
            indicatorVFX.TryGetComponent(out LineRegion lineRegion);
            indicatorVFX.transform.position = startChild.position + new Vector3(0, 0.1f, 0); // 시작 위치에 VFX 소환
            Vector3 direction = endChild.position - startChild.position; // 시작 위치에서 끝 위치로 향하는 방향 벡터
            lineRegion.Angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            lineRegion.Length = Vector3.Distance(startChild.position, endChild.position); // 길이 설정

            // 두트윈으로 tornadoVFX를 endChild.position으로 이동시키기
            DOVirtual.DelayedCall(1f, () =>
            {
                ObjectPoolManager.Singleton.ReturnObject(indicatorVFX, indicatorVFXPrefab);
            });

        }
    }
    private void ShootTornadoFromSavedData(GameObject startObj, GameObject endObj, List<int> startIndexList, List<int> endIndexList)
    {
        if (startObj == null || endObj == null || startIndexList.Count == 0 || endIndexList.Count == 0)
        {
            Debug.LogWarning("저장된 인덱스 데이터가 없습니다.");
            return;
        }

        for (int i = 0; i < startIndexList.Count; i++)
        {
            int startIndex = startIndexList[i];
            int endIndex = endIndexList[i];

            if (startIndex >= startObj.transform.childCount || endIndex >= endObj.transform.childCount) continue;

            Transform startChild = startObj.transform.GetChild(startIndex);
            Transform endChild = endObj.transform.GetChild(endIndex);

            GameObject tornadoVFXPrefab = ObjectPoolManager.Singleton.GetPrefab(ObjectPoolType.FirstBoss, 8);
            GameObject tornadoVFX = ObjectPoolManager.Singleton.GetObject(ObjectPoolType.FirstBoss, 8);
            tornadoVFX.transform.position = startChild.position; // 시작 위치에 VFX 소환
            // 두트윈으로 tornadoVFX를 endChild.position으로 이동시키기
            tornadoVFX.transform
                .DOMove(endChild.position, 2f) // 2초 동안 이동
                .SetDelay(Random.Range(0f, 1f)) // 랜덤한 지연 시간
                .SetEase(Ease.Linear) // 부드러운 Ease-In-Out 효과
                .OnComplete(() =>
                {
                    // 이동이 끝난 후 VFX를 풀로 반환
                    ObjectPoolManager.Singleton.ReturnObject(tornadoVFX, tornadoVFXPrefab);
                });

            Debug.DrawLine(startChild.position, endChild.position, Color.blue, 3f); // 기존과 다른 색상 (파란색)
        }
    }

    // 

    //  private void DrawLinesFromSavedData(GameObject startObj, GameObject endObj, List<int> startIndexList, List<int> endIndexList)
    // {
    //     if (startObj == null || endObj == null || startIndexList.Count == 0 || endIndexList.Count == 0)
    //     {
    //         Debug.LogWarning("저장된 인덱스 데이터가 없습니다.");
    //         return;
    //     }

    //     for (int i = 0; i < startIndexList.Count; i++)
    //     {
    //         int startIndex = startIndexList[i];
    //         int endIndex = endIndexList[i];

    //         if (startIndex >= startObj.transform.childCount || endIndex >= endObj.transform.childCount) continue;

    //         Transform startChild = startObj.transform.GetChild(startIndex);
    //         Transform endChild = endObj.transform.GetChild(endIndex);

    //         Debug.DrawLine(startChild.position, endChild.position, Color.blue, 3f); // 기존과 다른 색상 (파란색)
    //     }
    // }
}
