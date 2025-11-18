using Unity.Netcode;
using UnityEngine;

public class WarriorGolemPattern3PrisonChain : NetworkBehaviour
{
    public bool isHold;
    public Vector3 ChainPos;
    public PlayerManager GrabbedPlayer; 

    private void Update()
    {
        HoldingPlayerServerRpc();
    }

    [ServerRpc]
    public void ChainingServerRpc(Vector3 _ChainPos)
    {
        ChainingClientRpc(_ChainPos);
    }


    [ClientRpc]
    public void ChainingClientRpc(Vector3 _ChainPos)
    {
        ChainPos = _ChainPos;
        isHold = true;
    }

    [ServerRpc]
    public void GrabPlayerServerRpc(ulong pid)
    {
        GrabPlayerClientRpc(pid);
    }


    [ClientRpc]
    public void GrabPlayerClientRpc(ulong pid)
    {
        GrabbedPlayer = NetworkManager.Singleton.SpawnManager.SpawnedObjects[pid].GetComponent<PlayerManager>();
    }

    [ServerRpc]
    public void HoldingPlayerServerRpc()
    {
        HoldingPlayerClientRpc();
    }

    [ClientRpc]
    public void HoldingPlayerClientRpc()
    {
        if (!isHold)
        {
            if (GrabbedPlayer != null)
            {
                transform.position = GrabbedPlayer.transform.position + Vector3.up;
            }
        }
        else
        {
            if (GrabbedPlayer != null)
            {
                transform.position = ChainPos + Vector3.up * 4f;
                GrabbedPlayer.characterController.Move(transform.position - GrabbedPlayer.transform.position);
            }
        }
    }
}
