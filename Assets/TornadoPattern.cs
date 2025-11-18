using DG.Tweening;
using DTT.AreaOfEffectRegions;
using System.Collections;
using System.Collections.Generic;
// using System.Numerics;
using Unity.Netcode;
using UnityEngine;

public class TornadoPattern : MonoBehaviour
{
    AIWarriorGolemCharacterManager boss;

    private bool isPatternSuccess;
    public bool IsPatternSuccess { get => isPatternSuccess; set => isPatternSuccess = value; }


    public CharacterController controller;
    public IEnumerator CoroutineAction;
    float AnimationTime = 2.4f; // 참고용. 실제로 쓰진 않음. 
    public float poseReadyTime = 0f; // 찌르기 직전
    public float poseTime = 0.2f; // 찌르기 직전
    public float dashTime = 0.6f; // 찌르기
    public float termTime = 0f; // 멈칫
    public float SlashTime = 0f;// 휘두르기

    [SerializeField] private float pushSpeed = 5f;
    [SerializeField] private float pushDuration = 3f;

    // 보스가 중앙에서부터 얼마만큼의 거리로 이동할지지
    [SerializeField] private float zDistance = 20f;

    // 영혼 VFX가 플레이어 캐릭터의 어느 정도 높이에서 생성될지
    [SerializeField] private float soulVFXHeight = 1.5f;
    // 보스의 정면에서 어느 정도 위치에 에너지 차징 VFX가 생성될지 정함.
    [SerializeField] private Vector3 energyVFXOffset = new Vector3(0, 1.5f, 0);

    // -Z축?
    [SerializeField] private Vector3 pushDirection = -Vector3.forward;

    // 데이터 가지고 있다가 랜덤으로 추출해서 실행행
    public LineIndexData[] lineDataList;
    private int previousIndex = -1;

    // 회오리 생성 횟수
    public int tornadoCount = 100;
    public float indicatorOnTime = 2f;
    public Vector3 yDirection = Vector3.up * 0.1f;

    // 

    [Header("소환된 마법진과 벽")]
    [SerializeField] private List<MagicCircleObject> magicCircleList = new List<MagicCircleObject>();
    [SerializeField] private GameObject magicWall;
    [SerializeField] GameObject chargeObject;
    void Awake()
    {
        boss = GetComponent<AIWarriorGolemCharacterManager>();
        controller = GetComponent<CharacterController>();
    }

    public bool test = false;

    void Update()
    {
        if (test == true)
        {
            test = false;
        }

        if (boss.IsOwner)
        {
            CheckPatternEnd();
        }
    }

    private void CheckPatternEnd()
    {
        if (magicCircleList.Count != 0 && !isPatternSuccess)
        {
            int doneCount = 0;
            foreach (MagicCircleObject magicCircle in magicCircleList)
            {
                if (magicCircle.Done == true)
                {
                    doneCount++;
                    break;
                }
            }
            if (doneCount == magicCircleList.Count)
            {
                // 모든 마법진이 완료되면 패턴 종료
                isPatternSuccess = true;
            }
        }
    }

    [ServerRpc]
    public void GenerateAndLaunchTornadoServerRpc()
    {
        // 무조건 이전과 같은 토네이도도 패턴이 안되게끔 서버 쪽에서 어떤 인덱스를 쓸껀지 전파한다. 
        int currentIndex = Random.Range(0, lineDataList.Length);
        while (currentIndex == previousIndex)
        {
            if (lineDataList.Length == 1)
                break;
            currentIndex = Random.Range(0, lineDataList.Length);
        }
        previousIndex = currentIndex;
        GenerateAndLaunchTornadoClientRpc(currentIndex);
    }

