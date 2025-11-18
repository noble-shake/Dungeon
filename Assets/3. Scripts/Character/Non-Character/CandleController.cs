using System;
using Unity.Netcode;
using UnityEngine;

public class CandleController : NetworkBehaviour, IDestructable
{

    [SerializeField] private string destructAnimation = "";

    [SerializeField] private GameObject FlamePrefab; // Looping Particle.
    [SerializeField] private GameObject MagicCirclePrefab;
    [SerializeField] private GameObject originalPrefab;

    private void Awake()
    {

    }

    private void FlameOn()
    { 
        FlamePrefab.SetActive(true);
    }

    /// <summary>
    /// 바위 등의 오브젝트가 파괴될 때 실행되어야 하는 함수
    /// </summary>
    public void BeingDestructed()
    {
        RemoveCandleServerRpc();
    }

    public string GetDestructAnimation()
    {
        return destructAnimation;
    }

    [ServerRpc]
    public void ActivateCandleServerRPC()
    {
        ActivateCandleClientRPC();

    }

    [ClientRpc]
    private void ActivateCandleClientRPC()
    {
        FlamePrefab.SetActive(true);
    }


    // 어차피 바위 오브젝트는 서버 오브젝트라 
    [ServerRpc]
    public void RemoveCandleServerRpc()
    {
        RemoveCandleClientRpc();

    }

    [ClientRpc]
    private void RemoveCandleClientRpc()
    {
        if (IsOwner)
        {
            Destroy(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
    }

    public bool isGimmickObject()
    {
        // throw new NotImplementedException();
        return true;
    }
}
