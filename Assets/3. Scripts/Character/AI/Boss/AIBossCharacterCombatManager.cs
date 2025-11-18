using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

public class AIBossCharacterCombatManager : AICharacterCombatManager
{
    private AIBossCharacterManager bossCharacter;
    [SerializeField] private bool isAggroFixedToPlayer = false;
    [SerializeField] private int[] hpPerPlayerCount;
    [SerializeField] private int[] groggyGaugePerPlayerCount;



    protected override void Awake()
    {
        base.Awake();
        bossCharacter = GetComponent<AIBossCharacterManager>();

        // GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("MapCenter", originPosition);
    }

    public override void SetStanceBroken()
    {
        bossCharacter.bossNetworkManager.isStanceBroken.Value = true;
    }

    public override void ResetStanceBroken()
    {
        bossCharacter.bossNetworkManager.isStanceBroken.Value = false;
    }

    // 보스가 기믹 진행중이라면 통하지 않는다.
    // 보스가 
    public async UniTaskVoid SetAggroFixedToplayer(PlayerManager player, float interceptAggroTime)
    {
        if (isAggroFixedToPlayer) return;
        if (bossCharacter.bossNetworkManager.isGimmickInProcess == true) return;
        isAggroFixedToPlayer = true;
        bossCharacter.GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("FixedTarget", player);

        bossCharacter.bossCombatManager.currentTarget = player;
        bossCharacter.GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("ImprovedEnemyState", ImprovedEnemyState.NormalPatternWithFixedAggro);
        await UniTask.Delay(TimeSpan.FromSeconds(interceptAggroTime));
        // 시간이 흘러 보스가 기믹 진행중이라면 실행하지 않는다.
        if (bossCharacter.bossNetworkManager.isGimmickInProcess == true)
        {
            isAggroFixedToPlayer = false;
            return;
        }
        bossCharacter.GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue("ImprovedEnemyState", ImprovedEnemyState.Idle);
        isAggroFixedToPlayer = false;

    }

    public void SetHpAndGroggyGaugeByPlayerCount(int count)
    {
        count -= 1;
        Debug.Log("입장한 플레이어 수 : " + count);
        bossCharacter.bossNetworkManager.maxHp.Value = hpPerPlayerCount[count];
        bossCharacter.bossCombatManager.maxGroggy = groggyGaugePerPlayerCount[count];
        bossCharacter.bossCombatManager.currentGroggy = groggyGaugePerPlayerCount[count];
    }
}
