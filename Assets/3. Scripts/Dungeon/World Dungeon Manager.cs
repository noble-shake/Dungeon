using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDungeonManager : MonoBehaviour
{
    public static WorldDungeonManager instance;

    [Header("Dungeon Objects")]
    public List<DungeonObject> dungeonObjects;

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

    private void Start()
    {
        for (int i = 0; i < dungeonObjects.Count; i++)
        {
            dungeonObjects[i].dungeonIndex = i;
        }
    }

    // public WeaponAction GetWeaponItemActionByID(int ID)
    // {
    // return dungeonObjects.IndexOf(ID);
    // }

    public int GetDungeonCount()
    {
        return dungeonObjects.Count;
    }

    public DungeonObject GetDungeonObjectByIndex(int index)
    {
        return dungeonObjects[index];
    }
}
