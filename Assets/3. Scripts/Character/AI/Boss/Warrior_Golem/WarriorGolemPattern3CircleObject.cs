using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Purchasing;

public enum CircleObjectType
{ 
    Red,
    Blue,
    Yellow,
    Purple,
}

public class WarriorGolemPattern3CircleObject : ObjectItem
{
    private ulong owner;
    private PlayerManager player;
    float curTime;
    [SerializeField] private float earnDelay;
    public CircleObjectType CurCircleType;
    public List<ParticleSystem> CircleOrbs;
    public int CircleOrbsIdx;
    public Vector3 DropPos;
    public int tossValueIdx; // Host Hase this, then Clients will get this Value with RPC
    public int ownerIndex;

    public ParticleSystem CurrentOrb;
    // private Rigidbody rb;
    IEnumerator boundEffect;
    private void Start()
    {
        // rb = GetComponent<Rigidbody>();
        objectItemType = ObjectItemType.MagicCircle;
    }

    public void SetObject(int idx)
    {
        CurrentOrb = CircleOrbs[idx];
        CurCircleType = (CircleObjectType)idx;
        CircleOrbsIdx = idx;
        tossValueIdx = idx;
    }

    [ServerRpc]
    public void SetObjectServerRpc(int idx)
    {
        SetObjectClientRpc(idx);
    }

    [ClientRpc]
    public void SetObjectClientRpc(int idx)
    {
        CurrentOrb = CircleOrbs[idx];
        CurCircleType = (CircleObjectType)idx;
        CircleOrbsIdx = idx;
        tossValueIdx = idx;
    }

    public void OnOrbTriggered(Collider other)
    {
        if (player != null) return;
        if (curTime > 0f) return;
        if (other.GetComponent<PlayerManager>() == null) return;

        player = other.GetComponent<PlayerManager>();
        if (player.playerInventoryManager.circleObject != null)
        {
            player = null;
            curTime = earnDelay;
            owner = ulong.MaxValue;
            return;
        } 

        player.playerInventoryManager.circleObject = this;
        curTime = earnDelay;
        owner = player.PlayerID;
    }

    public void Drop()
    {
        ItemBound();
    }

    private void Update()
    {
        curTime -= Time.deltaTime;
        if (curTime < 0f)
        {
            curTime = 0f;
        }

        if (player != null)
        {
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<NetworkTransform>().transform.position = new Vector3(player.transform.position.x, 2f, player.transform.position.z);
        }
    }

    public void ItemBound()
    {
        GetComponent<Rigidbody>().useGravity = true;
        if (player != null)
        {
            GetComponent<NetworkTransform>().transform.position = player.transform.position + Vector3.up * 2;
        }
        else
        {
            GetComponent<NetworkTransform>().transform.position = DropPos + Vector3.up * 2;
        }

        player = null;
        owner = ulong.MaxValue;
        curTime = earnDelay;

        Vector3 BoundVelocity = new Vector3(0f, 10f, 0f);

        GetComponent<Rigidbody>().AddForce(BoundVelocity, ForceMode.Impulse);

    }

    [ServerRpc]
    public void CircleObjectPosSetServerRpc(int idx, Vector3 HitPos)
    {
        CircleObjectPosSetClientRpc(idx, HitPos);
    }

    [ClientRpc]
    public void CircleObjectPosSetClientRpc(int idx, Vector3 HitPos)
    {
        gameObject.SetActive(true);
        SetObject(idx);

        foreach (ParticleSystem orb in CircleOrbs)
        {
            if (orb.gameObject.activeSelf) orb.gameObject.SetActive(false);
        }

        CurrentOrb.gameObject.SetActive(true);

        DropPos = HitPos;
        Drop();
    }


}
