using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class UIDungeonSelectButton : MonoBehaviour
{
    [SerializeField] private string dungeonSceneName;
    // Start is called before the first frame update

    // 로비 파라미터로 포함시켜야 하는 것 => 어떤 맵인지 알아야 함.
    // 맨 처음 지도에서 맵을 고르고 로비를 만들겠다 하면 실행되는 함수. 지금은 그냥 발동

    public void UICreateLobby()
    {
        // 어딘가에서 로비 파라미터를 가져옴.
        LobbyParameters lobbyParameters = null;
        GameNetworkManager.Instance.StartAsHost(lobbyParameters, dungeonSceneName);
    }

}
