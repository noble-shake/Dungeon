using System;
using Unity.Netcode;
using UnityEngine;

public class RockController : NetworkBehaviour, IDestructable
{

    [SerializeField] private string destructAnimation = "";

    [SerializeField] private ParticleController particleController;
    [SerializeField] private GameObject originalPrefab;
    [SerializeField] private bool gimmickObject = false;

    private void Awake()
    {
        particleController = GetComponent<ParticleController>();
    }

    public void RiseRock()
    {
        // particleController.PlayParticleAndStop().Forget();
    }

    /// <summary>
    /// 바위 등의 오브젝트가 파괴될 때 실행되어야 하는 함수
    /// </summary>
    public void BeingDestructed()
    {
        // particleController.RemoveParticle();
        Debug.Log("얘 왜 두번 실행 됨?");
        RemoveParticleServerRpc();
    }

    public string GetDestructAnimation()
    {
        return destructAnimation;
    }

    // 어차피 바위 오브젝트는 서버 오브젝트라 
    [ServerRpc]
    public void RemoveParticleServerRpc()
    {
        RemoveParticleClientRpc();

    }

    [ClientRpc]
    private void RemoveParticleClientRpc()
    {
        // particleController.RemoveParticle();
        if (IsOwner)
        {
            Debug.Log("실행된건가 이거");
            // gameObject.GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        }
    }

    // 맨 처음 바위를 스폰만 시킴.
    public override void OnNetworkSpawn()
    {
        // particleController.PlayParticleAndStop().Forget();
    }

    public bool isGimmickObject()
    {
        return gimmickObject;
    }
}
