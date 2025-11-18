using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;

public class LobbyCharacterContoller : MonoBehaviour
{
    [SerializeField] Transform[] spawnPoistionList;

    [SerializeField] GameObject lobbyCharacter;
    private Dictionary<ulong, GameObject> lobbyCharacterDict = new();

    private void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // 플레이어가 로비룸에 들어왔을 때 실행 됨. 떠날 때 지우는 작업도 해야 됨.
    public void GenerateLobbyCharacter(ulong clientID, int position)
    {
        GameObject playerCharacter = Instantiate(lobbyCharacter, spawnPoistionList[position]);
        playerCharacter.transform.SetParent(null);
        lobbyCharacterDict.Add(clientID, playerCharacter);
        playerCharacter.GetComponent<NetworkObject>().Spawn();
    }

    public void RemoveLobbyCharacter(ulong clientID)
    {
        lobbyCharacterDict[clientID].GetComponent<NetworkObject>().Despawn();
        lobbyCharacterDict.Remove(clientID);
    }

    // 이상하게 네트워크 오브젝트가 씬 로드 시 자동으로 삭제 안되어 수동 삭제해줌.
    // 아마 씬 로드가 additive여서 혹은 수동 생성한 오브젝트라 그런 것 같기도 함.
    private void OnDisable()
    {
        foreach (var player in lobbyCharacterDict)
        {
            Destroy(player.Value);
        }
        lobbyCharacterDict.Clear();

    }


    public void EnableCharacter(ulong clientID, PlayerClass characterClass)
    {
        if (!lobbyCharacterDict.ContainsKey(clientID))
            return;
        GameObject lobbyCharacter = lobbyCharacterDict[clientID];
        int characterIndex = (int)characterClass;
        if (characterIndex != -1)
        {
            lobbyCharacter.GetComponent<LobbyCharacter>().EnableCharacterClientRpc(characterIndex);
        }
    }

    public void DisableCharacter(ulong clientID)
    {
        GameObject lobbyCharacter = lobbyCharacterDict[clientID];
        lobbyCharacter.GetComponent<LobbyCharacter>().DisableCharacterClientRpc();

    }
}
