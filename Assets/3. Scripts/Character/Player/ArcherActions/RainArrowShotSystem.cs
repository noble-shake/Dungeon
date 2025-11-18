using Unity.Netcode;
using UnityEngine;

public class RainArrowShotSystem : NetworkBehaviour
{
    [HideInInspector] private PlayerManager playerManager;
    [SerializeField] private ArcherRainZone RainZoneIndicatorPrefab;
    [SerializeField] private ArcherRainExecutor RainZoneExecutorPrefab;
    [SerializeField] private ArcherRainZone RainZoneObject;
    [SerializeField] private float circleSpeed;
    public bool isShotReady;
    // Indicator

    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    public void OnIndicatorHit()
    {
        playerManager.animator.speed = 1f;
        if (IsOwner)
        {
            RainZeonCreateServerRpc(RainZoneObject.transform.position);
            Destroy(RainZoneObject.gameObject);
        } 
        isShotReady = false;
    }

    [ServerRpc]
    public void RainZeonCreateServerRpc(Vector3 pos)
    {
        RainZoneCreateClientRpc(pos);
    }

    [ClientRpc]
    public void RainZoneCreateClientRpc(Vector3 pos)
    {
        var rainObject = Instantiate(RainZoneExecutorPrefab);
        rainObject.Init(gameObject);
        rainObject.transform.position = pos;
    }

    public void OnShotReady()
    {

        playerManager.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.SpecialAttack, "Main_Special_Attack_02", true, canRotate: false, canMove: false);


    }

    public void OnShotDisplay()
    {
        playerManager.animator.speed = 0f;
        isShotReady = true;
        if (!IsOwner) return;
        if (RainZoneObject != null) return;
        RainZoneObject = Instantiate(RainZoneIndicatorPrefab);
        RainZoneObject.transform.position = transform.position + transform.forward * 4f;
        RainZoneObject.transform.rotation = transform.rotation;
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (isShotReady && RainZoneObject != null)
        {
            playerManager.isPerformingAction = true;
            Vector2 circleVector = InputManager.instance.movementInput;
            Vector3 actualVector = transform.rotation * new Vector3(circleVector.x, 0f, circleVector.y).normalized;
            RainZoneObject.transform.position += actualVector * circleSpeed * Time.deltaTime;


        }
    }
}
