using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class ArcherSynergySystem : NetworkBehaviour
{
    [HideInInspector] public PlayerManager playerManager; 
    [SerializeField] public ArcherSynergyShot VFXPrefab;

    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    public void OnSynergyShot()
    {
        ArcherSynergyShot synergyShot = Instantiate(VFXPrefab);
        synergyShot.transform.position = transform.position + transform.forward * 6f;
        synergyShot.transform.rotation = transform.rotation;
    }
}