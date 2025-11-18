using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldItemDatabase : MonoBehaviour
{
    public static WorldItemDatabase instance;

    [Header("Weapons")]
    [SerializeField] List<Weapon> weapons = new List<Weapon>();

    [Header("Items")]
    private List<Item> items = new List<Item>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        foreach (var weapon in weapons)
        {
            items.Add(weapon);
        }

        for (int i = 0; i < items.Count; i++)
        {
            items[i].itemID = i;
        }
    }

    public Weapon GetWeaponByID(int ID)
    {
        return weapons.FirstOrDefault(weapon => weapon.itemID == ID);
    }
}
