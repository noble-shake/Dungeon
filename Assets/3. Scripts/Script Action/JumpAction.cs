using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class JumpAction : NetworkBehaviour
{
    public bool frontJump = false;
    public bool backJump = false;
    private CharacterManager character;
    public CharacterController controller;
    [SerializeField] private JumpData[] jumpDataList;

    public IEnumerator CoroutineAction;
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
    [SerializeField] private float endTime = 0.9f;

    [SerializeField] private string testAnimation = "";



    void Awake()
    {
        character = GetComponent<CharacterManager>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (frontJump)
        {
            frontJump = false;
            character.characterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Attack, testAnimation, true, applyRootmotion: false);
            StartCoroutine(ParabolicJumpCoroutine(Vector3.down));
        }
        if (backJump)
        {
            backJump = false;
            character.characterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Attack, testAnimation, true, applyRootmotion: false);
            StartCoroutine(ParabolicJumpCoroutine(Vector3.up));
        }
    }


    public void StartJumpAction(int index)
    {
        // 점프공격을 하기 전에 무조건 타겟이 있는지 알 수 있느냐?
        // 비해이비어 그래프에서 타겟을 정하고 공격을 정하기 때문에 가능.
        if (character.characterCombatManager.currentTarget == null)
        {
            Debug.Log("점프를 뛸 대상 없음");
            return;
        }
        InitializeJumpData(index);
        Vector3 destination = character.characterCombatManager.currentTarget.transform.position;
        StartCoroutine(ParabolicJumpCoroutine(destination));
    }

    private void InitializeJumpData(int index)
    {
        // 점프 데이터 초기화
        if (index < 0 || index >= jumpDataList.Length)
        {
            Debug.LogError("Invalid jump data index: " + index);
            return;
        }
        waitTime = jumpDataList[index].waitTime;
        liftHeight = jumpDataList[index].liftHeight;
        liftTime = jumpDataList[index].liftTime;
        holdTime = jumpDataList[index].holdTime;
        diveTime = jumpDataList[index].diveTime;
        endTime = jumpDataList[index].endTime;
    }

    public IEnumerator ParabolicJumpCoroutine(Vector3 diveDestination)
    {
        if (diveDestination == Vector3.down)
        {
            diveDestination = transform.position + transform.forward * 5f;
        }
        if (diveDestination == Vector3.up)
        {
            diveDestination = transform.position - transform.forward * 5f;
        }
        Vector3 startPosition = transform.position;

        // 궤적의 최고점 높이
        // 대상과 높이 차이가 있을 경우를 고려함.
        float peakHeight = liftHeight + Mathf.Max(startPosition.y, diveDestination.y);

        float elapsedTime = 0f;

        // 대기 시간
        while (elapsedTime < waitTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;

        // 점프 전 중력의 영향을 받지 않는다.
        character.characterLocomotionManager.useGravity = false;

        // 포물선 이동
        while (elapsedTime < diveTime)
        {
            float t = elapsedTime / diveTime;
            float easedT = EaseOutExpo(t);

            // 포물선 경로 계산
            Vector3 currentPos = GetParabolaPoint(startPosition, diveDestination, peakHeight, t);
            Vector3 moveStep = currentPos - transform.position;

            controller.Move(moveStep);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // 착지 후 중력의 영향을 다시 받는다.
        character.characterLocomotionManager.useGravity = true;
    }
    Vector3 GetParabolaPoint(Vector3 start, Vector3 end, float height, float t)
    {
        // 직선 보간
        Vector3 mid = Vector3.Lerp(start, end, t);

        // 높이 보간 (4ht(1-t) = 1차원 포물선 식)
        float parabolaY = 4 * height * t * (1 - t);

        // y값은 포물선으로 덮어씌움
        mid.y = Mathf.Lerp(start.y, end.y, t) + parabolaY;

        return mid;
    }

    private float EaseOutExpo(float t)
    {
        return t == 1 ? 1 : 1 - Mathf.Pow(2, -10 * t);
    }

}
