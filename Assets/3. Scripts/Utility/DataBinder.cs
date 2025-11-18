using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 호스트만 접근하고 사용한다. 
public class DataBinder : MonoBehaviour
{
    public static DataBinder instance { get; private set; }

    // 인게임 로비에서의 데이터 영역
    private PlayerLobbyData playerLobbyData;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            playerLobbyData = new PlayerLobbyData();

            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
            return;
        }
    }

    private void Start()
    {

    }
    public List<PlayerData> GetCopiedPlayerList()
    {
        return playerLobbyData.GetCopiedPlayerList();
    }

    public int GetOrderOfPlayer(ulong clientID)
    {
        return playerLobbyData.GetOrderOfPlayer(clientID);
    }

    public bool IfClassExistsInPlayers(int characterClassIndex)
    {
        return playerLobbyData.IfClassExistsInPlayers(characterClassIndex);
    }

    public void RespondToClassSelect(ulong clientID, int characterClassIndex)
    {
        playerLobbyData.RespondToClassSelect(clientID, characterClassIndex);
    }

    public void RevertClassSelected(ulong clientID)
    {
        playerLobbyData.RevertClassSelected(clientID);
    }

    public void AddConnectedPlayer(ulong clientID, string playerName, int order)
    {
        playerLobbyData.AddConnectedPlayer(clientID, playerName, order);
    }
    public void AddConnectedPlayer(ulong clientID)
    {
        playerLobbyData.AddConnectedPlayer(clientID);
    }

    public Dictionary<ulong, int> GetCopiedPlayerClassList()
    {
        return playerLobbyData.GetCopiedPlayerClassList();
    }

    public void RemoveConnectedPlayer(ulong clientID)
    {
        playerLobbyData.RemoveConnectedPlayer(clientID);
    }
}