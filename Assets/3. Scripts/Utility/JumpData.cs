using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Jump Actions/Jump Data")]
public class JumpData : ScriptableObject
{
    [Header("떠오를 때 설정")]
    [Tooltip("도약 전 준비 시간")]
    public float waitTime = 1.5f;  // 도약 준비 시간
    [Tooltip("도약 최고점점 높이")]
    public float liftHeight = 2f;
    [Tooltip("최고 높이까지 걸리는 시간")]
    public float liftTime = 1.5f;
    [Tooltip("최고 높이에서 정지해 있는 시간")]
    public float holdTime = 0f;
    [Tooltip("강하하는데데 걸리는 시간")]
    public float diveTime = 1.0f;  // 강하하는 시간
    [Tooltip("애니메이션 루프를 중단하는 정규화된 시점값")]
    // diveTime을 기준으로 한다. diveTime의 endTime 지점부터 애니메이션 루프가 중단된다.
    public float endTime = 0.9f;
}
