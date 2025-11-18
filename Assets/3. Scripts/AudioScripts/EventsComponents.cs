using JetBrains.Annotations;

[System.Serializable]
public enum SFXType
{
    Player =0,
    WarriorGolem=1,
    Title=2,
    Main=3,
    Lobby=4,
    Background=5,
    Ambience=6,

    Dungeon=7,
}

[System.Serializable]
public enum PlayerSFX
{ 
    Move,
    Dead,
    NormalAttack,
    SpecialAttack1,
    SpecialAttack2,
    Parry,
    Evade,
    Hit,
    HeavyHit,
    Rebirth,
    Dash,
    Buff,
    None,
    CutScene,
    BossGroggy,
    PatternBreak,
    PatternReady,
    BossKill,
    OtherRebirth,
    VNormalAttack01,
    VNormalAttack02,
    VNormalAttack03,
    VNormalAttack04,
    VHit,
    VSpecialAttack01,
    VSpecialAttack02,
    CounterAttack,
    Synergy,
    VDead,
    VParry,
    ReactPullForce,
}

[System.Serializable]
public enum WarriorGolemSFX 
{
    Attack01, 
    Attack02, 
    Attack03, 
    Attack04, 
    Attack05,
    BackAttack01,
    Dash,
    Death,
    Groggy,
    Hit,
    BladeAura,
    SealPlayer,
    Charge,
    ChargeAttack,
    ForcePull,
    SummonMagicCircle,
    SummonPrison,
    Jump,
    PatternStart,
    RangeAttack01,
    Run,
    Walk,
    HeavyHit,
}

[System.Serializable]
public enum TitleSFX
{ 
    BackgroundMusic,
    ButtonClicked,
}

[System.Serializable]
public enum LobbySFX
{ 
    LobbyMusic,
    ButtonClicked,
}

[System.Serializable]
public enum DungeonSFX
{ 
    Background =0,

}