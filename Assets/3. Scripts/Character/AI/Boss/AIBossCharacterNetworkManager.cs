using System;
using System.Collections;
using System.Collections.Generic;
using TeleportFX;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class AIBossCharacterNetworkManager : AICharacterNetworkManager
{
    AIBossCharacterManager boss;
    public NetworkVariable<bool> bossFightIsActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> rageMode = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    // public NetworkVariable<bool> isPartiallyInvulnerable = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> gimmickHitCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public bool isGimmickInProcess = false;
    private GameObject pooledObject;

    [SerializeField, Range(0f, 1f)] float Phase1Ratio;
    [SerializeField, Range(0f, 1f)] float Phase2Ratio;
    [SerializeField, Range(0f, 1f)] float Phase3Ratio;
    bool isPhase1Played;
    bool isPhase2Played;
    bool isPhase3Played;

    public void OnRageModeChange(bool oldValue, bool newValue)
    {
        if (newValue == true)
            boss.animator.SetBool("SecondPhase", newValue);
    }

    protected override void Awake()
    {
        base.Awake();

        boss = GetComponent<AIBossCharacterManager>();
    }

    public override void OnIsGroggyChanged(bool previousValue, bool newValue)
    {
        if (newValue == true)
        {
            pooledObject = ObjectPoolManager.Singleton.GetObject(ObjectPoolType.Hit, 10);
            pooledObject.transform.SetParent(boss.transform);
            pooledObject.transform.localPosition = Vector3.up * 1.5f;
        }
        else
        {
            if (pooledObject != null)
            {
                pooledObject.transform.SetParent(null);
                ObjectPoolManager.Singleton.ReturnObject(pooledObject, ObjectPoolManager.Singleton.GetPrefab(ObjectPoolType.Hit, 10));
                pooledObject = null;
            }
        }
    }

    /// <summary>
    /// 보스 BGM 실행 및 HP Bar UI 생성하여 HUD에 추가함.
    /// </summary>
    /// <param name="oldStatus"></param>
    /// <param name="newStatus"></param>
    public void OnBossFightIsActiveChanged(bool oldStatus, bool newStatus)
    {
        if (bossFightIsActive.Value)
        {
            // WorldSoundFXManager.instance.PlayBossTrack(bossIntroClip, bossBattleLoopClip);

            // HUD에 보스 UI를 활성화 및 함수 연결
            HUD_UIManager.instance.combatUIManager.EnableBossUI(boss);

        }
        else
        {
            HUD_UIManager.instance.combatUIManager.DisableBossUI(boss);
            // WorldSoundFXManager.instance.StopBossMusic();
        }
    }


    public void SetNewMaxHpValueOnUIBar(int oldValue, int newValue)
    {
        HUD_UIManager.instance.combatUIManager.SetMaxBossHpValue(maxHp.Value);
    }

    public override void CheckHP(int oldValue, int newValue)
    {
        if (currentHp.Value <= 0.15f * maxHp.Value)
        {
            if (Camera.main.TryGetComponent<DungeonAudioManager>(out DungeonAudioManager dungeonAudio)) dungeonAudio.Dungeon01SFXAdjustGimmikState(DungeonAudioManager.Dungeon01BackgroundSFX.LastGimmick);

        }
        else if (currentHp.Value <= 0.45f * maxHp.Value)
        {
            if (Camera.main.TryGetComponent<DungeonAudioManager>(out DungeonAudioManager dungeonAudio)) dungeonAudio.Dungeon01SFXAdjustGimmikState(DungeonAudioManager.Dungeon01BackgroundSFX.Gimmick3);

        }
        else if (currentHp.Value <= 0.65f * maxHp.Value)
        {
            if (Camera.main.TryGetComponent<DungeonAudioManager>(out DungeonAudioManager dungeonAudio)) dungeonAudio.Dungeon01SFXAdjustGimmikState(DungeonAudioManager.Dungeon01BackgroundSFX.Gimmick2);
        }
        else if (currentHp.Value <= 0.85f * maxHp.Value)
        {
            if (Camera.main.TryGetComponent<DungeonAudioManager>(out DungeonAudioManager dungeonAudio)) dungeonAudio.Dungeon01SFXAdjustGimmikState(DungeonAudioManager.Dungeon01BackgroundSFX.Gimmick1);
        }

        // base.CheckHP(oldValue, newValue);
        if (currentHp.Value <= 0)
        {
            boss.ProcessDeathEvent();
        }


        if (boss.IsOwner)
        {
            if (currentHp.Value > maxHp.Value)
            {
                currentHp.Value = maxHp.Value;
            }
        }


        if (boss.IsOwner)
        {
            if (currentHp.Value <= 0)
                return;

            //float healthNeededForPhaseShift = maxHp.Value * (bossCharacter.minimumHealthPercentageToShift / 100);

            //if (currentHp.Value <= healthNeededForPhaseShift)
            //{
            //    bossCharacter.PhaseShift();
            //}

            PhaseShiftByRatio();
        }
    }

    private void PhaseShiftByRatio()
    {

        if (isPhase1Played != true && currentHp.Value <= Phase1Ratio * maxHp.Value)
        {
            FreeGroggyTriggerServerRpc();
            isPhase1Played = true;
            boss.PhaseShift(PhaseType.Phase1);
            rageMode.Value = true;
            return;
        }

        if (isPhase2Played != true && currentHp.Value <= Phase2Ratio * maxHp.Value)
        {
            FreeGroggyTriggerServerRpc();
            boss.animator.ResetTrigger("GroggyTrigger");
            isPhase2Played = true;
            boss.PhaseShift(PhaseType.Phase2);
            return;
        }

        //if (isPhase3Played != true && currentHp.Value <= Phase3Ratio * maxHp.Value)
        //{

        //    isPhase3Played = true;
        //    boss.PhaseShift(PhaseType.Phase3);
        //    rageMode.Value = true;
        //    return;
        //}
    }

    [ServerRpc]
    public void FreeGroggyTriggerServerRpc()
    {
        FreeGroggyTriggerClientRpc();
    }

    [ClientRpc]
    public void FreeGroggyTriggerClientRpc()
    {
        boss.animator.ResetTrigger("GroggyTrigger");
    }

    [ServerRpc]
    public void TeleportDisappearServerRpc(bool disappear = true)
    {
        TeleportClientRpc(disappear);
    }

    [ClientRpc]
    private void TeleportClientRpc(bool disappear = true)
    {
        TryGetComponent(out KriptoFX_Teleportation teleportationComponent);
        if (disappear)
        {
            boss.characterController.enabled = false;
            teleportationComponent.TeleportationState = KriptoFX_Teleportation.TeleportationStateEnum.Disappear;
        }
        else
        {
            boss.characterController.enabled = true;
            teleportationComponent.TeleportationState = KriptoFX_Teleportation.TeleportationStateEnum.Appear;
        }
    }
}
