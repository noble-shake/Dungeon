using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class WarriorGolemPattern3Prison : NetworkBehaviour
{
    public WarriorGolemPattern3Component owner;
    public GameObject DestructableObjectPrefab;
    public List<WarriorGolemPattern3PrisonObject> DestructableObjects;
    public List<Transform> DestructablePostion;
    List<Collider> Colliders;
    public int OriginObjectHP = 400;

    private void Awake()
    {
        Colliders = GetComponentsInChildren<Collider>().ToList();
    }

    public void OnTransmitable(bool isOn)
    {
        foreach (Collider coll in Colliders)
        {
            coll.enabled = isOn;
        }
    }

    public void DestroyHappened()
    {
        ClearCheck();
    }

    public void ClearCheck()
    {
        bool ClearCheck = true;
        foreach (WarriorGolemPattern3PrisonObject _object in DestructableObjects)
        {
            if (_object.isDestructed == false)
            {
                ClearCheck = false;
            }
        }

        if (ClearCheck == false) return;

        // Phase Clear

        owner.PrisonPatternBreaked = true;
        owner.ResetAll();
    }

    public void CleanRemained()
    {
        foreach (WarriorGolemPattern3PrisonObject _object in DestructableObjects)
        {
            _object.isDestructed = false;
            _object.RemoveServerRpc();
        }

        DestructableObjects = new List<WarriorGolemPattern3PrisonObject>();
    }

    //[ServerRpc]
    //public void PrisonObjectEffectServerRpc()
    //{
    //    PrisonObjectEffectClientRpc();
    //    foreach (Transform t in DestructablePostion)
    //    {
    //        WarriorGolemPattern3PrisonObject prisonObject = ObjectPoolManager.Singleton.GetObject(DestructableObjectPrefab).GetComponent<WarriorGolemPattern3PrisonObject>();
    //        prisonObject.GetComponent<NetworkObject>().Spawn();
    //        prisonObject.isDestructed = false;
    //        prisonObject.Prison = this;
    //        prisonObject.transform.position = t.position;
    //        prisonObject.MaxHP.Value = OriginObjectHP;
    //        DestructableObjects.Add(prisonObject);
    //    }
    //}

    //[ClientRpc]
    //public void PrisonObjectEffectClientRpc()
    //{
    //    OnTransmitable(false);

    //}

    [ServerRpc]
    public void PrisonTurnOnServerRpc()
    {
        PrisonTurnOnClientRpc();
    }

    [ClientRpc]
    public void PrisonTurnOnClientRpc()
    {
        Debug.Log("씨발 제발");
        OnTransmitable(true);
        GetComponent<ParticleSystem>().Play();

    }

    [ServerRpc]
    public void setPrisonCompServerRpc()
    {
        setPrisonCompClientRpc();
    }

    [ClientRpc]
    public void setPrisonCompClientRpc()
    {
        
    }

}
