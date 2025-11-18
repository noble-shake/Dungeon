using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class NonCharacterManager : CharacterManager
{
    // [HideInInspector] public new Rigidbody rigidbody;


    protected override void Start()
    {
        // IgnoreMyOwnColliders();
    }

    protected override void Update()
    {

    }

    // OnEnable
    // OnDisable

    public override void OnNetworkSpawn()
    {
        //base.OnNetworkSpawn();

        //// 밑에 델리게이트 걸어놓음에도 값을 세팅해주는 이유는, 맨 처음 호스트에 접속했을 때의 상태 초기화를 해주기 위함임.
        //animator.SetBool("isMoving", characterNetworkManager.isMoving.Value);

        //// 
        //characterNetworkManager.OnIsActiveChanged(false, characterNetworkManager.isActive.Value);

        //characterNetworkManager.isMoving.OnValueChanged += characterNetworkManager.OnIsMovingChanged;
        //characterNetworkManager.isActive.OnValueChanged += characterNetworkManager.OnIsActiveChanged;

        //if (IsOwner)
        //{
        //    // 체력 레벨이 변했을 시 거기에 맞게 최대 체력 설정
        //    // characterNetworkManager.vitality.OnValueChanged += characterNetworkManager.SetNewMaxHealthValue;
        //    // 지구력 레벨이 변했을 시 거기에 맞춰 최대 스태미나 설정
        //    // characterNetworkManager.enduracne.OnValueChanged += characterNetworkManager.SetNewMaxStaminaValue;

        //    // Debug.Log("캐릭터의 초기 스탯을 설정합니다.");
        //    // Debug.Log($"체력 : {characterStatsManager.vatality}, 스태미나 : {characterStatsManager.endurance}");

        //    // characterNetworkManager.vitality.Value = characterStatsManager.vatality;
        //    // characterNetworkManager.enduracne.Value = characterStatsManager.endurance;
        //}
    }
    public override void OnNetworkDespawn()
    {
        //base.OnNetworkDespawn();

        //characterNetworkManager.isActive.OnValueChanged -= characterNetworkManager.OnIsActiveChanged;
        //characterNetworkManager.isMoving.OnValueChanged -= characterNetworkManager.OnIsMovingChanged;
    }


}
