using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AICharacterSpawner : MonoBehaviour
{
    [Header("Character")]
    [SerializeField] GameObject characterToSpawn;
    
    private GameObject instantiatedGameObject;

    // private BoolReactiveProperty spawn = new BoolReactiveProperty(false);

    [SerializeField] private bool spawnbool = false;
    private void Start()
    {
        WorldAIManager.instance.SpawnCharacter(this);
        gameObject.SetActive(false);

        // UniRx를 사용하여 spawn 변수가 true로 변경될 때 AttempToSpawnCharacter 함수를 실행
        // spawn.Where(value => value == true).Subscribe(_ => AttempToSpawnCharacter()).AddTo(this);
    }

    public void AttempToSpawnCharacter()
    {
        if (characterToSpawn != null)
        {
            instantiatedGameObject = Instantiate(characterToSpawn);
            instantiatedGameObject.transform.position = transform.position;
            instantiatedGameObject.transform.rotation = transform.rotation;
            instantiatedGameObject.GetComponent<NetworkObject>().Spawn();
            WorldAIManager.instance.AddCharacterToSpawnCharacterList(instantiatedGameObject.GetComponent<AICharacterManager>());
        }
    }

    private void Update()
    {
        if (spawnbool)
        {
            spawnbool = false;
            SetSpawn(true);
        }
    }

    // spawn 변수를 변경하는 메서드
    public void SetSpawn(bool value)
    {
        // spawn.Value = value;
    }

    public GameObject GetInstantiatedObject()
    {
        return instantiatedGameObject;
    }
}