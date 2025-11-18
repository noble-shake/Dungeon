using System.Collections;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class SpecialAttackAction_RiseAndDive : NetworkBehaviour
{
    private PlayerManager player;
    private Shockwave shockwave;
    public Vector3 divePosition;
    public CharacterController controller;
    public IEnumerator CoroutineAction;

    private void Awake()
    {
        player = GetComponent<PlayerManager>();
        shockwave = GetComponent<Shockwave>();
        controller = GetComponent<CharacterController>();
    }

    // 누울 자리를 보고 발을 뻗어야 한다.
    // case 1: 조준했을 때
    // 1. 상대방과 내가 높이 차가 있을 때 -> 됐음.
    // 높이 혹은 거리에 따른 예외 케이스는 조준이 끊기는 것으로 대체함.
    // case 2: 조준하지 않았을 때
    // 캐릭터가 바라보는 일정 거리로 한다.

    // 실행 중에 헤비어택을 맞았으면 -> 코루틴 자체를 씹어야한다. 현재는 무적. 

    [Header("떠오를 때 설정")]
    [Tooltip("도약 준비 시간")]
    public float waitTime = 1.5f;  // 도약 준비 시간
    [Tooltip("도약 높이")]
    public float liftHeight = 2f;
    [Tooltip("초기 상승 속도")]
    public float initialLiftSpeed = 5f;  // 초기 상승 속도
    [Tooltip("최고 높이까지 걸리는 시간")]
    public float liftTime = 1.5f;  // 공중에 떠 있는 시간
    [Tooltip("최고 높이에서 정지해 있는 시간")]
    public float holdTime = 0f;    // 강하 기다리는 시간간
    [Tooltip("강하하는데 걸리는 시간")]
    public float diveTime = 1.0f;  // 강하하는 시간
    public bool isGentleCurve = false;


    // 락온 안 했을 때, 점프 후 본인의 앞 쪽으로 강하한다.
    public void SpecialAttackActionRiseAndDive(Vector3 diveDestination)
    {
        SpecialAttackServerRpc(diveDestination);
    }
    // 락온 했을 때, 점프 후 회전하며 공중에서 타겟의 위치로 강하한다.
    public void SpecialAttackActionRiseAndDive(ulong networkObjectId)
    {
        SpecialAttackServerRpc(networkObjectId);
    }

    [ServerRpc]
    private void SpecialAttackServerRpc(Vector3 diveDestination)
    {
        SpecialAttackClientRpc(diveDestination);
    }
    [ServerRpc]
    private void SpecialAttackServerRpc(ulong networkObjectId)
    {
        SpecialAttackClientRpc(networkObjectId);
    }

    [ClientRpc]
    private void SpecialAttackClientRpc(Vector3 diveDestination)
    {
        CoroutineAction = LiftAndDiveCoroutine(diveDestination);
        player.coroutineInProcess = StartCoroutine(CoroutineAction);
    }

    [ClientRpc]
    private void SpecialAttackClientRpc(ulong networkObjectId)
    {
        CoroutineAction = LiftAndDiveCoroutine(networkObjectId);
        player.coroutineInProcess = StartCoroutine(CoroutineAction);
    }

    public IEnumerator LiftAndDiveCoroutine(Vector3 diveDestination)
    {
        liftHeight += diveDestination.y;

        Vector3 moveDirection = transform.forward;
        if (diveDestination == Vector3.zero)
            diveDestination = transform.position + transform.forward * 5f;

        Vector3 liftDestination = transform.position + new Vector3(0, liftHeight, 0) + transform.forward * 2f;
        if (isGentleCurve)
        {
            liftDestination = Vector3.Lerp(transform.position, diveDestination, 0.5f) + new Vector3(0, liftHeight, 0);

            liftDestination.y = Mathf.Lerp(transform.position.y, liftHeight, 0.5f);
        }
        Vector3 startPosition = transform.position;

        // 1️⃣ 공중으로 띄우기 (서서히 감속)
        float elapsedTime = 0f;
        float liftSpeed = initialLiftSpeed;

        // 도약 전 대기
        while (elapsedTime < waitTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        elapsedTime = 0f;

        // 도약 
        while (elapsedTime < liftTime)
        {
            float t = elapsedTime / liftTime;
            float easedT = EaseOutExpo(t); // Ease-Out 적용
            Vector3 currentPosition = Vector3.Lerp(startPosition, liftDestination, easedT);
            Vector3 moveStep = currentPosition - transform.position; // 현재 위치에서 이동할 거리
            controller.Move(moveStep); // 캐릭터 이동

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 도약 후 대기기
        elapsedTime = 0f;
        while (elapsedTime < holdTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        startPosition = transform.position;
        Vector3 totalDisplacement = diveDestination - startPosition; // 이동해야 할 거리
        Vector3 direction = totalDisplacement.normalized; // 이동 방향
        elapsedTime = 0f;

        while (elapsedTime < diveTime)
        {
            float t = elapsedTime / diveTime; // 현재 진행 비율 (0~1)
            float easedT = EaseOutExpo(t); // Ease-Out 적용

            Vector3 currentPosition = Vector3.Lerp(startPosition, diveDestination, easedT);
            Vector3 moveStep = currentPosition - transform.position; // 현재 위치에서 이동할 거리
            controller.Move(moveStep); // 캐릭터 이동

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    public IEnumerator LiftAndDiveCoroutine(ulong networkObjectId)
    {
        NetworkObject target = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];

        liftHeight += target.transform.position.y;

        Vector3 moveDirection = transform.forward;

        Vector3 liftDestination = transform.position + new Vector3(0, liftHeight, 0) + transform.forward * 2f;
        if (isGentleCurve)
        {
            liftDestination = Vector3.Lerp(transform.position, target.transform.position, 0.5f) + new Vector3(0, liftHeight, 0);

            liftDestination.y = Mathf.Lerp(transform.position.y, liftHeight, 0.5f);
        }
        Vector3 startPosition = transform.position;

        // 1️⃣ 공중으로 띄우기 (서서히 감속)
        float elapsedTime = 0f;
        float liftSpeed = initialLiftSpeed;

        // 도약 전 대기
        while (elapsedTime < waitTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        elapsedTime = 0f;

        // 도약하는 코드
        // 도약 중에 타겟을 향해 회전한다. 애니메이션 도중에 EnableCanRotate를 걸어준다.
        while (elapsedTime < liftTime)
        {
            float t = elapsedTime / liftTime;
            float easedT = EaseOutExpo(t); // Ease-Out 적용
            Vector3 currentPosition = Vector3.Lerp(startPosition, liftDestination, easedT);
            Vector3 moveStep = currentPosition - transform.position; // 현재 위치에서 이동할 거리
            controller.Move(moveStep); // 캐릭터 이동

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 도약 후 대기
        elapsedTime = 0f;
        while (elapsedTime < holdTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        startPosition = transform.position;
        Vector3 diveDestination = target.transform.position;
        Vector3 totalDisplacement = diveDestination - startPosition; // 이동해야 할 거리
        Vector3 direction = totalDisplacement.normalized; // 이동 방향
        elapsedTime = 0f;

        while (elapsedTime < diveTime)
        {
            float t = elapsedTime / diveTime; // 현재 진행 비율 (0~1)
            float easedT = EaseOutExpo(t); // Ease-Out 적용

            Vector3 currentPosition = Vector3.Lerp(startPosition, diveDestination, easedT);
            Vector3 moveStep = currentPosition - transform.position; // 현재 위치에서 이동할 거리
            controller.Move(moveStep); // 캐릭터 이동

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    private float EaseOutExpo(float t)
    {
        return t == 1 ? 1 : 1 - Mathf.Pow(2, -10 * t);
    }

    public void StopSpecialAttack()
    {
        if (CoroutineAction != null)
        {
            StopCoroutine(CoroutineAction);
        }
    }

}

