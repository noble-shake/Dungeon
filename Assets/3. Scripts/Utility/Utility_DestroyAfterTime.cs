using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility_DestroyAfterTime : MonoBehaviour
{
    [SerializeField] float timeUnitlDestroyed = 5;

    private void Awake()
    {
        Destroy(gameObject, timeUnitlDestroyed);
    }
}