    [ClientRpc]
    private void GenerateAndLaunchTornadoClientRpc(int currentIndex)
    {
        // 데이터 읽고
        LineIndexData lineData = lineDataList[currentIndex];

        GameObject verticalStart = TornadoPatternSettingManager.instance.verticalStart;
        GameObject verticalEnd = TornadoPatternSettingManager.instance.verticalEnd;
        // ShowIndicatorFromSavedData(verticalStart, verticalEnd, lineData.verticalStartIndices, lineData.verticalEndIndices);
        ShootTornadoFromSavedData(verticalStart, verticalEnd, lineData.verticalStartIndices, lineData.verticalEndIndices);

        GameObject horizontalStart = TornadoPatternSettingManager.instance.horizontalStart;
        GameObject horizontalEnd = TornadoPatternSettingManager.instance.horizontalEnd;
        // ShowIndicatorFromSavedData(horizontalStart, horizontalEnd, lineData.horizontalStartIndices, lineData.horizontalEndIndices);
        ShootTornadoFromSavedData(horizontalStart, horizontalEnd, lineData.horizontalStartIndices, lineData.horizontalEndIndices, half: true);
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
            indicatorVFX.transform.position = startChild.position + yDirection; // 시작 위치에 VFX 소환
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

    [ServerRpc]
    public void PrepareTornadoPatternServerRpc(int playerCount)
    {
        PrepareTornadoPatternClientRpc(playerCount);
    }

    [ClientRpc]
    private void PrepareTornadoPatternClientRpc(int playerCount)
    {
        // 마법벽 설치하고
        PrepareTornadoPattern(playerCount);
        // 인원수에 따라서 마법진 설치 1,2,3,4개

    }

    private void PrepareTornadoPattern(int playerCount)
    {
        Vector3 magicWallPosition = TornadoPatternSettingManager.instance.magicWallTransform.position;
        magicWall = ObjectPoolManager.Singleton.GetObject(ObjectPoolType.FirstBoss, 9);
        magicWall.transform.position = magicWallPosition + yDirection; // y축으로 약간 위로 올리기

        Vector3 leftEnd = TornadoPatternSettingManager.instance.magicCircleTransformList.GetChild(0).position;
        Vector3 rightEnd = TornadoPatternSettingManager.instance.magicCircleTransformList.GetChild(1).position;
        GameObject magicCircle = null;
        switch (playerCount)
        {
            case 1:
                magicCircle = ObjectPoolManager.Singleton.GetObject(ObjectPoolType.FirstBoss, 10);
                magicCircle.transform.position = Vector3.Lerp(leftEnd, rightEnd, 0.5f) + yDirection;
                magicCircleList.Add(magicCircle.GetComponent<MagicCircleObject>());
                break;
            case 2:
            case 3:
            case 4:
                for (int i = 0; i < playerCount; i++)
                {
                    Vector3 magicCirclePosition = Vector3.Lerp(leftEnd, rightEnd, (float)i / (playerCount - 1)); // 플레이어 수에 따라서 마법진 위치 조정
                    magicCircle = ObjectPoolManager.Singleton.GetObject(ObjectPoolType.FirstBoss, 10);
                    magicCircle.transform.position = magicCirclePosition + yDirection; // y축으로 약간 위로 올리기
                    magicCircleList.Add(magicCircle.GetComponent<MagicCircleObject>());
                }
                break;
            default:
                Debug.LogError("Invalid player count: " + playerCount);
                break;
        }


    }

    [ServerRpc]
    public void EndTornadoPatternServerRpc()
    {
        EndTornadoPatternClientRpc();
    }

    [ClientRpc]
    private void EndTornadoPatternClientRpc()
    {
        // 마법벽 설치하고
        EndTornadoPattern();
        // 인원수에 따라서 마법진 설치 1,2,3,4개
    }

    private void EndTornadoPattern()
    {

    }

    [ServerRpc]
    public void JumpToWallServerRpc(Vector3 destination)
    {
        Vector3 targetDirection = new Vector3(0, 0, 1);
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        boss.transform.DORotateQuaternion(targetRotation, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            // 회전이 완료된 후에 점프를 시작합니다.
            // JumpToWallClientRpc(destination);
            Debug.Log("회전 완료");
        });
        // boss.transform.rotation = Quaternion.Slerp(boss.transform.rotation, targetRotation, Time.deltaTime * 10f);
        JumpToWallClientRpc(destination);
    }

    private void JumpToWallClientRpc(Vector3 destination)
    {
        boss.bossAnimatorManager.PlayAnimationOnNetwork(AnimationType.Locomotion, "jump", true, applyRootmotion: true);
        StartCoroutine(DashCoroutine(destination + new Vector3(0, 0, zDistance)));
    }

    [ServerRpc]
    public void PlacePlayersTowardsWallServerRpc()
    {
        PlacePlayersTowardsWallClientRpc();
    }

    [ClientRpc]
    private void PlacePlayersTowardsWallClientRpc()
    {
        PlayerManager player = InputManager.instance.player;
        if (player == null)
            return;

        StartCoroutine(Push(player, Vector3.forward, 20f, 2f));
    }

