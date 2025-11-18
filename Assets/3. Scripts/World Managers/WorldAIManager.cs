using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Linq;
using System;

public class WorldAIManager : MonoBehaviour
{
    public static WorldAIManager instance;

    [Header("Characters")]
    [SerializeField] List<AICharacterManager> spawnedAIList;
    [SerializeField] List<AICharacterSpawner> aiSpawnerList;

    [Header("Bosses")]
    [SerializeField] List<AIBossCharacterManager> spawnedBossList;

    [Header("Debug")]
    [SerializeField] bool despawnAll = false;

    [SerializeField] bool respawnAll = false;

    private void Update()
    {
        if (despawnAll)
        {
            despawnAll = false;
            DespawnAllCharacters();
        }
        if (respawnAll)
        {
            respawnAll = false;
            RespawnAllCharacters();
        }
    }

    private void RespawnAllCharacters()
    {
        foreach (var character in spawnedAIList)
        {
            if (character.isDead.Value)
                character.GetComponent<AICharacterManager>().ReviveCharacter();
        }
    }

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
    }

    void Start()
    {

    }

    public void AddCharacterToSpawnCharacterList(AICharacterManager character)
    {
        if (spawnedAIList.Contains(character))
            return;

        spawnedAIList.Add(character);

        AIBossCharacterManager bossCharacter = character as AIBossCharacterManager;

        if (bossCharacter != null)
        {
            if (spawnedBossList.Contains(bossCharacter))
                return;

            spawnedBossList.Add(bossCharacter);
        }
    }

    public AIBossCharacterManager GetBossCharacterByID(int ID)
    {
        return spawnedBossList.FirstOrDefault(boss => boss.bossID == ID);
    }

    public void SpawnCharacter(AICharacterSpawner aiCharacterSpawner)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            aiSpawnerList.Add(aiCharacterSpawner);
            aiCharacterSpawner.AttempToSpawnCharacter();
        }
    }

    public void ResetAllCharacters()
    {
        DespawnAllCharacters();
        foreach (var spawner in aiSpawnerList)
        {
            spawner.AttempToSpawnCharacter();
        }

    }

    // TODO : 디스폰될 시 디스트로이 되면서 문제발생. 오브젝트풀링을 쓰면 해결될 듯함.
    private void DespawnAllCharacters()
    {
        foreach (var character in spawnedAIList)
        {
            character.GetComponent<NetworkObject>().Despawn();
        }

        spawnedAIList.Clear();
    }

    private void DisableAllCharacters()
    {

    }


}
