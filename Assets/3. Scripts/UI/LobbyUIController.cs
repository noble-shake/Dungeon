using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

//
public class LobbyUIController : MonoBehaviour
{
    [SerializeField] private GameObject lobbyList;
    [SerializeField] private GameObject lobbyUIPrefab;
    [SerializeField] private string gameVersion;


    void Start()
    {
        gameVersion = NetworkManager.Singleton.GetComponent<GameNetworkManager>().demoVersion;
    }

    public void UIUpdateLobbyList()
    {
        UpdateLobbyList();
    }

    private async void UpdateLobbyList()
    {
        while (Application.isPlaying && this.gameObject is not null)
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            foreach (Transform t in lobbyList.transform)
            {
                Destroy(t.gameObject);
            }
            if (queryResponse.Results.Count == 0)
            {
                Debug.Log("로비가 없습니다.");
            }
            foreach (Lobby lobby in queryResponse.Results)
            {
                if (!lobby.Data.ContainsKey("joinCode"))
                {
                    // Destroy(newLobbyItem);
                    continue;
                }
                if (lobby.Data["version"].Value != gameVersion)
                {
                    continue;
                }
                Transform newLobbyItem = Instantiate(lobbyUIPrefab, lobbyList.transform).GetComponent<Transform>();
                string lobbyId = lobby.Id;
                string dungeonName = lobby.Data["dungeon"].Value;
                string currentPlayerStatusString = string.Empty;
                // lobby.Players.Count + " / " + lobby.MaxPlayers;
                float playTime = float.Parse(lobby.Data["time"].Value);
                float nowTime = Time.time;
                int minute = (int)(playTime - nowTime) / 60;
                newLobbyItem.GetComponent<LobbyButton>().LobbySetting(lobbyId, dungeonName, currentPlayerStatusString, minute.ToString(), lobby.LobbyCode);
            }
            await Task.Delay(3000);
        }
    }
}
