using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class 
    AroundSelf : MonoBehaviour
{
    [SerializeField] private GameObject summonPrefab;
    [SerializeField] private List<Vector3> PointsAroundSelf = new();
    [SerializeField] private float summonMaxRadius = 10f;
    [SerializeField] private float summonMinRadius = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // 호스트의 
    public void SummonStoneWallNetworkObject()
    {
        if (GameManager.Instance.IsOwner == false) return;
        GetRandomPointsAroundSelf(summonMinRadius, summonMaxRadius);
        for (int i = 0; i < 2; i++)
        {
            GameObject summonObject = ObjectPoolManager.Singleton.GetObject(summonPrefab);
            summonObject.transform.position = PointsAroundSelf[i];
            summonObject.GetComponent<NetworkObject>().Spawn();
        }
    }

    public void GetRandomPointsAroundSelf(float minRadious, float maxRadius)
    {
        PointsAroundSelf.Clear();
        PointsAroundSelf = GetRandomPoints(transform.position, minRadious, maxRadius);
    }

    public List<Vector3> GetRandomPoints(Vector3 center, float minRadius, float maxRadius)
    {
        Vector3[] points = new Vector3[3];

        // 120도씩 나누어진 기준 각도 (0도, 120도, 240도)
        float[] baseAngles = { 0f, 120f, 240f };

        for (int i = 0; i < 3; i++)
        {
            // minRadius ~ maxRadius 범위에서 랜덤한 거리 선택
            float randomDistance = Random.Range(minRadius, maxRadius);

            // 랜덤한 위치 (기준 각도에서 소폭 변화 추가)
            float angleOffset = Random.Range(-30f, 30f); // ±30도 변형 가능
            float angle = baseAngles[i] + angleOffset;

            // 각도를 라디안으로 변환
            float radian = angle * Mathf.Deg2Rad;

            // 위치 계산
            float x = center.x + randomDistance * Mathf.Cos(radian);
            float z = center.z + randomDistance * Mathf.Sin(radian);
            points[i] = new Vector3(x, center.y, z);
        }

        return points.ToList();
    }

}
