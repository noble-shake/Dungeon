using Unity.Netcode;
using UnityEngine;

public class ArcherSynergyShot : MonoBehaviour
{
    [SerializeField] float LifeTime = 20f;

    private void Update()
    {
        LifeTime -= Time.deltaTime;
        if (LifeTime <= 0f) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<AICharacterManager>(out AICharacterManager target))
        {
            // Boss RPC 요청.
            target.HitFromVineShotServerRpc();
            Destroy(gameObject);
        }
        else
        {
            return;
        }
    }
}