    private IEnumerator Push(PlayerManager player, Vector3 direction, float speed, float duration)
    {
        CharacterController controller = player.characterController;

        float timer = 0f;
        Vector3 velocity = direction.normalized * speed;

        while (timer < duration)
        {
            controller.Move(velocity * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
    }

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
    public void ExtractSoulFromPlayersServerRpc()
    {
        ExtractSoulFromPlayersClientRpc();
        // 플레이어들 캐릭터 모두에서 영혼 모양의의 VFX를 발생시키고 뽑아온다.
    }

    [ClientRpc]
    private void ExtractSoulFromPlayersClientRpc()
    {
        // 플레이어들 캐릭터 모두에서 영혼 모양의의 VFX를 발생시키고 뽑아온다.
        PlayerManager[] players = FindObjectsByType<PlayerManager>(FindObjectsSortMode.None);
        foreach (PlayerManager player in players)
        {
            if (player == null)
                continue;

            // 플레이어들 위치 다 찾기
            // VFX가 OnEnable 되면서 크기가 점점 커지며 위치로 이동.
            Vector3 playerSoulVfxPosition = player.transform.position + Vector3.up * soulVFXHeight; // 플레이어에서 살짝 위에에
            Vector3 bossEnergyPosition = boss.transform.position + energyVFXOffset; // 보스의 위치 + 에너지 VFX 오프셋
            Vector3 direction = (playerSoulVfxPosition - bossEnergyPosition).normalized; // 플레이어와 보스의 방향 벡터
            float distance = Vector3.Distance(playerSoulVfxPosition, bossEnergyPosition); // 플레이어와 보스의 거리

            Vector3 midPoint = (playerSoulVfxPosition + bossEnergyPosition) / 2 + Vector3.up * 2f;

            // VFX 소환
            GameObject soulVFXPrefab = ObjectPoolManager.Singleton.GetPrefab(ObjectPoolType.FirstBoss, 0);
            GameObject soulVFX = ObjectPoolManager.Singleton.GetObject(ObjectPoolType.FirstBoss, 0);
            soulVFX.transform.position = playerSoulVfxPosition; // 

            // 두트윈을 사용해서 soulVFX를 bossEnergyPosition으로 이동시키기
            soulVFX.transform
            .DOPath(new Vector3[] { playerSoulVfxPosition, midPoint, bossEnergyPosition }, 2f, PathType.CatmullRom)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                // 이동이 끝난 후 VFX를 풀로 반환
                ObjectPoolManager.Singleton.ReturnObject(soulVFX, soulVFXPrefab);
            });

        }
        // 플레이어들 위치 다 찾기
        // VFX 소환
        // 각각의 VFX를 boss의 위치로 보내기
        // 두트윈을 써서 궤적 생성 
    }

