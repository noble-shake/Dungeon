/*
    This Pattern be Legacied.
 */


using Unity.Netcode;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class WarriorGolemPattern3PrisonObject : NetworkBehaviour
{
    public WarriorGolemPattern3Prison Prison;
    public GameObject Glows;
    public GameObject BreakGlow;
    public bool isDestructed;
    public int DamageHP = 400;
    public NetworkVariable<int> CurrentHP = new NetworkVariable<int>(400, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> MaxHP = new NetworkVariable<int>(400, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Awake()
    {
        MaxHP.OnValueChanged += setHP;
        CurrentHP.OnValueChanged += Hit;
    }

    [ServerRpc]
    public void SetPosServerRpc(Vector3 Pos, int HP)
    {
        SetPosClientRpc(Pos, HP);
    }

    [ClientRpc]
    public void SetPosClientRpc(Vector3 Pos, int HP)
    {
        isDestructed = false;
        transform.position = Pos;
        MaxHP.Value = HP;
    }

    public void setHP(int oldDmg, int newDmg)
    {
        MaxHP.Value = newDmg;
        CurrentHP.Value = newDmg;
    }

    public void Hit(int oldDmg, int newDmg)
    {

        if (isDestructed) return;

        if (CurrentHP.Value <= 0f)
        {
            TuronOrbServerRpc();

        }
    }

    public override void OnNetworkDespawn()
    {
        MaxHP.OnValueChanged -= setHP;
        CurrentHP.OnValueChanged -= Hit;
    }

    [ServerRpc(RequireOwnership = false)]
    public void TuronOrbServerRpc()
    {
        TuronOrbClientRpc();
    }

    [ClientRpc]
    public void TuronOrbClientRpc()
    {
        isDestructed = true;
        if(IsOwner) Prison.DestroyHappened();
        Glows.SetActive(false);
        BreakGlow.gameObject.SetActive(true);
    }


    [ServerRpc]
    public void RemoveServerRpc()
    {
        RemoveClientRpc();

    }

    [ClientRpc]
    private void RemoveClientRpc()
    {
        // particleController.RemoveParticle();
        if (IsOwner)
        {
            gameObject.GetComponent<NetworkObject>().Despawn();
        }
    }
}