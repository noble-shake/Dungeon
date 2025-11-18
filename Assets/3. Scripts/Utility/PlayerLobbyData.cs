using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLobbyData
{
    // public Dictionary<int, ulong> ClassPerPlayerDict { get => classPerPlayerDict; private set => classPerPlayerDict = value; }

    // class PlayerData
    // {
    //     public ulong clientID;
    //     public string playerName;
    //     public int characterClass = -1;
    //     public int order;
    //     public PlayerData()
    //     { }
    //     public PlayerData(ulong clientID, string playerName, int order)
    //     {
    //         this.clientID = clientID;
    //         this.playerName = playerName;
    //         this.order = order;
    //     }

    //     public PlayerData(ulong clientID, int order)
    //     {
    //         this.clientID = clientID;
    //     }
    // }
    private List<PlayerData> connectedPlayerList = new();

    public void Initialization()
    {
        if (connectedPlayerList == null)
        {
            connectedPlayerList = new List<PlayerData>();
        }
        else
        {
            connectedPlayerList.Clear();
        }
    }

    public void AddConnectedPlayer(ulong clientID, string playerName, int spawnPositionIndex)
    {
        Debug.Log($"{clientID} 플레이어 접속");
        PlayerData playerData = connectedPlayerList.Find(data => data.clientID == clientID);
        if (playerData == null)
        {
            int index = spawnPositionIndex;
            connectedPlayerList.Add(new PlayerData(clientID, playerName, index));
        }
        else
        {
            playerData.playerName = playerName;
        }
    }

    public void AddConnectedPlayer(ulong clientID)
    {
        Debug.Log($"{clientID} 플레이어 접속");
        PlayerData playerData = connectedPlayerList.Find(data => data.clientID == clientID);
        if (playerData == null)
        {
            int order = connectedPlayerList.Count;
            connectedPlayerList.Add(new PlayerData(clientID, order));
        }
    }

    public void RemoveConnectedPlayer(ulong clientID)
    {
        PlayerData dataToRemove = connectedPlayerList.Find(data => data.clientID == clientID);
        connectedPlayerList.Remove(dataToRemove);
    }

    public List<PlayerData> GetCopiedPlayerList()
    {

        List<PlayerData> copiedPlayerList = new();
        foreach (PlayerData playerData in connectedPlayerList)
        {
            copiedPlayerList.Add(new PlayerData(playerData));
        }
        return copiedPlayerList;
    }



    public void RespondToClassSelect(ulong clientID, int characterClassIndex)
    {
        // 이미 검사를 하고 실행 됨.
        // 아래 줄 주석을 풀면 캐릭 중복 선택 안 됨.
        // if (!IfClassExistsInPlayers(characterClassIndex) 
        PlayerData playerData = connectedPlayerList.Find(data => data.clientID == clientID);
        if (playerData == null)
        {
            Debug.LogError("플레이어가 없습니다.");
            return;
        }
        playerData.characterClass = characterClassIndex;
    }

    public int GetOrderOfPlayer(ulong clientID)
    {
        foreach (PlayerData playerDat in connectedPlayerList)
        {
            Debug.Log($"플레이어 순서 : {playerDat.order}");
        }
        PlayerData playerData = connectedPlayerList.Find(data => data.clientID == clientID);
        if (playerData == null)
        {
            Debug.LogError("플레이어가 없습니다.");
        }
        return playerData.order;
    }

    public bool IfClassExistsInPlayers(int characterClassIndex)
    {
        if (connectedPlayerList.Find(data => data.characterClass == characterClassIndex) == null)
            return false;
        return true;
    }

    public void RevertClassSelected(ulong clientID)
    {
        PlayerData playerData = connectedPlayerList.Find(data => data.clientID == clientID);
        // 다른 사용자가 이미 사용중인 클래스가 있다고 생각하지 않게끔 -1로 변경.
        playerData.characterClass = -1;
    }

    // 
    public Dictionary<ulong, int> GetCopiedPlayerClassList()
    {
        Dictionary<ulong, int> copiedPlayerClassList = new Dictionary<ulong, int>();
        foreach (var data in connectedPlayerList)
        {
            copiedPlayerClassList[data.clientID] = data.characterClass;
        }
        return copiedPlayerClassList;
    }
}
