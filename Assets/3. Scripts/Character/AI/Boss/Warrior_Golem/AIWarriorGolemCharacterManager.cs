using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AIWarriorGolemCharacterManager : AIBossCharacterManager
{
    private AIWarriorGolemCombatManager combatManager;

    [HideInInspector] public SwordWavePattern swordWavePattern;
    [HideInInspector] public Shockwave shockwave;

    [HideInInspector] public TornadoPattern tornadoPattern;
    [HideInInspector] public EmblemPattern emblemPattern;
    protected override void Awake()
    {
        base.Awake();
        combatManager = GetComponent<AIWarriorGolemCombatManager>();
        swordWavePattern = GetComponent<SwordWavePattern>();
        tornadoPattern = GetComponent<TornadoPattern>();
        shockwave = GetComponent<Shockwave>();
        emblemPattern = GetComponent<EmblemPattern>();

    }


    protected override void Update()
    {
        base.Update();
    }

    public override void ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        base.ProcessDeathEvent(manuallySelectDeathAnimation);
    }


}
