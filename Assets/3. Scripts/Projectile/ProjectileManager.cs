using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

public class ProjectileManager : NetworkBehaviour
{
    [SerializeField] private List<ProjectileStructure> BakedProjectiles;
    [SerializeField] private Transform firePosition;
    [SerializeField] private Vector3 fireRotation;

    // Fire Event
    // RPC로 발사해야 된다? 

    // 이 함수 자체는 애니메이션에서 이벤트로 실행된다.
    public void ShootProjectile(int idx = 0)
    {
        if (IsOwner)
        {
            // 서버에다가 요청한다.
            // 투사체를 생성하고 세팅하고, 스폰할 것을 요청.
            ShootProjectileServerRpc(idx, firePosition.position, firePosition.rotation.eulerAngles);
        }
    }

    [ServerRpc]
    private void ShootProjectileServerRpc(int idx, Vector3 position, Vector3 rotation)
    {
        // 서버에서 발사한다.
        ShootProjectileOnNetwork(idx, position, rotation);
    }


    public void ShootProjectileOnNetwork(int idx, Vector3 position, Vector3 rotation)
    {
        ProjectileStructure projectileStructure = BakedProjectiles[idx];
        GameObject projectileObject = ObjectPoolManager.Singleton.GetObject(projectileStructure.objectPoolType, projectileStructure.index);

        projectileObject.GetComponent<NetworkObject>().Spawn(true);

        projectileObject.transform.position = position;
        projectileObject.transform.rotation *= Quaternion.Euler(rotation);

        ProjectileObject projectile = projectileObject.GetComponent<ProjectileObject>();
        // RPC로 오너가 아니면, 
        // 받은 입장에서, 그 원본 오브젝트의 복제본을 가리킴. 
        ulong networkObjectID = GetComponent<NetworkObject>().NetworkObjectId;
        // 클라이언트 입장에서 알아야 할 것들을 세팅한다. 
        projectile.InitializeServerRpc(networkObjectID, projectileStructure.objectPoolType, projectileStructure.index,
        BakedProjectiles[idx].isParryable, BakedProjectiles[idx].isReflectable, BakedProjectiles[idx].AttackDamage,
        BakedProjectiles[idx].AttackGroggyDamage);

        projectile.SetMove(BakedProjectiles[idx].GetMoveMethod(projectile));

        projectile.Speed = BakedProjectiles[idx].Speed;
        projectile.PenetratedCounter = BakedProjectiles[idx].PenetrateCounter;

        // projectile.gameObject.SetActive(true);

        // 어차피 중요한건 발사 각도니까 Z축만 바꾸면 안 되나? 
        // if (firePosition != null)
        // {
        //     // GameObject castObject = ObjectPoolManager.Singleton.GetObject(ObjectPoolType.Explosion, 0);
        //     // castObject.transform.position = firePosition.position;
        //     // castObject.transform.rotation = transform.rotation;
        //     projectile.transform.position = firePosition.position;
        //     projectile.transform.rotation = Quaternion.LookRotation(-firePosition.forward);
        //     projectile.transform.rotation *= Quaternion.Euler(0, 0, fireRotation.z);
        // }
        // else
        // {
        //     projectile.transform.position = this.transform.position + Vector3.up;
        //     projectile.transform.rotation = transform.rotation;
        // }
    }
    // public void ShootProjectile(int idx = 0)
    // {
    //     ProjectileStructure projectileStructure = BakedProjectiles[idx];
    //     GameObject projectileObject = ObjectPoolManager.Singleton.GetObject(projectileStructure.objectPoolType, projectileStructure.index);

    //     ProjectileObject projectile = projectileObject.GetComponent<ProjectileObject>();
    //     projectile.objectPoolType = projectileStructure.objectPoolType;
    //     projectile.index = projectileStructure.index;

    //     projectile.SetPrefab(BakedProjectiles[idx].VFXObject.gameObject);
    //     projectile.SetMove(BakedProjectiles[idx].GetMoveMethod(projectile));
    //     projectile.Speed = BakedProjectiles[idx].Speed;
    //     projectile.PenetratedCounter = BakedProjectiles[idx].PenetrateCounter;

    //     projectile.GetComponent<ProjectileCollider>().characterAttacking = GetComponent<CharacterManager>();
    //     projectile.GetComponent<ProjectileCollider>().isParryable = BakedProjectiles[idx].isParryable;
    //     projectile.GetComponent<ProjectileCollider>().isReflectable = BakedProjectiles[idx].isReflectable;
    //     projectile.GetComponent<ProjectileCollider>().physicalDamage = BakedProjectiles[idx].AttackDamage;
    //     projectile.GetComponent<ProjectileCollider>().groggyDamage = BakedProjectiles[idx].AttackGroggyDamage;

    //     projectile.gameObject.SetActive(true);

    //     // 어차피 중요한건 발사 각도니까 Z축만 바꾸면 안 되나? 
    //     if (firePosition != null)
    //     {
    //         // GameObject castObject = ObjectPoolManager.Singleton.GetObject(ObjectPoolType.Explosion, 0);
    //         // castObject.transform.position = firePosition.position;
    //         // castObject.transform.rotation = transform.rotation;
    //         projectile.transform.position = firePosition.position;
    //         projectile.transform.rotation = Quaternion.LookRotation(-firePosition.forward);
    //         projectile.transform.rotation *= Quaternion.Euler(0, 0, fireRotation.z);
    //     }
    //     else
    //     {
    //         projectile.transform.position = this.transform.position + Vector3.up;
    //         projectile.transform.rotation = transform.rotation;
    //     }
    // }

    public void ChangeFiringRotation(string rotationString)
    {
        string[] splitValues = rotationString.Split(',');
        if (splitValues.Length == 3 &&
            float.TryParse(splitValues[0], out float x) &&
            float.TryParse(splitValues[1], out float y) &&
            float.TryParse(splitValues[2], out float z))
        {
            x = x == 0f ? fireRotation.x : x;
            y = y == 0f ? fireRotation.y : y;
            z = z == 0f ? fireRotation.z : z;
            // fireRotation = new Vector3(x, y, z);
            firePosition.localRotation = Quaternion.Euler(0, 0, 0);
            firePosition.localRotation *= Quaternion.Euler(x, y, z);
        }

    }


}