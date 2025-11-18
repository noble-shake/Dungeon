using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Singleton { get; private set; }
    List<PoolConfigObject> pooledPrefabList;
    private GameObject currentChosenGameObject;
    [SerializeField] List<PoolConfigObject[]> poolConfigObjectList = new();
    [SerializeField] PoolConfigObject[] hitList;
    [SerializeField] PoolConfigObject[] projectileList;
    [SerializeField] PoolConfigObject[] explosionList;
    [SerializeField] PoolConfigObject[] sealList;
    [SerializeField] PoolConfigObject[] firstBossList;
    [SerializeField] PoolConfigObject[] buffList;

    HashSet<GameObject> prefabs = new HashSet<GameObject>();

    [SerializeField] private Transform defaultParent;

    Dictionary<GameObject, ObjectPool<GameObject>> pooledObjects = new Dictionary<GameObject, ObjectPool<GameObject>>();

    public void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }

        poolConfigObjectList.Add(hitList);
        poolConfigObjectList.Add(projectileList);
        poolConfigObjectList.Add(explosionList);
        poolConfigObjectList.Add(sealList);
        poolConfigObjectList.Add(firstBossList);
        poolConfigObjectList.Add(buffList);



        PoolObject();

        // foreach (var configObject in pooledPrefabList)
        // {
        //     RegisterPrefabInternal(configObject.prefab, configObject.prewarmCount);
        // }
    }

    private void PoolObject()
    {
        foreach (var prefabList in poolConfigObjectList)
        {
            for (int i = 0; i < prefabList.Length; i++)
            {
                currentChosenGameObject = prefabList[i].prefab;
                pooledObjects[prefabList[i].prefab] = new ObjectPool<GameObject>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy, defaultCapacity: prefabList[i].prewarmCount);
                var tempPool = new List<GameObject>();

                // 미리 생성해두는 과정. 
                for (var j = 0; j < prefabList[i].prewarmCount; j++)
                {
                    tempPool.Add(pooledObjects[prefabList[i].prefab].Get());
                }
                foreach (var gameObject in tempPool)
                {
                    pooledObjects[prefabList[i].prefab].Release(gameObject);
                }
            }
        }
    }

    GameObject CreateFunc()
    {
        GameObject instantiatedGameObject = Instantiate(currentChosenGameObject);
        if (instantiatedGameObject.GetComponent<NetworkObject>() == null)
            instantiatedGameObject.transform.SetParent(defaultParent);
        return instantiatedGameObject;
    }
    void ActionOnGet(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }
    void ActionOnRelease(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
    void ActionOnDestroy(GameObject gameObject)
    {
        Destroy(gameObject);
    }

    public GameObject GetObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        try
        {
            var pooledObject = pooledObjects[prefab].Get();
            pooledObject.transform.position = position;
            pooledObject.transform.rotation = rotation;

            return pooledObject;
        }
        catch (KeyNotFoundException e)
        {
            Debug.LogError($"Prefab {prefab.name} not found in the pool. Exception: {e.Message}");
            return null;
        }
    }

    public GameObject GetObject(ObjectPoolType objectPoolType, int index)
    {
        GameObject pooledObject = null;
        switch (objectPoolType)
        {
            case ObjectPoolType.Hit:
                currentChosenGameObject = hitList[index].prefab;
                pooledObject = pooledObjects[hitList[index].prefab].Get();
                break;
            case ObjectPoolType.Projectile:
                currentChosenGameObject = projectileList[index].prefab;
                pooledObject = pooledObjects[projectileList[index].prefab].Get();
                break;
            case ObjectPoolType.Explosion:
                currentChosenGameObject = explosionList[index].prefab;
                pooledObject = pooledObjects[explosionList[index].prefab].Get();
                break;
            case ObjectPoolType.Seal:
                currentChosenGameObject = sealList[index].prefab;
                pooledObject = pooledObjects[sealList[index].prefab].Get();
                break;
            case ObjectPoolType.FirstBoss:
                currentChosenGameObject = firstBossList[index].prefab;
                pooledObject = pooledObjects[firstBossList[index].prefab].Get();
                break;
            case ObjectPoolType.Buff:
                currentChosenGameObject = buffList[index].prefab;
                pooledObject = pooledObjects[buffList[index].prefab].Get();
                break;
        }
        return pooledObject;
    }

    public GameObject GetObject(GameObject prefab)
    {
        var pooledObject = pooledObjects[prefab].Get();
        // pooledObject.TryGetComponent(out NetworkObject networkObject);
        // if (networkObject != null)
        // {
        //     if (networkObject.IsSpawned == false)
        //         networkObject.Spawn();
        // }

        return pooledObject;
    }

    public void ReturnObject(GameObject gameObject, GameObject prefab)
    {
        pooledObjects[prefab].Release(gameObject);
    }

    public async UniTaskVoid WaitAndReturnVFX(GameObject pooledObject, GameObject prefab)
    {
        await UniTask.WaitUntil(() => pooledObject.GetComponent<ParticleSystem>().IsAlive() == false);
        ReturnObject(pooledObject, prefab);
    }


    public GameObject GetPrefab(ObjectPoolType objectPoolType, int index)
    {
        GameObject prefab = null;
        switch (objectPoolType)
        {
            case ObjectPoolType.Hit:
                prefab = hitList[index].prefab;
                break;
            case ObjectPoolType.Projectile:
                prefab = projectileList[index].prefab;
                break;
            case ObjectPoolType.Explosion:
                prefab = explosionList[index].prefab;
                break;
            case ObjectPoolType.FirstBoss:
                prefab = firstBossList[index].prefab;
                break;
            case ObjectPoolType.Seal:
                prefab = sealList[index].prefab;
                break;
            case ObjectPoolType.Buff:
                prefab = buffList[index].prefab;
                break;

        }
        return prefab;
    }

    // private void RegisterPrefabInternal(GameObject prefab, int prewarmCount)
    // {

    //     GameObject CreateFunc()
    //     {
    //         GameObject instantiatedGameObject = Instantiate(prefab);
    //         if (instantiatedGameObject.GetComponent<NetworkObject>() == null)
    //             instantiatedGameObject.transform.SetParent(defaultParent);
    //         return instantiatedGameObject;
    //     }
    //     void ActionOnGet(GameObject gameObject)
    //     {
    //         gameObject.SetActive(true);
    //     }
    //     void ActionOnRelease(GameObject gameObject)
    //     {
    //         gameObject.SetActive(false);
    //     }
    //     void ActionOnDestroy(GameObject gameObject)
    //     {
    //         Destroy(gameObject);
    //     }

    //     // 해시셋이라 자동으로 중복 제거 
    //     prefabs.Add(prefab);

    //     pooledObjects[prefab] = new ObjectPool<GameObject>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy, defaultCapacity: prewarmCount);

    //     var tempPool = new List<GameObject>();

    //     // 미리 생성해두는 과정. 
    //     for (var i = 0; i < prewarmCount; i++)
    //     {
    //         tempPool.Add(pooledObjects[prefab].Get());
    //     }
    //     foreach (var gameObject in tempPool)
    //     {
    //         pooledObjects[prefab].Release(gameObject);
    //     }

    // }
}

[Serializable]
struct PoolConfigObject
{
    public int index;
    public GameObject prefab;
    public int prewarmCount;
}

