using Cinemachine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    public PlayerManager player;
    public CinemachineVirtualCamera casualCam;
    public CinemachineVirtualCamera lockOnCam;
    public CinemachineFramingTransposer transposer;
    public WatchCameraTarget watchingCameraTarget;

    [Header("락온 카메라의 거리에 따른 높이 설정")]
    [SerializeField] private float minDistance = 5f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float minHeight = 0.3f;
    [SerializeField] private float maxHeight = 0.6f;

    [Header("Archer Camera")]
    public CinemachineVirtualCamera AimLockOnCam;

    // 얘가 있어야 카메라가 같이 안 도는 듯?
    public GameObject CinemachineCameraTarget; // 시네머신 카메라가 팔로우하는 객체. 플레이어의 자식 오브젝트이지만 
    private float cinemachineTargetYaw; // 카메라가 y축 기준으로 얼마나 회전했는지에 대한 변수. 수평 인풋에 대응한다.
    private float cinemachineTargetPitch; // 카메라가 z축 기준으로 얼마나 회전했는지에 대한 변수. 수직 인풋에 대응한다.

    private bool isPlayerSpawned = false;

    [Tooltip("카메라를 얼마나 위로 올릴 수 있는지에 대한 값")]
    public float TopClamp = 70.0f;

    [Tooltip("카메라를 얼마나 아래로 내릴 수 있는지에 대한 값")]
    public float BottomClamp = -30.0f;

    [Tooltip("값 이하의 카메라 움직임은 무시한다.")]
    [SerializeField] private float horizontalThreshold = 0.01f;
    [SerializeField] private float verticalThreshold = 0.01f;

    [Header("Lock On")]
    [SerializeField] private float lockOnRadius = 20f;
    [SerializeField] private float minimumViewableAngle = -50;
    [SerializeField] private float maximumViewableAngle = 50;
    [SerializeField] private float maximumLockOnDistance = 50;

    private List<CharacterManager> availableTargets = new List<CharacterManager>();
    public CharacterManager nearestLockOnTarget;
    public CharacterManager leftLockOnTarget;
    public CharacterManager rightLockOnTarget;

    // 기본적으로 카메라는 몬스터만 조준할 수 있게끔 세팅이 되어있어야 한다. 
    [SerializeField] private List<CharacterType> lockOnTargetTypes = new List<CharacterType>();
    [SerializeField] private float lerpSpeed = 1f;
    [SerializeField] private bool dynamicHeightChange = true;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
    }

    private void Start()
    {
        transposer = lockOnCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        lockOnTargetTypes.Add(CharacterType.Monster);
    }

    private void LateUpdate()
    {
        if (player == null) return;
        if (InputManager.instance.active == false) return;
        CameraMove();
    }

    public void SetTargetTypeAll()
    {
        lockOnTargetTypes.Clear();
        lockOnTargetTypes.Add(CharacterType.Player);
        lockOnTargetTypes.Add(CharacterType.Monster);
    }

    public void SetTargetType(CharacterType targetType)
    {
        lockOnTargetTypes.Clear();
        lockOnTargetTypes.Add(targetType);
    }

    private void CameraMove()
    {
        if (player.IsSpawned == false) return;

        if (isPlayerSpawned == false)
        {
            cinemachineTargetYaw = player.transform.rotation.eulerAngles.y;
            cinemachineTargetPitch = player.transform.rotation.eulerAngles.x + 20;
            isPlayerSpawned = true;
        }
        // 카메라가 락온되면? 
        // 1. 원래 내 가상 카메라를 정해진 위치로 옮긴다.
        // 2. 내 뒤에 있는 락온용 카메라로 전환한다. 
        if (player.playerNetworkManager.isLockedOn.Value)
        {
            // 가상카메라
            // if (lockOnCam.LookAt == null) return;
            if (player.characterClass == PlayerClass.Archer)
            {
                {
                    // 다른 기기를 지원할 때 주석 해제할 것.
                    // float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
                    float deltaTimeMultiplier = 1.0f;

                    cinemachineTargetYaw += InputManager.instance.cameraInput.x * deltaTimeMultiplier;
                }
                // if (Math.Abs(InputManager.instance.cameraInput.y) >= verticalThreshold)
                {
                    float deltaTimeMultiplier = 1.0f;

                    cinemachineTargetPitch += InputManager.instance.cameraInput.y * deltaTimeMultiplier;
                }
            }

        }
        else
        {
            // if (Math.Abs(InputManager.instance.cameraInput.x) >= horizontalThreshold)
            {
                // 다른 기기를 지원할 때 주석 해제할 것.
                // float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
                float deltaTimeMultiplier = 1.0f;

                cinemachineTargetYaw += InputManager.instance.cameraInput.x * deltaTimeMultiplier;
            }
            // if (Math.Abs(InputManager.instance.cameraInput.y) >= verticalThreshold)
            {
                float deltaTimeMultiplier = 1.0f;

                cinemachineTargetPitch += InputManager.instance.cameraInput.y * deltaTimeMultiplier;
            }
        }
        cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);

        // 카메라를 회전시키는 것이 아닌, 카메라가 바라보고 있는 대상을 회전시키는 것임. 
        // 마우스는 가만히 두고 캐릭터만 회전할 때, 카메라의 각도를 유지하게끔 항상 코드가 실행되고 있음. 
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch, cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    public void InitializeCamera(PlayerManager player)
    {
        // 플레이어의 카메라 타겟 객체 각자의 
        this.player = player;
        CinemachineCameraTarget = player.casualFollowTransform;

        casualCam.Follow = player.casualFollowTransform.transform;
        casualCam.LookAt = player.transform;
        casualCam.Priority = 2;

        lockOnCam.Follow = player.lockOnFollowTransform.transform;
        lockOnCam.LookAt = player.transform;
        lockOnCam.Priority = 1;

        AimLockOnCam.Follow = player.casualFollowTransform.transform;
        AimLockOnCam.LookAt = player.transform;
        AimLockOnCam.Priority = 0;

        // CinemachineCameraTarget.transform.rotation = Quaternion.LookRotation(player.transform.forward);
    }

    public void LockOn(bool lockOn)
    {
        int tempPriority = casualCam.Priority;
        casualCam.Priority = lockOnCam.Priority;
        lockOnCam.Priority = tempPriority;
        if (lockOn)
        {
            lockOnCam.LookAt = player.playerCombatManager.currentTarget.transform;
        }
        else
        {
            lockOnCam.LookAt = null;
            cinemachineTargetYaw = ClampAngle(player.transform.rotation.eulerAngles.y, float.MinValue, float.MaxValue);
        }
    }

    public void AimOn()
    {
        int tempPriority = casualCam.Priority;
        casualCam.Priority = AimLockOnCam.Priority;
        AimLockOnCam.Priority = tempPriority;
    }

    public void ClearLockOnTargets()
    {
        nearestLockOnTarget = null;
        leftLockOnTarget = null;
        rightLockOnTarget = null;
        availableTargets.Clear();
    }

    public bool FindLockOnTargets()
    {
        float shortestDistance = Mathf.Infinity;
        float shortestDistacneOfRightTarget = Mathf.Infinity;
        float shortestDistanceOfLeftTarget = -Mathf.Infinity;

        Collider[] colliders = Physics.OverlapSphere(player.transform.position, lockOnRadius, WorldUtilityManager.instance.GetCharacterLayers());

        // 락온 가능한 대상들을 찾는다.
        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager lockOnTarget = colliders[i].GetComponent<CharacterManager>();
            if (lockOnTarget == null)
            {
                continue;
            }
            bool isValidTarget = false;
            foreach (CharacterType type in lockOnTargetTypes)
            {
                if (lockOnTarget.characterGroup == type)
                {
                    isValidTarget = true;
                    break;
                }
            }
            if (isValidTarget == false)
                continue;

            if (lockOnTarget != null)
            {
                Vector3 lockOnTargetDirection = lockOnTarget.transform.position - player.transform.position;
                float distanceFromTarget = Vector3.Distance(player.transform.position, lockOnTarget.transform.position);
                float viewableAngle = Vector3.Angle(lockOnTargetDirection, Camera.main.transform.forward);

                if (lockOnTarget.isDead.Value)
                    continue;

                if (lockOnTarget.transform.root == player.transform.root)
                    continue;

                if (distanceFromTarget > maximumLockOnDistance) continue;

                if (viewableAngle > minimumViewableAngle && viewableAngle < maximumViewableAngle)
                {
                    RaycastHit hit;

                    if (Physics.Linecast(player.playerCombatManager.lockOnTransform.position, lockOnTarget.characterCombatManager.lockOnTransform.position,
                    out hit, WorldUtilityManager.instance.GetEnvironmentLayers()))
                    {
                        continue;
                    }
                    else
                    {
                        availableTargets.Add(lockOnTarget);
                    }
                }
            }
        }

        // 락온 가능한 대상들 중에 가장 가까운거, 왼쪽에 있는거, 오른쪽에 있는거 선별함.
        for (int k = 0; k < availableTargets.Count; k++)
        {
            if (availableTargets[k] != null)
            {
                float distanceFromTarget = Vector3.Distance(player.transform.position, availableTargets[k].transform.position);

                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargets[k];
                }

                if (player.playerNetworkManager.isLockedOn.Value)
                {
                    Vector3 relativeEnemyPosition = player.transform.InverseTransformPoint(availableTargets[k].transform.position);

                    var distanceFromLeftTarget = relativeEnemyPosition.x;
                    var distanceFromRightTarget = relativeEnemyPosition.x;

                    if (availableTargets[k] == player.playerCombatManager.currentTarget)
                        continue;
                    if (relativeEnemyPosition.x <= 0.00 && distanceFromLeftTarget > shortestDistanceOfLeftTarget)
                    {
                        shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                        leftLockOnTarget = availableTargets[k];
                    }
                    else if (relativeEnemyPosition.x >= 0.00 && distanceFromRightTarget < shortestDistacneOfRightTarget)
                    {
                        shortestDistacneOfRightTarget = distanceFromRightTarget;
                        rightLockOnTarget = availableTargets[k];
                    }
                }
            }
        }

        if (nearestLockOnTarget)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 부활했을 땐 다시 나에게 돌려놔야 함. 
    public void ChangeCameraFollow(PlayerManager player)
    {
        // 
        watchingCameraTarget.SetTransform(player);
        casualCam.Follow = watchingCameraTarget.transform;
        casualCam.LookAt = player.transform;
        CinemachineCameraTarget = watchingCameraTarget.gameObject;
        // casualCam.Follow = player.casualFollowTransform.transform;
    }

    public void ChangeCameraFollowToSelf()
    {
        Debug.Log("부활할 때 여기를 안 거치나?");
        watchingCameraTarget.SetTransform(null);

        GameManager.Instance.cameraChange.currentWatchingIndex = GameManager.Instance.cameraChange.myIndex;
        casualCam.Follow = player.casualFollowTransform.transform;
        CinemachineCameraTarget = player.casualFollowTransform;
    }

    public void UpdateCameraHeight(float distance)
    {
        if (dynamicHeightChange == false) return;
        // 부드럽게 
        Vector3 currentOffset = transposer.m_TrackedObjectOffset;
        float targetHeight;

        // 거리에 따라 높이 계산
        if (distance <= minDistance)
        {
            targetHeight = minHeight;
        }
        else if (distance >= maxDistance)
        {
            targetHeight = maxHeight;
        }
        else
        {
            // 거리가 minDistance, maxDistance 사이일 때 선형 보간
            targetHeight = Mathf.Lerp(0.3f, 0.6f, (distance - minDistance) / (maxDistance - minDistance));
        }

        // 부드럽게 높이 변경
        Vector3 targetOffset = new Vector3(currentOffset.x, targetHeight, currentOffset.z);
        currentOffset = Vector3.Slerp(currentOffset, targetOffset, Time.deltaTime * lerpSpeed);
        transposer.m_TrackedObjectOffset = currentOffset;
    }
}
