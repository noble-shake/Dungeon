using Unity.Netcode;
using UnityEngine;

public class MultipleShotSystem : NetworkBehaviour
{
    [HideInInspector] private PlayerManager playerManager;
    [SerializeField] private float standardAngle;
    [SerializeField] public ShotVFX shotVFX;
    [SerializeField] private float projectileSpeed;

    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    [ServerRpc]
    public void MultiShotServerRpc(Vector3 Direction, Quaternion Rot)
    {
        MultiShotClientRpc(Direction, Rot);
    }

    [ClientRpc]
    public void MultiShotClientRpc(Vector3 Direction, Quaternion Rot)
    {
        transform.rotation = Rot;
        Vector3 playPos = playerManager.playerEquipmentManager.LeftWeaponManager.transform.position;


        for (int idx = 0; idx < 5; idx++)
        {
            Vector3 direction = Quaternion.Euler(0f, standardAngle + 10f * idx, 0f) * (Direction - playPos);
            RaycastHit[] hitInfo = Physics.RaycastAll(playPos, direction, 20f); // Fdamagable
            Debug.DrawRay(playPos, direction, Color.blue, 3f);
            ShotVFX shotProjectile = Instantiate(shotVFX);
            shotProjectile.transform.position = playPos;
            shotProjectile.ArrowSpeed = projectileSpeed;
            shotProjectile.transform.rotation = Quaternion.LookRotation(Direction - playPos) * Quaternion.Euler(0f, standardAngle + 10f * idx, 0f);
            foreach (var hit in hitInfo)
            {
                Debug.Log(hit.collider.gameObject.name);
                Debug.Log(hit.collider.gameObject.layer);
                if (hit.collider == null)
                {

                    continue;

                }
                else
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Damagable Character"))
                    {
                        AICharacterManager monster = hit.collider.gameObject.GetComponentInParent<AICharacterManager>();
                        if (monster != null) playerManager.playerEquipmentManager.LeftWeaponManager?.damageCollider.RayTrigger(monster);
                    }


                }

            }
        }
    }

    public void OnMultiShot()
    {
        if (!IsOwner) return;

        Vector3 targetDirection = Vector3.zero;
        targetDirection = Camera.main.transform.forward;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
            targetDirection = transform.forward;

        Debug.Log("how much run?");
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        
        // Trace와는 별개로 Ray를 쏜다.
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2));
        Vector3 ScreenToPoint = ray.GetPoint(20f);

        MultiShotServerRpc(ScreenToPoint, targetRotation);



       


    }
}
