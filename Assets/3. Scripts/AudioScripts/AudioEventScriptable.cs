using FMODUnity;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AudioSO", menuName = "FMOD/AudioEventDefine")]
public class AudioEventScriptable : ScriptableObject
{

    [SerializeField] public EventReference eventPath;
    [SerializeField] public SFXType ownerType;

    [SerializeField] public PlayerSFX PlayerSFXType;
    [SerializeField] public TitleSFX TitleSFXType;
    [SerializeField] public LobbySFX LobbySFXType;
    [SerializeField] public DungeonSFX DungeonSFXType;
    [SerializeField] public WarriorGolemSFX WarriorGolemSFXType;

    public EventReference GetEventRef()
    {
        EventReference eventRef = eventPath;
        return eventRef;
    }

    public SFXType GetOwnerType()
    {
        SFXType _own = ownerType;
        return _own;
    }

    public int GetEnums(SFXType ownerType)
    {
        switch (ownerType)
        {
            default:
            case SFXType.Player:
                return (int)PlayerSFXType;
            case SFXType.Title:
                return (int)TitleSFXType;
            case SFXType.Lobby:
                return (int)LobbySFXType;
            case SFXType.Dungeon:
                return (int)DungeonSFXType;
            case SFXType.WarriorGolem:
                return (int)WarriorGolemSFXType;
        }
    }

}