using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public ulong clientID;
    public string playerName;
    public int characterClass = -1;
    public int order;
    public PlayerData()
    { }
    public PlayerData(ulong clientID, string playerName, int order)
    {
        this.clientID = clientID;
        this.playerName = playerName;
        this.order = order;
    }

    public PlayerData(PlayerData playerData)
    {
        this.clientID = playerData.clientID;
        this.playerName = playerData.playerName;
        this.characterClass = playerData.characterClass;
        this.order = playerData.order;
    }

    public PlayerData(ulong clientID, int order)
    {
        this.clientID = clientID;
    }
}