using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldActionManager : MonoBehaviour
{
    public static WorldActionManager instance;

    [Header("Weapon Item Actions")]
    public WeaponAction[] weaponItemActions;

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

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        for (int i = 0; i < weaponItemActions.Length; i++)
        {
            weaponItemActions[i].actionID = i;
        }
    }

    public WeaponAction GetWeaponItemActionByID(int ID)
    {
        return weaponItemActions.FirstOrDefault(action => action.actionID == ID);
    }
}
