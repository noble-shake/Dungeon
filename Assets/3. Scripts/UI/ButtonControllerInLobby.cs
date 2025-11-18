using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ButtonControllerInLobby : MonoBehaviour
{
    // 던전 입장 버튼에 할당되어 실행된다. 
    public void GameStartButton()
    {
        // StartCoroutine(EnterDungeon());
        EnterDungeon(1f).Forget();

        // 원본
        // GameNetworkManager.Instance.EnterDungeon();
    }

    // 이 함수는 호스트가 던전 씬을 네트워크 로드할 때 호출된다.
    // 
    private async UniTaskVoid EnterDungeon(float fadeOutTime)
    {
        // RPC 해줄 애가 LobbySettingManager 밖에 없음.
        // 게임네트워크 매니저로 통일시켜야 하나? 
        LobbySettingManager.instance.FadeOutServerRpc(fadeOutTime);
        await UniTask.Delay(TimeSpan.FromSeconds(fadeOutTime));
        GameNetworkManager.Instance.EnterDungeon();
    }

    // 클라이언트 입장에선 씬 로딩 + 부팅 씬 활성화
    // 호스트 입장에선 씬 로딩 + 부팅 씬 활성화 + 로비 폭파 + 클라이언트들한테 네트워크 단절 시 함수수 호출
    public async void ExitLobbyButton()
    {
        if (LobbySettingManager.instance.IsHost)
            await GameNetworkManager.Instance.RemoveLobby();
        if (LobbySettingManager.instance.IsClient == true && LobbySettingManager.instance.IsHost == false)
        {
            GameNetworkManager.Instance.QuitLobby();
        }
        NetworkManager.Singleton.Shutdown();
        SceneLoadManager.Instance.LoadRegularScene("Scene_Booting");
    }

    public void ExitLobbyWhenDisconnectedButton()
    {
        NetworkManager.Singleton.Shutdown();
        SceneLoadManager.Instance.LoadRegularScene("Scene_Booting");
    }
}
