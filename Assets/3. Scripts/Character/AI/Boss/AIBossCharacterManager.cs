using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Netcode;
using UnityEngine;

public class AIBossCharacterManager : AICharacterManager
{
    [SerializeField] string bossDeadText = "The Night Died";
    public string bossName = "The Night";

    [HideInInspector]
    public AIBossCharacterAnimatorManager bossAnimatorManager;

    [HideInInspector]
    public AIBossCharacterNetworkManager bossNetworkManager;

    [HideInInspector]
    public AIBossCharacterCombatManager bossCombatManager;

    [HideInInspector]
    public AIBossUIManager bossUIManager;


    // ID 부여
    public int bossID = 0;

    [Header("Music")]
    [SerializeField] AudioClip bossIntroClip;
    [SerializeField] AudioClip bossBattleLoopClip;

    [Header("Status")]
    public NetworkVariable<bool> hasBeenAwakened = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> hasBeenDefeated = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] List<FogWallInteractable> fogWalls;
    [SerializeField] string sleepAnimation;
    [SerializeField] string awakenAnimation;

    // 스폰됐을 때 세이브파일 체크
    // 세이브파일이 없으면 만들고
    // 있으면 깬 적 있는지 체크
    // 깬 적 있으면 비활성화
    // 깬 적 없으면 활성화

    [Header("TEST")]
    [SerializeField] bool defeatedBossDebug = false;
    [SerializeField] bool wakeBossUp = false;
    [SerializeField] bool phaseShift = false;

    protected override void Awake()
    {
        base.Awake();

        bossAnimatorManager = GetComponent<AIBossCharacterAnimatorManager>();
        bossNetworkManager = GetComponent<AIBossCharacterNetworkManager>();
        bossCombatManager = GetComponent<AIBossCharacterCombatManager>();
        bossUIManager = GetComponent<AIBossUIManager>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        bossNetworkManager.bossFightIsActive.OnValueChanged += bossNetworkManager.OnBossFightIsActiveChanged;
        // 클라이언트가 뒤늦게 접속될 때, 네트워크 오브젝트의 상태변화에 따라 발생해야될 이벤트가 발생되지 않은 상태다.
        // 그렇기 때문에 수동으로 발생시켜 준다. 
        // 원래 접속해 있는 상태였으면, OnBossFightIsActiveChanged 함수가 상태 변화에 따라 자동으로 발생했겠지만
        // 뒤늦게 접속한 클라이언트는 상태 변화를 겪지 않으므로 자동으로 발생하지 않고, 수동으로 발생시켜 줘야 한다.

        // 현재는 중간 접속을 허락하지 않기 때문에 이 코드가 의미 없다.
        bossNetworkManager.OnBossFightIsActiveChanged(false, bossNetworkManager.bossFightIsActive.Value);


        // 최대 체력이 변했을 시 거기에 맞게 최대 체력 설정
        bossNetworkManager.maxHp.OnValueChanged += bossNetworkManager.SetNewMaxHealthValue;
        bossNetworkManager.maxHp.OnValueChanged += bossNetworkManager.SetNewMaxHpValueOnUIBar;


        bossNetworkManager.currentHp.OnValueChanged += HUD_UIManager.instance.combatUIManager.SetNewBossHpValue; // 체력이 변화할 때 HUD UI 업데이트 해줌.
        //  bossNetworkManager.currentHp.OnValueChanged += DungeonAudioManager.Instance.SFXAdjustBossHP; // 체력이 변화할 때 HUD UI 업데이트 해줌.
        // 보스 몬스터의 바이탈리티가 변했을 때의 일 


        // PlayerUIManager.instance.playerUIHudManager.SetMaxBossHpValue(playerNetworkManager.maxStamina.Value);
        // PlayerUIManager.instance.playerUIHudManager.SetNewBossHpValue(playerNetworkManager.maxHealth.Value);

        // 그로기 시 보스 머리 위에 별 이펙트 추가가
        bossNetworkManager.isGroggy.OnValueChanged += bossNetworkManager.OnIsGroggyChanged;

        if (IsOwner)
        {
            bossNetworkManager.maxHp.Value = characterCombatManager.hp;
            GameManager.Instance.SetCurrentBoss(this);
        }
        else
        {
            bossNetworkManager.SetNewMaxHealthValue(0, bossNetworkManager.maxHp.Value);
            bossNetworkManager.SetNewMaxHpValueOnUIBar(0, bossNetworkManager.maxHp.Value);

        }

        
        if (Camera.main.TryGetComponent<DungeonAudioManager>(out DungeonAudioManager dungeonAudio)) dungeonAudio.PlayerMaxHP = bossNetworkManager.maxHp.Value;



    }


    private IEnumerator GetFogWallsFromWorldObjectManager()
    {
        while (WorldObjectManager.instance.fogWalls.Count == 0)
            yield return new WaitForEndOfFrame();

        fogWalls = new();

        foreach (var fogWall in WorldObjectManager.instance.fogWalls)
        {
            if (fogWall.fogWallID == bossID)
            {
                fogWalls.Add(fogWall);
            }
        }
    }

    // 할일 : 죽었을 때 무기를 떨구는 로직 추가해야 됨. 
    public override void ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        base.ProcessDeathEvent();

        HUD_UIManager.instance.popUpUIManager.SendBossDefeatedPopUp(bossDeadText, ScriptColor.Red);
        if (IsOwner)
        {

            bossNetworkManager.bossFightIsActive.Value = false;

            foreach (var fogWall in fogWalls)
            {
                fogWall.isActive.Value = false;
            }

            hasBeenDefeated.Value = true;

            // 보스가 죽었을 때 문구를 띄워주고 게임을 끝
            GameManager.Instance.GameEnd();

        }

    }

    protected override void Update()
    {
        base.Update();
    }

    public void WakeBoss()
    {
        if (IsOwner)
        {
            if (!hasBeenAwakened.Value)
            {
                if (awakenAnimation != string.Empty)
                    characterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Etc, awakenAnimation, true);
            }
            hasBeenAwakened.Value = true;

            if (!WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
            {
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
            }
            else
            {
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Remove(bossID);
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
            }

            foreach (var fog in fogWalls)
            {
                fog.isActive.Value = true;
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        bossNetworkManager.bossFightIsActive.OnValueChanged -= bossNetworkManager.OnBossFightIsActiveChanged;
    }



    public virtual void PhaseShift(PhaseType _phase)
    {
        // 잠시 이 때 무적이어야 함. 애니메이션에 무적 판정 걸어줄 것. -> 근데 언제 풀어주지?
        // bossCharacterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Hit, phaseShiftAnimation, true);
        // currentState = animationPlayState;
        // bossCharacterCombatManager.ChangeAttackListWithPhaseShift();
        switch (_phase)
        {
            default:
            case PhaseType.Phase1:
                GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("PhaseTypeVariable", PhaseType.Phase1);
                break;
            case PhaseType.Phase2:
                GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("PhaseTypeVariable", PhaseType.Phase2);
                break;
            case PhaseType.Phase3:
                GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("PhaseTypeVariable", PhaseType.Phase3);
                break;
        }

        GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("State", EnemyState.PhaseShift);
        GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("ImprovedEnemyState", ImprovedEnemyState.PhaseShift);
    }

    public void PlayCutScene()
    {
        // 비해이비어 상태를 컷신 상태로 전환, 컷신 상태에서는 애니메이션이 실행되는 동안 다른 상태로 변경 불가.
        GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("ImprovedEnemyState", ImprovedEnemyState.CutScenePlaying);

    }

    protected override void OnEnable()
    {

    }
    protected override void OnDisable()
    {
    }
}
