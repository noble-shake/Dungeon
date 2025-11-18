using DTT.AreaOfEffectRegions;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class FirstBossGimmickFunc : NetworkBehaviour
{
    AIWarriorGolemCharacterManager boss;
    private UnityAction bossGimmickAction;

    Vector3 targetDirection;


    // 특정 시점 이후로 인디케이터를 활성화하고, 인디케이터의 각도를 상시 플레이어에게 유지시킨다. 

    // 특정 시간이 경과하면 바로 돌진 애니메이션 실행하고

    // 무언가에 부딪칠 때까지 이동한다. 

    // 이동할 때에는 이제 벽 또는 기둥에 부딪칠 수 있으며

    // 벽에 부딪치면 그냥 충돌 애니메이션 실행

    // 기둥에 부딪치면 기믹 파훼 표시해주고(UI), 충돌 애니메이션 실행, 기둥 제거

    // 그럼 충돌은 어떻게 측정? 데미져블 콜라이더로 생성하는게 어떨까...

    // 

    [SerializeField] private bool rushButton = false;
    [SerializeField] private bool turnButton = false;
    [SerializeField] private bool continuousTurnButton = false;
    [SerializeField] private float walkMotion = 0.2f;
    [SerializeField] private float rotationSpeed = 20f;
    private bool timeToAttack = false;
    private bool HaveAttcked = false;

    public NetworkVariable<bool> indicatorActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> indicatorFillProgress = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<Vector3> targetTransform = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] public LineRegion indicator;

    private void Awake()
    {
        boss = GetComponent<AIWarriorGolemCharacterManager>();
    }

    void Start()
    {
        indicator.transform.SetParent(indicator.transform.parent, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (indicatorActive.Value == true)
        {
            indicator.Length = Vector3.Distance(targetTransform.Value, transform.position);

        }
        if (throwButton)
        {
            ThrowProjectiles();
            throwButton = false;
        }
    }

    // 타겟이 된 위치로 달려간다.

    // 타겟을 향해 회전한다.
    // 

    // 달려가다가 특정 거리 안에 들어오면 휘두르는 공격 애니메이션을 실행한다.

    public void SetRotateTowardsPlayerAction()
    {
        bossGimmickAction = RotateTowardsPlayer;
    }

    private void RotateTowardsPlayer()
    {
        CharacterManager player = boss.aiCharacterCombatManager.currentTarget;
        if (player == null) return;
        Vector3 targetDirection = player.transform.position - boss.transform.position;
        targetDirection.y = 0f;
        targetDirection.Normalize();
        Vector3 currentForward = boss.transform.forward;

        // 왼쪽 또는 오른쪽 회전 판단
        Vector3 crossProduct = Vector3.Cross(currentForward, targetDirection);
        float direction = crossProduct.y;

        if (direction > 0)
        {
            Debug.Log("오른쪽으로 회전 중");
        }
        else if (direction < 0)
        {
            Debug.Log("왼쪽으로 회전 중");
        }
        else
        {
            Debug.Log("정확히 정면을 바라봄 (회전 없음)");
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        boss.transform.rotation = Quaternion.Slerp(boss.transform.rotation, targetRotation, 10 * Time.deltaTime);
    }

    public void SetRushAction()
    {
        HaveAttcked = false;
        bossGimmickAction = Rush;

    }

    public void RotateByAngle(GameObject gameObject)
    {
        // 만약의 경우 중복 실행이 안 되게끔
        if (boss.isPerformingAction) return;

        Vector3 forward = boss.transform.forward;
        forward.y = 0; // Y축 영향 제거 (수평 방향만 고려)
        forward.Normalize();

        Vector3 toTarget = gameObject.transform.position - boss.transform.position;
        toTarget.y = 0; // Y축 영향 제거
        toTarget.Normalize();

        float angle = Mathf.Atan2(Vector3.Cross(forward, toTarget).y, Vector3.Dot(forward, toTarget)) * Mathf.Rad2Deg;

        // 결과 출력
        if (angle >= -45 && angle <= 45)
        {
        }
        else if (angle > 45 && angle <= 135)
        {
            boss.aiCharacterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, "Turn_R90", true);
        }
        else if (angle < -45 && angle >= -135)
        {
            boss.aiCharacterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, "Turn_L90", true);
        }
        else if ((angle > 135 && angle <= 225) || (angle < -135 && angle >= -225))
        {
            boss.aiCharacterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, "Turn_R180", true);
        }
    }

    public void Rush()
    {
        CharacterManager player = boss.aiCharacterCombatManager.currentTarget;
        if (player == null) return;
        if (timeToAttack) return;

        // 달려가면서 방향을 수정하기 위함. 
        Vector3 targetDirection = player.transform.position - boss.transform.position;
        targetDirection.y = 0f;
        targetDirection.Normalize();
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        boss.transform.rotation = Quaternion.Slerp(boss.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        Vector3 destination = player.transform.position;
        // 달려감.
        boss.aiCharacterAnimatorManager.UpdateAnimatorMovementParameters(0, walkMotion, false);

        if (Vector3.Distance(boss.transform.position, player.transform.position) < 3f)
        {
            timeToAttack = true;
        }
    }

    public void SetAttackAction()
    {
        bossGimmickAction = Attack;
    }

    public void Attack()
    {
        if (HaveAttcked) return;
        HaveAttcked = true;
        boss.aiCharacterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Attack, "Attack_DashAtk02", true);
    }

    public override void OnNetworkSpawn()
    {
        indicatorActive.OnValueChanged += OnIndicatorActive;
        indicatorFillProgress.OnValueChanged += OnIndicatorFillProgressChanged;
    }

    private void OnIndicatorActive(bool previousValue, bool newValue)
    {
        if (newValue == true)
        {
            indicator.gameObject.SetActive(true);
        }
        else
        {
            indicator.gameObject.SetActive(false);
        }
    }

    private void OnIndicatorFillProgressChanged(float oldValue, float newValue)
    {
        if (!IsOwner)
        {
            indicator.FillProgress = newValue;
        }
    }

    public GameObject grenadePrefab; // 수류탄 프리팹
    public Transform firePoint; // 발사 위치 기준 (캐릭터 중심)
    public float spawnRadius = 1.5f; // 발사 위치 반경
    public float spawnHeight = 1.5f;
    public int minGrenades = 3; // 최소 발사 개수
    public int maxGrenades = 6; // 최대 발사 개수
    public float minForce = 5f; // 최소 발사 세기
    public float maxForce = 15f; // 최대 발사 세기
    public float minAngle = 10f; // 최소 발사 각도 (위로 향하는 각도)
    public float maxAngle = 80f; // 최대 발사 각도

    public bool throwButton = false;

    public void ThrowProjectiles()
    {
        int grenadeCount = Random.Range(minGrenades, maxGrenades + 1);
        for (int i = 0; i < grenadeCount; i++)
        {
            ThrowSingleGrenade();
        }
    }

    private void ThrowSingleGrenade()
    {
        float angle = Random.Range(0f, 360f);
        Vector3 spawnPosition = firePoint.position + new Vector3(Mathf.Cos(angle) * spawnRadius, spawnHeight, Mathf.Sin(angle) * spawnRadius);

        GameObject grenade = Instantiate(grenadePrefab, spawnPosition, Quaternion.identity);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float force = Random.Range(minForce, maxForce);
            float launchAngle = Random.Range(minAngle, maxAngle);

            // 발사 방향을 firePoint에서 spawnPosition을 향하는 방향으로 설정 (xz 평면 기준)
            Vector3 horizontalDirection = (spawnPosition - firePoint.position).normalized;
            horizontalDirection.y = 0; // y 축 영향 제거하여 xz 평면에서만 방향 유지
            Vector3 axis = Vector3.Cross(horizontalDirection, Vector3.up); // 회전 축 계산
            Vector3 finalDirection = Quaternion.AngleAxis(launchAngle, axis) * horizontalDirection;

            // 위쪽 각도를 추가하여 최종 방향 계산
            // Vector3 finalDirection = Quaternion.Euler(launchAngle, 0, 0) * horizontalDirection;

            rb.AddForce(finalDirection * force, ForceMode.Impulse);
        }
    }
}
