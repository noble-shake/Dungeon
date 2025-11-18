using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WorldObjectManager : MonoBehaviour
{
    public static WorldObjectManager instance;


    [Header("Network Objects")]
    public List<NetworkObjectSpawner> networkObjectSpawner = new();

    [Header("Fog Walls")]
    public List<FogWallInteractable> fogWalls;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SpawnObject(NetworkObjectSpawner networkObjectSpawner)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            this.networkObjectSpawner.Add(networkObjectSpawner);
            networkObjectSpawner.AttempToSpawnObject();
        }
    }

    public void AddFogWallToList(FogWallInteractable fogWall)
    {
        if (!fogWalls.Contains(fogWall))
        {
            fogWalls.Add(fogWall);
        }
    }

    public void RemoveFogWallFromList(FogWallInteractable fogWall)
    {
        if (fogWalls.Contains(fogWall))
        {
            fogWalls.Remove(fogWall);
        }
    }
}
