using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class WarriorGolemPattern3MagicCircle : NetworkBehaviour
{
    public WarriorGolemPattern3Component CircleManager;
    public ParticleSystem DefaultCircle;
    public List<ParticleSystem> Circles;
    public List<ParticleSystem> CircleHints;
    
    public ParticleSystem CurrentCircle;
    public ParticleSystem CurrentHint;
    public CircleObjectType CurrentID;
    public bool isActivated;
    float CurTime;
    float ActivatedTime = 3f;

    public ParticleSystem SummonObject;
    public DamageCollider damageCollider;
    public List<CharacterManager> Players;

    public void Start()
    {
        Players = new List<CharacterManager>();
    }

    public void SetCircle(int idx)
    {
        CurrentCircle = Circles[idx];
        CurrentHint= CircleHints[idx];
        CurrentID= (CircleObjectType)idx;

    }
    private void Update()
    {
        CurTime -= Time.deltaTime;

        if (CurTime < 0f)
        {
            CurTime = 0f;
            if (isActivated == false)
            {
                DefaultCircle.gameObject.SetActive(true);
                if(CurrentHint != null) CurrentHint.gameObject.SetActive(false);
                if (CurrentCircle != null) CurrentCircle.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;

        if (other.GetComponent<PlayerManager>() == null) return;

        if (isActivated == true) return;

        //CurTime = ActivatedTime;

        //DefaultCircle.gameObject.SetActive(false);
        //CurrentCircle.gameObject.SetActive(true);
        //CurrentHint.gameObject.SetActive(true);

        TriggerServerRpc();

        PlayerManager player = other.GetComponent<PlayerManager>();

        var earned =  player.playerInventoryManager.circleObject;
        if (earned == null) return;

        if (earned.CurCircleType == CurrentID)
        {

            PlayerTriggerServerRpc();
            CircleManager.MagicCircleClearCheck();

            player.playerInventoryManager.circleObject = null;
            earned.GetComponent<NetworkObject>().Despawn();
            return;
        }
        else
        {
            // Explosion Effect.
            HitTargets();
            SummonObject.Play();

            int objectID = player.playerInventoryManager.circleObject.ownerIndex;
            player.playerInventoryManager.circleObject = null;
            earned.GetComponent<NetworkObject>().Despawn();

            CircleManager.ReproduceObject(objectID);

            return;
        }
    }

    [ServerRpc]
    public void TriggerServerRpc()
    {
        TriggerClientRpc();
    }

    [ClientRpc]
    public void TriggerClientRpc()
    {
        CurTime = ActivatedTime;

        DefaultCircle.gameObject.SetActive(false);
        CurrentCircle.gameObject.SetActive(true);
        CurrentHint.gameObject.SetActive(true);
    }

    [ServerRpc]
    public void PlayerTriggerServerRpc()
    {
        PlayerTriggerClientRpc();
    }

    [ClientRpc]
    public void PlayerTriggerClientRpc()
    {
        isActivated = true;
        DefaultCircle.gameObject.SetActive(false);
        CurrentCircle.gameObject.SetActive(true);
    }

    [ServerRpc]
    public void OnMagicCircleSetServerRpc(int posID, int tossID, Vector3 circlePosID)
    {
        OnMagicCircleSetClientRpc(posID, tossID, circlePosID);
    }

    [ClientRpc]
    public void OnMagicCircleSetClientRpc(int posID, int tossID, Vector3 circlePosID)
    {
        SetCircle(tossID);
        transform.position = circlePosID;
        gameObject.SetActive(true);
    }

    public void ExplosionTrigger(Collider other)
    {
        if (other.GetComponentInParent<CharacterManager>() != null)
        {
            CharacterManager target = other.GetComponentInParent<CharacterManager>();
            if (Players.Contains(target) == false && target.characterGroup == CharacterType.Player)
            {
                Players.Add(target);
            }
        }
    }

    public void ExplosionTriggerOut(Collider other)
    {
        if (other.GetComponentInParent<CharacterManager>() != null)
        {
            CharacterManager target = other.GetComponentInParent<CharacterManager>();
            if (Players.Contains(target) == true && target.characterGroup == CharacterType.Player)
            {
                Players.Remove(target);
            }
        }
    }


    public void HitTargets()
    {
        WaitAndDisableDamageCollider(0.5f).Forget();
    }

    public async UniTaskVoid WaitAndDisableDamageCollider(float duration)
    {
        damageCollider.EnableDamageCollider();
        await UniTask.WaitForSeconds(duration);
        damageCollider.DisableDamageCollider();
        Players.Clear();
    }

}
