using TMPro;
using UnityEngine;

public class LobbyButton : MonoBehaviour
{
    // TODO : 팀원들의 캐릭터 타입 로고 노출
    // TODO : 방제도 노출해야 됨.
    private string lobbyId;
    private string joinCode;

    [Header("로비 버튼 노출 목록")]
    [SerializeField] private TextMeshProUGUI dungeonName; // 던전 이름
    [SerializeField] private TextMeshProUGUI lobbyCode;
    [SerializeField] private TextMeshProUGUI currentPlayerStatusString; // 현재 플레이어 인원 수 
    [SerializeField] private TextMeshProUGUI playTime; // 게임 진행시간 

    public void LobbySetting(string lobbyId, string dungeonName, string currentPlayerStatusString, string playTime, string lobbyCode)
    {
        this.lobbyId = lobbyId;
        this.dungeonName.text = dungeonName;
        this.currentPlayerStatusString.text = currentPlayerStatusString;
        this.playTime.text = playTime;
        this.lobbyCode.text = "Lobby Code : " + lobbyCode;
    }

    // TODO : 이 함수를 사실 LobbyManager로 접근해서 다뤄야 함. 
    public void UI_JoinAsClient()
    {
        if (lobbyId == string.Empty)
        {
            Debug.LogWarning("로비 아이디가 세팅되지 않아서 로비에 진입할 수 없습니다.");
            return;
        }

        GameNetworkManager.Instance.JoinAsClient(lobbyId);
    }
}
