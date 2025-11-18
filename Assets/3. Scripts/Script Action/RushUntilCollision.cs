using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class RushUntilCollision : NetworkBehaviour
{
    [SerializeField] private bool onRush = false;
    [SerializeField] private float speed = 10f;
    [SerializeField] private Vector3 dir;
    [SerializeField] private bool hasCollision = false;
    [SerializeField] private bool hasCollisionWithGimmickObject = false;
    [SerializeField] DestructCollider rushDamageCollider;

    private CharacterManager character;
    public CharacterController controller;
    public IEnumerator CoroutineAction;
    float AnimationTime = 0.667f;
    float rushTime = 0f; // 돌진 시간 
    [SerializeField] float rushSpeed = 10f; // 대시 속도

    public Vector3 offset = new Vector3(0, 0, -2f); // 대시 후 이동할 위치를 정하기 위한 오프셋

    public bool OnRush { get => onRush; set => onRush = value; }
    public bool HasCollision { get => hasCollision; set => hasCollision = value; }
    public bool HasCollisionWithGimmickObject { get => hasCollisionWithGimmickObject; set => hasCollisionWithGimmickObject = value; }




    private void Awake()
    {
        character = GetComponent<CharacterManager>();
        controller = GetComponent<CharacterController>();
    }

    [ServerRpc]
    public void BossRushServerRpc(Vector3 destination)
    {
        character.characterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, "Boss_Dash_Start", true);

        BossRushClientRpc(destination);
    }

    [ClientRpc]
    private void BossRushClientRpc(Vector3 destination)
    {
        destination += transform.TransformDirection(offset);

        // 대시 중에 맞아 죽었을 때는? 코루틴을 취소시켜줘야 된다. 플레이어와 마찬가지로.
        CoroutineAction = DashTowardsPlayer(destination);

        StartCoroutine(CoroutineAction);
    }

    public IEnumerator DashTowardsPlayer(Vector3 destination)
    {
        Vector3 moveDirection = transform.forward;
        rushTime = (destination - transform.position).magnitude / rushSpeed; // 대시 시간 계산
        Debug.Log("rushTime: " + rushTime);
        // float poseCurTime = rushTime;
        float poseCurTime = 0f;
        Vector3 startPosition = transform.position;
        while (poseCurTime < rushTime)
        {

            // poseCurTime -= Time.deltaTime;
            poseCurTime += Time.deltaTime;

            // float t = 1 - (poseCurTime / rushTime);
            float t = poseCurTime / rushTime;
            float easedT = EaseOutExpo(t); // Ease-Out 적용
            Vector3 currentPosition = Vector3.Lerp(startPosition, destination, t);
            Vector3 moveStep = currentPosition - transform.position; // 현재 위치에서 이동할 거리
            controller.Move(moveStep); // 캐릭터 이동
            yield return null;
        }
        character.animator.SetTrigger("LoopEnd");

        yield return null;
    }
    private float EaseOutExpo(float t)
    {
        return t == 1 ? 1 : 1 - Mathf.Pow(2, -10 * t);
    }

    public void EnableCollisionWithPlayer()
    {
        int collisionLayer = LayerMask.NameToLayer("Character");
        Physics.IgnoreLayerCollision(collisionLayer, collisionLayer, false); // 충돌 활성화
    }

    public void DisableCollisionWithPlayer()
    {
        int collisionLayer = LayerMask.NameToLayer("Character");
        Physics.IgnoreLayerCollision(collisionLayer, collisionLayer, true); // 충돌 비활성화
    }


    private void Rush()
    {
        character.characterController.Move(dir * speed * Time.deltaTime);
    }


    public void RushAfterInitialization(Vector3 dir)
    {
        this.dir = dir;
        OnRush = true;
    }

    public void StopAction()
    {
        StopCoroutine(CoroutineAction);
    }


    public void OpenRushDamageCollider()
    {
        rushDamageCollider.EnableDamageCollider();
    }
    public void CloseRushDamageCollider()
    {
        rushDamageCollider.DisableDamageCollider();
    }
}

// 대전제: 공격 이후 자리잡기 확률 33%. 돌진, B, C 그룹 스택 +1씩 됨. 

// 처음 보스 시작할 때
// 백스텝 혹은 사이드 스텝을 밟고 A'' 공격
// A'' 공격을 못했을 때 A 공격

// A, A'' 공격을 한 이후 돌진 Y / N
// 돌진 Y : 확률적 돌진 이후 공격
// 돌진 N : 확률적 B 그룹 공격, C 그룹 스택 + 1
// B 그룹 공격 N : 확률적 C 그룹 공격, 

// 지금 어그로 튀는 건 최단 거리 기준. -> 다시 어그로를 랜덤으로 변경

// 1. 맨 처음 시작하면, 나한테 걸어오다가 일정 시간동안 걸었으면 거리가 멀다고 판단하고 내 앞까지 대시 후 공격
//    기획 의도 : 한 명에게 어그로가 너무 오랫동안 끌리는 것을 방지.
// 2. 걷기 최대 시간 이전에도 거리가 너무 멀어졌으면, 바로 대시 후 공격

// 3. 공격들을 쪼개서 공격 대시 후 공격. 이 때 대시 이전과 이후의 타겟이 변함. 대시는 뒤에 공격 타겟을 향함.
//   만약에 한 명만 있으면? 일정 거리 이상 떨어져 있으면 대시  
// 기획 의도 : 공격 중에 타겟을 바꿈. 

// 공격 중에 타겟을 바꿀지말지 판단. -> 타겟을 바꾸는 함수를 따로 애니메이션 도중에 실행? 

// 타겟을 바꿨으면 거리 계산 후 일정 범위 안이면 바로 공격, 아니면 대시 후 공격 ? 

// 판정, 이동, 공격, 자리잡기

// A공격, 돌진, B 공격, C 공격