    private void ShootTornadoFromSavedData(GameObject startObj, GameObject endObj, List<int> startIndexList, List<int> endIndexList, bool half = false)
    {
        if (startObj == null || endObj == null || startIndexList.Count == 0 || endIndexList.Count == 0)
        {
            Debug.LogWarning("저장된 인덱스 데이터가 없습니다.");
            return;
        }
        float randomTime = 0f;
        if (half == true)
        {
            for (int i = 0; i < startIndexList.Count / 2; i++)
            {
                int startIndex = startIndexList[i];
                int endIndex = endIndexList[i];

                if (startIndex >= startObj.transform.childCount || endIndex >= endObj.transform.childCount) continue;

                Transform startChild = startObj.transform.GetChild(startIndex);
                Transform endChild = endObj.transform.GetChild(endIndex);

                randomTime = Random.Range(0f, 0.2f); // 랜덤한 시간 설정

                GameObject indicatorVFXPrefab = ObjectPoolManager.Singleton.GetPrefab(ObjectPoolType.FirstBoss, 12);
                GameObject indicatorVFX = ObjectPoolManager.Singleton.GetObject(ObjectPoolType.FirstBoss, 12);
                indicatorVFX.TryGetComponent(out LineRegion lineRegion);
                indicatorVFX.transform.position = startChild.position + yDirection; // 시작 위치에 VFX 소환
                Vector3 direction = endChild.position - startChild.position; // 시작 위치에서 끝 위치로 향하는 방향 벡터
                lineRegion.Angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                lineRegion.Length = Vector3.Distance(startChild.position, endChild.position); // 길이 설정

                // 두트윈으로 tornadoVFX를 endChild.position으로 이동시키기
                DOVirtual.DelayedCall(1f, () =>
                {
                    ObjectPoolManager.Singleton.ReturnObject(indicatorVFX, indicatorVFXPrefab);
                });



                GameObject tornadoVFXPrefab = ObjectPoolManager.Singleton.GetPrefab(ObjectPoolType.FirstBoss, 8);
                GameObject tornadoVFX = ObjectPoolManager.Singleton.GetObject(ObjectPoolType.FirstBoss, 8);
                tornadoVFX.transform.position = startChild.position; // 시작 위치에 VFX 소환
                                                                     // 두트윈으로 tornadoVFX를 endChild.position으로 이동시키기
                DOVirtual.DelayedCall(1 + randomTime, () => tornadoVFX.transform

                    .DOMove(endChild.position, 3) // 1초 미만 동안 이동
                                                  // .SetDelay(Random.Range(0f, 1f)) // 랜덤한 지연 시간
                    .SetEase(Ease.Linear) // 부드러운 Ease-In-Out 효과
                    .OnComplete(() =>
                    {
                        // 이동이 끝난 후 VFX를 풀로 반환
                        ObjectPoolManager.Singleton.ReturnObject(tornadoVFX, tornadoVFXPrefab);
                    }));

                Debug.DrawLine(startChild.position, endChild.position, Color.blue, 3f); // 기존과 다른 색상 (파란색)
            }
            for (int i = startIndexList.Count / 2; i < startIndexList.Count; i++)
            {
                int startIndex = startIndexList[i];
                int endIndex = endIndexList[i];

                if (startIndex >= startObj.transform.childCount || endIndex >= endObj.transform.childCount) continue;

                Transform endChild = startObj.transform.GetChild(startIndex);
                Transform startChild = endObj.transform.GetChild(endIndex);
                randomTime = Random.Range(0f, 0.2f); // 랜덤한 시간 설정

                GameObject indicatorVFXPrefab = ObjectPoolManager.Singleton.GetPrefab(ObjectPoolType.FirstBoss, 12);
                GameObject indicatorVFX = ObjectPoolManager.Singleton.GetObject(ObjectPoolType.FirstBoss, 12);
                indicatorVFX.TryGetComponent(out LineRegion lineRegion);
                indicatorVFX.transform.position = startChild.position + yDirection; // 시작 위치에 VFX 소환
                Vector3 direction = endChild.position - startChild.position; // 시작 위치에서 끝 위치로 향하는 방향 벡터
                lineRegion.Angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                lineRegion.Length = Vector3.Distance(startChild.position, endChild.position); // 길이 설정

                // 두트윈으로 tornadoVFX를 endChild.position으로 이동시키기
                DOVirtual.DelayedCall(1, () =>
                {
                    ObjectPoolManager.Singleton.ReturnObject(indicatorVFX, indicatorVFXPrefab);
                });


                GameObject tornadoVFXPrefab = ObjectPoolManager.Singleton.GetPrefab(ObjectPoolType.FirstBoss, 8);
                GameObject tornadoVFX = ObjectPoolManager.Singleton.GetObject(ObjectPoolType.FirstBoss, 8);
                tornadoVFX.transform.position = startChild.position; // 시작 위치에 VFX 소환
                                                                     // 두트윈으로 tornadoVFX를 endChild.position으로 이동시키기

                DOVirtual.DelayedCall(1 + randomTime, () => tornadoVFX.transform

                    .DOMove(endChild.position, 3) // 2초 동안 이동
                                                  // .SetDelay(Random.Range(0f, 1f)) // 랜덤한 지연 시간
                    .SetEase(Ease.Linear) // 부드러운 Ease-In-Out 효과
                    .OnComplete(() =>
                    {
                        // 이동이 끝난 후 VFX를 풀로 반환
                        ObjectPoolManager.Singleton.ReturnObject(tornadoVFX, tornadoVFXPrefab);
                    }));

                Debug.DrawLine(startChild.position, endChild.position, Color.blue, 3f); // 기존과 다른 색상 (파란색)
            }

        }
        else
        {

            for (int i = 0; i < startIndexList.Count; i++)
            {
                int startIndex = startIndexList[i];
                int endIndex = endIndexList[i];

                if (startIndex >= startObj.transform.childCount || endIndex >= endObj.transform.childCount) continue;

                Transform startChild = startObj.transform.GetChild(startIndex);
                Transform endChild = endObj.transform.GetChild(endIndex);
                randomTime = Random.Range(0f, 0.2f); // 랜덤한 시간 설정

                GameObject indicatorVFXPrefab = ObjectPoolManager.Singleton.GetPrefab(ObjectPoolType.FirstBoss, 12);
                GameObject indicatorVFX = ObjectPoolManager.Singleton.GetObject(ObjectPoolType.FirstBoss, 12);
                indicatorVFX.TryGetComponent(out LineRegion lineRegion);
                indicatorVFX.transform.position = startChild.position + yDirection; // 시작 위치에 VFX 소환
                Vector3 direction = endChild.position - startChild.position; // 시작 위치에서 끝 위치로 향하는 방향 벡터
                lineRegion.Angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                lineRegion.Length = Vector3.Distance(startChild.position, endChild.position); // 길이 설정

                // 두트윈으로 tornadoVFX를 endChild.position으로 이동시키기
                DOVirtual.DelayedCall(1, () =>
                {
                    ObjectPoolManager.Singleton.ReturnObject(indicatorVFX, indicatorVFXPrefab);
                });

                GameObject tornadoVFXPrefab = ObjectPoolManager.Singleton.GetPrefab(ObjectPoolType.FirstBoss, 8);
                GameObject tornadoVFX = ObjectPoolManager.Singleton.GetObject(ObjectPoolType.FirstBoss, 8);
                tornadoVFX.transform.position = startChild.position; // 시작 위치에 VFX 소환
                                                                     // 두트윈으로 tornadoVFX를 endChild.position으로 이동시키기
                DOVirtual.DelayedCall(1 + randomTime, () => tornadoVFX.transform

                    .DOMove(endChild.position, 3) // 2초 동안 이동
                                                  // .SetDelay(Random.Range(0f, 1f)) // 랜덤한 지연 시간
                    .SetEase(Ease.Linear) // 부드러운 Ease-In-Out 효과
                    .OnComplete(() =>
                    {
                        // 이동이 끝난 후 VFX를 풀로 반환
                        ObjectPoolManager.Singleton.ReturnObject(tornadoVFX, tornadoVFXPrefab);
                    }));

                Debug.DrawLine(startChild.position, endChild.position, Color.blue, 3f); // 기존과 다른 색상 (파란색)
            }
        }
    }

