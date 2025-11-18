using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SpecialAttackAction_Dash : NetworkBehaviour
{
    private PlayerManager player;
    // private Shockwave shockwave;
    public Vector3 divePosition;
    public CharacterController controller;
    public IEnumerator CoroutineAction;
    float AnimationTime = 2.4f; // 참고용. 실제로 쓰진 않음. 
    public float poseReadyTime = 0f; // 찌르기 직전
    public float poseTime = 0.2f; // 찌르기 직전
    public float dashTime = 0.6f; // 찌르기
    public float termTime = 0f; // 멈칫
    public float SlashTime = 0f;// 휘두르기

    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, 0f); // 대쉬할 때의 offset

    private void Awake()
    {
        player = GetComponent<PlayerManager>();
        controller = GetComponent<CharacterController>();
    }

    public void SpecialAttackActionDash(Vector3 enemyPosition)
    {
        // offsetDistance만큼 가까워진 위치 계산
        if (enemyPosition != Vector3.zero)
        {
            enemyPosition = enemyPosition + transform.TransformDirection(offset);
        }

        CoroutineAction = DashCoroutine(enemyPosition);
        player.coroutineInProcess = StartCoroutine(CoroutineAction);
    }

    public IEnumerator DashCoroutine(Vector3 enemyPosition)
    {
        Debug.Log("Special Action Coroutine Work!");
        Vector3 moveDirection = transform.forward;
        if (enemyPosition == Vector3.zero)
            enemyPosition = transform.position + transform.forward * 4f;

        float poseCurTime = poseReadyTime;
        while (poseCurTime > 0f)
        {
            poseCurTime -= Time.deltaTime;
            yield return null;
        }

        yield return null;

        poseCurTime = poseTime;
        while (poseCurTime > 0f)
        {
            poseCurTime -= Time.deltaTime;
            Vector3 mvTerm = -moveDirection * Mathf.Pow(5, poseTime) * Time.deltaTime;
            controller.Move(mvTerm); // slightly backmoving.

            yield return null;
        }

        yield return null;

        poseCurTime = dashTime;
        Vector3 startPosition = transform.position;
        while (poseCurTime > 0f)
        {

            poseCurTime -= Time.deltaTime;

            float t = 1 - (poseCurTime / dashTime);
            float easedT = EaseOutExpo(t); // Ease-Out 적용
            Vector3 currentPosition = Vector3.Lerp(startPosition, enemyPosition, easedT);
            Vector3 moveStep = currentPosition - transform.position; // 현재 위치에서 이동할 거리
            controller.Move(moveStep); // 캐릭터 이동
            yield return null;
        }

        yield return null;

        poseCurTime = termTime;
        while (poseCurTime > 0f)
        {

            poseCurTime -= Time.deltaTime;
            yield return null;

        }

        yield return null;
    }

    public void StopSpecialAttack()
    {
        if (CoroutineAction != null)
        {
            StopCoroutine(CoroutineAction);
        }
    }

    private float EaseOutExpo(float t)
    {
        return t == 1 ? 1 : 1 - Mathf.Pow(2, -10 * t);
    }

}
