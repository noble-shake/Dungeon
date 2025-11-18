using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkObjectPool : INetworkPrefabInstanceHandler
{
    private Queue<GameObject> _objectPool = new Queue<GameObject>();
    private GameObject _prefab;

    public NetworkObjectPool(GameObject prefab, int initialSize)
    {
        _prefab = prefab;

        // 초기 생성 및 풀에 저장
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = GameObject.Instantiate(_prefab);
            obj.SetActive(false);
            _objectPool.Enqueue(obj);
        }
    }

    // 🔹 네트워크 오브젝트 풀에서 꺼내거나 생성
    public GameObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
    {
        GameObject obj;

        if (_objectPool.Count > 0)
        {
            obj = _objectPool.Dequeue(); // 풀에서 가져오기
        }
        else
        {
            obj = GameObject.Instantiate(_prefab); // 없으면 새로 생성
        }

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        return obj;
    }

    // 🔹 네트워크 오브젝트를 다시 풀에 넣기
    public void Destroy(GameObject gameObject)
    {
        gameObject.SetActive(false);
        _objectPool.Enqueue(gameObject); // 다시 풀에 추가
    }

    NetworkObject INetworkPrefabInstanceHandler.Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
    {
        GameObject obj;

        if (_objectPool.Count > 0)
        {
            obj = _objectPool.Dequeue(); // 풀에서 가져오기
        }
        else
        {
            obj = GameObject.Instantiate(_prefab); // 없으면 새로 생성
        }

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        return obj.GetComponent<NetworkObject>();
    }

    public void Destroy(NetworkObject networkObject)
    {
        throw new System.NotImplementedException();
    }
}
