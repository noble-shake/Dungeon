using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FogWallInteractable : Interactable
{
    [Header("Fog")]
    [SerializeField] GameObject[] fogGameObjects;

    [Header("Collisions")]
    [SerializeField] Collider fogWallCollider;

    [Header("ID")]
    public int fogWallID;

    [Header("Sound")]
    private AudioSource fogWallAudioSource;


    [Header("Active")]
    public NetworkVariable<bool> isActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    protected override void Awake()
    {
        base.Awake();

        fogWallAudioSource = gameObject.GetComponent<AudioSource>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        OnIsActiveChanged(false, isActive.Value);
        isActive.OnValueChanged += OnIsActiveChanged;
        WorldObjectManager.instance.AddFogWallToList(this);

    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        isActive.OnValueChanged -= OnIsActiveChanged;
        WorldObjectManager.instance.RemoveFogWallFromList(this);

    }

    private void OnIsActiveChanged(bool previousValue, bool newValue)
    {
        if (isActive.Value)
        {
            foreach (var fogObject in fogGameObjects)
            {
                fogObject.SetActive(true);
            }
        }
        else
        {
            foreach (var fogObject in fogGameObjects)
            {
                fogObject.SetActive(false);
            }
        }
    }

    public override void Interact(PlayerManager player)
    {
        base.Interact(player);

        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward);
        player.transform.rotation = targetRotation;

        AllowPlayerThroughFogWallCollidersServerRpc(player.NetworkObjectId);

        print("내가 과연 여기서 실행되고 있을까? 0 ");
        // player.playerAnimatorManager.PlayTargetActionAnimation("Pass_Through_01", true);

        // 벽 보기
        // 콜라이더 제거(벽을 통과할 수 있게끔), 근데 서버단에서 진행되서 다른 클라이언트들의 벽 콜라이더도 없앰.
        // 통과
        // 다시 콜라이더 활성화
    }

    [ServerRpc(RequireOwnership = false)]
    private void AllowPlayerThroughFogWallCollidersServerRpc(ulong playerObjectID)
    {
        print("내가 과연 여기서 실행되고 있을까? 1.1");
        if (IsServer)
        {
            AllowPlayerThroughFogWallCollidersClientRpc(playerObjectID);
            print("내가 과연 여기서 실행되고 있을까? 1");
        }
    }

    // 유니티 넷코드
    // 로비에 접속 -> 릴레이 서버 접속 
    // 씬을 동기화하는 단계는 릴레이 서버 접속
    // 로비 생성하자마자 릴레이 서버 생성 후 방장은 이미 접속해있는 상태
    // 다른 사람들은 로비에 들어오자마자 릴레이 서버에 접속해서 씬을 동기화 

    // NetworkManager는 릴레이 서버가 활성화된 다음에 초기화 됨. 
    // ulong 값으로 관리 되는 데이터가 2개 있음.
    // 클라이언트 ID 0
    // 네트워크 상에 스폰된 오브젝트 ID 

    [ClientRpc]
    private void AllowPlayerThroughFogWallCollidersClientRpc(ulong playerObjectID)
    {
        PlayerManager player = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerObjectID].GetComponent<PlayerManager>();

        print("내가 과연 여기서 실행되고 있을까? 2");
        // fogWallAudioSource.PlayOneShot(fogWallSFX);

        if (player != null)
            StartCoroutine(DisableCollisionForTime(player));
    }

    private IEnumerator DisableCollisionForTime(PlayerManager player)
    {
        print("내가 과연 여기서 실행되고 있을까? 3");
        Physics.IgnoreCollision(player.characterController, fogWallCollider, true);
        yield return new WaitForSeconds(3);
        Physics.IgnoreCollision(player.characterController, fogWallCollider, false);
    }



}
