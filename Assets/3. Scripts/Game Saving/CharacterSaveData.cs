using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterSaveData
{
    [Header("Scene Index")]
    public int sceneIndex = 1;


    [Header("Character Name")]
    public string characterName = "Character";

    [Header("Time Played")]
    public float secondsPlayed;

    [Header("World Coordinates")]
    public float xPosition;
    public float yPosition;
    public float zPosition;

    [Header("Resources")]
    public int currentHealth;
    public float currentStamina;

    [Header("Stats")]
    public int vitality;
    public int endurance;

    [Header("Sites of Grace")]
    public SerializableDictionary<int, bool> sitesOfGrace;

    [Header("Bosses")]
    public SerializableDictionary<int, bool> bossesAwakened; // 키 값은 보스의 ID, 불 값은 보스의 활성화 상태
    public SerializableDictionary<int, bool> bossesDefeated; // 키 값은 보스의 ID, 불 값은 보스를 깬 적 있는지 없는지

    public CharacterSaveData()
    {
        bossesAwakened = new SerializableDictionary<int, bool>();
        bossesDefeated = new SerializableDictionary<int, bool>();
    }

}
