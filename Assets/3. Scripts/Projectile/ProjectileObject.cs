using Unity.Netcode;
using UnityEngine;

public class ProjectileObject : NetworkBehaviour
{
    GameObject BakedPrefab;
    [HideInInspector] public ObjectPoolType objectPoolType;
    [HideInInspector] public int index;
    [HideInInspector] public GameObject VFX;
    public IProjectile MoveMethod;
    public int PenetratedCounter;
    public float Speed;

    public ProjectileCollider projectileCollider;
    void Awake()
    {
        projectileCollider = GetComponent<ProjectileCollider>();
    }

    public void SetPrefab(GameObject _object)
    {
        BakedPrefab = _object;
    }

    public void SetMove(IProjectile _method)
    {
        MoveMethod = _method;
    }

    public void ReplyMove()
    {
        MoveMethod.Init(this, transform);
    }

    private void FixedUpdate()
    {
        if (IsOwner && MoveMethod != null)
            MoveMethod.Move();
    }

    public void Remove()
    {
        // Destroy(gameObject);
        if (IsOwner)
        {
            GetComponent<NetworkObject>().Despawn();
            MoveMethod = null;
        }
        // NetworkObject는 Despawn할 때 자동으로 삭제된다.
        // ObjectPoolManager.Singleton.ReturnObject(gameObject, BakedPrefab);
        // gameObject.SetActive(false);
    }

    public void PenetrateCount()
    {
        PenetratedCounter--;
    }

    // RPC로 넘길 수 있는 매개변수는 제가 알기로는 c# 기본 타입, Unity 기본 클래스 들 밖에? 

    // NetworkObjectReference

    /// <summary>
    /// networkObjectReference는 characterAttacking 등을 위해 필요.
    /// 프리팹 정체를 알기 위해 objectpoolTpye과 index 필요
    /// 로컬에서 패리 가능, 반사 가능 여부 판단 하기 위해 isParryable, isReflectable 필요
    ///  
    /// </summary>
    /// <param name="networkObjectReference"></param>
    /// <param name="objectPoolType"></param>
    /// <param name="objectpoolIndex"></param>
    /// <param name="isParryable"></param>
    /// <param name="isReflectable"></param>
    [ServerRpc]
    public void InitializeServerRpc(ulong networkObjectID,
     ObjectPoolType objectPoolType, int objectpoolIndex,
     bool isParryable, bool isReflectable, int damage, int groggyDamage)
    {
        InitializeClientRpc(networkObjectID, objectPoolType, objectpoolIndex, isParryable, isReflectable, damage, groggyDamage);
    }

    [ClientRpc]
    private void InitializeClientRpc(ulong networkObjectID, ObjectPoolType objectPoolType, int index, bool isParryable, bool isReflectable, int damage, int groggyDamage)
    {
        CharacterManager[] characterManagerList = FindObjectsByType<CharacterManager>(sortMode: FindObjectsSortMode.None);
        foreach (var characterManager in characterManagerList)
        {
            if (characterManager.NetworkObjectId == networkObjectID)
            {
                projectileCollider.characterAttacking = characterManager;

                break;
            }
        }
        projectileCollider.isParryable = isParryable;
        projectileCollider.isReflectable = isReflectable;
        projectileCollider.physicalDamage = damage;
        projectileCollider.groggyDamage = groggyDamage;
        this.objectPoolType = objectPoolType;
        this.index = index;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetDirectionServerRpc(Vector3 direction)
    {
        transform.rotation = Quaternion.LookRotation(direction);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetTargetGroupServerRpc(CharacterType[] targetGroup)
    {
        SetTargetGroupClientRpc(targetGroup);
    }

    [ClientRpc]
    private void SetTargetGroupClientRpc(CharacterType[] targetGroup)
    {
        projectileCollider.TargetGroup = targetGroup;
        projectileCollider.CharactersDamaged.Clear();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetCharacterAttackingServerRpc(NetworkObjectReference networkObjectReference)
    {
        networkObjectReference.TryGet(out NetworkObject characterAttacking);
        if (characterAttacking != null)
            SetCharacterAttackingClientRpc(characterAttacking.NetworkObjectId);
        // projectileCollider.characterAttacking = characterAttacking.GetComponent<CharacterManager>();
    }
    [ClientRpc]
    public void SetCharacterAttackingClientRpc(ulong networkObjectID)
    {
        CharacterManager[] characterManagerList = FindObjectsByType<CharacterManager>(sortMode: FindObjectsSortMode.None);
        foreach (var characterManager in characterManagerList)
        {
            if (characterManager.NetworkObjectId == networkObjectID)
            {
                projectileCollider.characterAttacking = characterManager;
                break;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetCanAttackWhenInvulnerableStateServerRpc(bool condition)
    {
        SetCanAttackWhenInvulnerableStateClientRpc(condition);
    }

    [ClientRpc]
    private void SetCanAttackWhenInvulnerableStateClientRpc(bool condition)
    {
        projectileCollider.canAttackWhenInvulnerableState = condition;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReflectServerRpc(Vector3 direction, bool attackOption, CharacterType[] characterTypes, NetworkObjectReference networkObject)
    {
        transform.rotation = Quaternion.LookRotation(direction);
        ReflectClientRpc(direction, attackOption, characterTypes, networkObject.NetworkObjectId);
    }

    [ClientRpc]
    private void ReflectClientRpc(Vector3 direction, bool attackOption, CharacterType[] targetGroup, ulong networkObjectID)
    {
        CharacterManager[] characterManagerList = FindObjectsByType<CharacterManager>(sortMode: FindObjectsSortMode.None);
        foreach (var characterManager in characterManagerList)
        {
            if (characterManager.NetworkObjectId == networkObjectID)
            {
                projectileCollider.characterAttacking = characterManager;
                break;
            }
        }

        projectileCollider.TargetGroup = targetGroup;
        projectileCollider.CharactersDamaged.Clear();

        projectileCollider.canAttackWhenInvulnerableState = attackOption;

    }
}