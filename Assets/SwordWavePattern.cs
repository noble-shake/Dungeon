using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SwordWavePattern : NetworkBehaviour
{
    private AIBossCharacterManager boss;


    public bool patternStarted = false;
    [SerializeField] private float rotationTime;

    [SerializeField] private int swordWaveCount;

    [SerializeField] private int parriedWaveHitCount;
    [SerializeField] private int parriedWaveHitCountThreshold;

    [SerializeField] private int sealCount;


    [SerializeField] private int groggyTime;

    [SerializeField] private int waitAfterAttackTime;

    [SerializeField] PlayerManager targetPlayer;

    public string[] swordWaveAnimationNames;

    [SerializeField] int[] sealCountByPlayerNumber;
    [SerializeField] int[] swordWaveCountByPlayerNumber;
    [SerializeField] int[] parriedSwordWaveHitCountByPlayerNumber;


    public float RotationTime { get => rotationTime; set => rotationTime = value; }
    public int SwordWaveCount { get => swordWaveCount; set => swordWaveCount = value; }
    public int ParryiedWaveHitCount { get => parriedWaveHitCount; set => parriedWaveHitCount = value; }
    public int GroggyTime { get => groggyTime; set => groggyTime = value; }
    public int WaitAfterAttackTime { get => waitAfterAttackTime; set => waitAfterAttackTime = value; }
    public PlayerManager TargetPlayer { get => targetPlayer; set => targetPlayer = value; }
    public int ParryiedWaveHitCountThreshold { get => parriedWaveHitCountThreshold; set => parriedWaveHitCountThreshold = value; }

    [Header("Debug Menu")]
    public bool jump = false;
    public bool applyRootmotion = false;
    // 점프 뛸 중앙의 mapCenter는 게임매니저가 갖고 있다. 

    void Awake()
    {
        boss = GetComponent<AIBossCharacterManager>();
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (jump == true)
        {
            jump = false;
            // boss.bossAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, "jump", true, applyRootmotion: applyRootmotion);
            JumpToCenterServerRpc(Vector3.zero);
        }
    }

    [ServerRpc]
    public void JumpToCenterServerRpc(Vector3 destination)
    {
        JumpToCenterClientRpc(destination);

    }

    [ClientRpc]
    private void JumpToCenterClientRpc(Vector3 destination)
    {
        boss.characterLocomotionManager.useGravity = false;
        StartCoroutine(ParabolicDiveCoroutine(GameManager.Instance.GetCenterOfBossRoom()));
        if (boss.IsOwner)
            boss.bossAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, "Jump_Start", true, applyRootmotion: applyRootmotion);
    }

    public CharacterController controller;
    public IEnumerator CoroutineAction;
    float AnimationTime = 2.4f; // 참고용. 실제로 쓰진 않음. 
    public float poseReadyTime = 0f; // 찌르기 직전
    public float poseTime = 0.2f; // 찌르기 직전
    public float dashTime = 0.6f; // 찌르기
    public float termTime = 0f; // 멈칫
    public float SlashTime = 0f;// 휘두르기

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
    [SerializeField] private float endTime = 0.9f;

    // VFX : 도약할 때, 그리고 착지할 때
    // 도약할 땐 원본, 착지할 땐 수정본 VFX

    // 추후 VFX 매니저로 뺄 수 있음. 
    public void PlayJumpVFX(int index)
    {
        GameObject jumpVFXPrefab = ObjectPoolManager.Singleton.GetPrefab(ObjectPoolType.FirstBoss, index);
        GameObject jumpVFX = ObjectPoolManager.Singleton.GetObject(ObjectPoolType.FirstBoss, index);
        jumpVFX.transform.position = transform.position;
        jumpVFX.TryGetComponent(out ParticleSystem particleSystem);
        if (particleSystem != null)
        {
            particleSystem.Play();
            ObjectPoolManager.Singleton.WaitAndReturnVFX(jumpVFX, jumpVFXPrefab).Forget();
        }
        else
        {
            Debug.LogError("ParticleSystem이 없습니다.");
        }
    }
    // 검기 맞을 때 VFX
    // 맞은 다음의 추가 효과?
    // F를 길게 눌러 아군 조준 기능.
    // 스킬 Q,E로 옮기기
    // UI + 궁수 개발 
    // public void 

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

            // liftDestination.y = Mathf.Lerp(transform.position.y, liftHeight, 0.5f);
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
        boss.characterLocomotionManager.useGravity = true;
        boss.animator.SetTrigger("LoopEnd");
    }

    public IEnumerator ParabolicDiveCoroutine(Vector3 diveDestination)
    {
        // if (diveDestination == Vector3.zero)
        //     diveDestination = transform.position + transform.forward * 5f;

        Vector3 startPosition = transform.position;

        // 궤적의 최고점 높이
        float peakHeight = liftHeight + Mathf.Max(startPosition.y, diveDestination.y);

        float elapsedTime = 0f;

        // 대기 시간
        while (elapsedTime < waitTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;

        bool almostDone = false;

        // 포물선 이동
        while (elapsedTime < diveTime)
        {
            float t = elapsedTime / diveTime;
            float easedT = EaseOutExpo(t);

            Vector3 currentPos = GetParabolaPoint(startPosition, diveDestination, peakHeight, t);
            Vector3 moveStep = currentPos - transform.position;

            controller.Move(moveStep);

            if (t > endTime && !almostDone)
            {
                almostDone = true;
                boss.animator.SetTrigger("LoopEnd");
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }


        // 도착 후 정리
        boss.characterLocomotionManager.useGravity = true;
        // boss.animator.SetTrigger("LoopEnd");
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

    public IEnumerator DashCoroutine(Vector3 destination)
    {
        Vector3 moveDirection = transform.forward;


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

        // poseCurTime = dashTime;
        poseCurTime = 0;
        Vector3 startPosition = transform.position;
        while (poseCurTime < dashTime)
        {

            poseCurTime += Time.deltaTime;

            // float t = 1-(poseCurTime / dashTime);
            float t = (poseCurTime / dashTime);
            // float easedT = EaseOutExpo(t); // Ease-Out 적용
            Vector3 currentPosition = Vector3.Lerp(startPosition, destination, t);
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
    private float EaseOutExpo(float t)
    {
        return t == 1 ? 1 : 1 - Mathf.Pow(2, -10 * t);
    }

    [ServerRpc]
    public void SetTargetAndRotateServerRpc()
    {
        while (true)
        {
            int randomIndex = UnityEngine.Random.Range(0, GameManager.Instance.connectedPlayerList.Count);
            PlayerManager player = GameManager.Instance.connectedPlayerList[randomIndex];
            if (player != null && player.isDead.Value != true && !player.playerNetworkManager.isSealed.Value)
            {
                SetTargetAndRotateClientRpc(player.NetworkObjectId);
                break;
            }
        }
    }

    [ClientRpc]
    private void SetTargetAndRotateClientRpc(ulong networkObjectID)
    {
        PlayerManager[] playerList = FindObjectsByType<PlayerManager>(FindObjectsSortMode.None);
        foreach (var player in playerList)
        {
            if (player.NetworkObjectId == networkObjectID)
            {
                StartCoroutine(RotateTowardsTarget(player.transform, rotationTime));
            }
        }
    }

    IEnumerator RotateTowardsTarget(Transform target, float time)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / time;
            t = 1 - (1 - t) * (1 - t); // Ease-Out 적용
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }

        transform.rotation = targetRotation;
    }

    [ServerRpc]
    public void AllKillServerRpc()
    {
        AllKillClientRpc();
        Debug.Log("모두가 죽었습니다.");
    }

    [ClientRpc]
    private void AllKillClientRpc()
    {
        // 내가 다루는 playerManager를 알기 위해 인풋에 연결된 애를 갖다 씀.
        InputManager.instance.player.KillCharacter();
    }

    public void InitializeByPlayerNumber()
    {
        int connectedPlayerNumber = NetworkManager.Singleton.ConnectedClients.Count;

        // NOTE : 인원수에 따라 난이도를 세팅해준다.

        // 검기 애니메이션을 몇 번 실행할 지 초기화해준다.
        swordWaveCount = swordWaveCountByPlayerNumber[connectedPlayerNumber - 1];
        // 검기를 몇 번 맞아야 패턴이 파훼되는지를 초기화해준다.
        parriedWaveHitCountThreshold = parriedSwordWaveHitCountByPlayerNumber[connectedPlayerNumber - 1];
        // 봉인 플레이어의 수를 초기화해준다.
        sealCount = sealCountByPlayerNumber[connectedPlayerNumber - 1];
        StartPatternServerRpc();
    }

    [ServerRpc]
    private void StartPatternServerRpc()
    {
        StartPatternClientRpc();
    }

    [ClientRpc]
    private void StartPatternClientRpc()
    {
        patternStarted = true;
    }

    // 이 함수는 랜덤으로 애니메이션 두 개 중 하나를 실행한다.
    public void PlaySwordWaveAnimation()
    {
        int randomIndex = UnityEngine.Random.Range(0, swordWaveAnimationNames.Length);
        boss.bossAnimatorManager.PlayAnimationOnNetwork(AnimationType.Attack, swordWaveAnimationNames[randomIndex], true, transitionNomalizedTime: 0);
    }

    [ServerRpc]
    public void SetIconServerRpc()
    {
        SetIconClientRpc(parriedWaveHitCountThreshold);
    }

    [ClientRpc]
    private void SetIconClientRpc(int count)
    {
        HUD_UIManager.instance.combatUIManager.SetSwordWaveIcon(count);
    }

    [ServerRpc]
    public void RemoveIconServerRpc()
    {
        RemoveIconClientRpc();
    }

    [ClientRpc]
    private void RemoveIconClientRpc()
    {
        HUD_UIManager.instance.combatUIManager.RemoveSwordWaveIcon();
    }
}
