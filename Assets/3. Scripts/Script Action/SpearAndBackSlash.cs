using UnityEngine;
using System.Collections;
using UnityEngine.TextCore.Text;

public class SpecialAttackAction_SpearAndBackSlash : MonoBehaviour
{
    private PlayerManager player;
    // private Shockwave shockwave;
    public Vector3 divePosition;
    public CharacterController controller;
    public IEnumerator CoroutineAction;
    float AnimationTime = 3.8f;
    float poseReadyTime = 0.18f; // 찌르기 직전
    float poseTime = 0.5f; // 찌르기 직전
    float spearTime = 0.6f; // 찌르기
    float termTime = 0.49f; // 멈칫
    float SlashTime = 0.26f;// 휘두르기

    private void Awake()
    {
        player = GetComponent<PlayerManager>();
        controller = GetComponent<CharacterController>();
    }

    public void SpecialAttackActionSpearAndBackSlash(Vector3 _enemyPos)
    {
        CoroutineAction = SpearAndSlashCoroutine(_enemyPos);
        player.coroutineInProcess = StartCoroutine(CoroutineAction);
    }

    public IEnumerator SpearAndSlashCoroutine(Vector3 _enemyPos)
    {
        Debug.Log("Special Action Coroutine Work!");
        Vector3 moveDirection = transform.forward;
        if (_enemyPos == Vector3.zero)
            _enemyPos = transform.position + transform.forward * 4f;

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

        poseCurTime = spearTime;
        Vector3 startPosition = transform.position;
        while (poseCurTime > 0f)
        {

            poseCurTime -= Time.deltaTime;

            float t = 1 - (poseCurTime / spearTime);
            float easedT = EaseOutExpo(t); // Ease-Out 적용
            Vector3 currentPosition = Vector3.Lerp(startPosition, _enemyPos, easedT);
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

        poseCurTime = SlashTime;
        while (poseCurTime > 0f)
        {

            poseCurTime -= Time.deltaTime;

            // 2바퀴 돌리고 싶다.
            Vector3 mvTerm = -moveDirection * Mathf.Abs(Mathf.Sin(360f * (1 - poseCurTime / 2))) * 4f * Time.deltaTime;
            controller.Move(mvTerm); // Cyclick Func
            yield return null;
        }

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