    [ServerRpc]
    public void StartChargingEnergyServerRpc()
    {
        StartChargingEnergyClientRpc();
    }

    [ClientRpc]
    private void StartChargingEnergyClientRpc()
    {
        // 보스의 에너지를 차징하는 VFX를 재생합니다.
        chargeObject = ObjectPoolManager.Singleton.GetObject(ObjectPoolType.FirstBoss, 11);
        chargeObject.transform.SetParent(gameObject.transform); // 보스의 자식으로 설정
        chargeObject.transform.localPosition = new Vector3(0.07814886f, 2.06757f, 0.1471845f); // 보스의 위치 + 에너지 VFX 오프셋

        Vector3 startScale = chargeObject.transform.localScale;
        Vector3 targetScale = startScale * 9f;

        chargeObject.transform.DOScale(targetScale, 30f).SetEase(Ease.Linear);
    }

    public void SetHpPenalty(float limit)
    {
        GameManager.Instance.SetLimitOnMaxHpPenalty(limit); // 10% 감소
    }

    [ServerRpc]
    public void CleanPatternObjectsServerRpc()
    {
        CleanPatternObjectsClientRpc();
    }

    [ClientRpc]
    private void CleanPatternObjectsClientRpc()
    {
        // 마법진과 마법벽을 제거합니다.
        if (magicWall != null)
        {
            ObjectPoolManager.Singleton.ReturnObject(magicWall, ObjectPoolManager.Singleton.GetPrefab(ObjectPoolType.FirstBoss, 9));
            magicWall = null;
        }

        foreach (MagicCircleObject magicCircle in magicCircleList)
        {
            if (magicCircle != null)
            {
                ObjectPoolManager.Singleton.ReturnObject(magicCircle.gameObject, ObjectPoolManager.Singleton.GetPrefab(ObjectPoolType.FirstBoss, 10));
            }
        }
        magicCircleList.Clear();

        if (chargeObject != null)
            ObjectPoolManager.Singleton.ReturnObject(chargeObject, ObjectPoolManager.Singleton.GetPrefab(ObjectPoolType.FirstBoss, 11));

    }
}
