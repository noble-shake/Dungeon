using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

public class Enums : MonoBehaviour
{

}

public enum PlayerClass
{
    Knight = 0,
    Warrior = 1,
    Thief = 2,
    //Mage = ,
    Archer = 3,
    Selecting = 4,
}

public enum CharacterType
{
    Player,
    Monster,
    Object,
}


public enum CharacterSlot
{
    CharacterSlot_01 = 1,
    CharacterSlot_02 = 2,
    CharacterSlot_03 = 3,
    CharacterSlot_04 = 4,
    CharacterSlot_05 = 5,
    CharacterSlot_06 = 6,
    CharacterSlot_07 = 7,
    CharacterSlot_08 = 8,
    CharacterSlot_09 = 9,
    CharacterSlot_10 = 10,
    NO_SLOT
}

public enum WeaponModelSlot
{
    RightHandWeaponSlot,
    LeftHandWeaponSlot,
    LeftHandShieldSlot
}

public enum WeaponModelType
{
    Weapon,
    Shield
}

public enum AnimationType
{
    None,
    Locomotion,
    Attack,
    SpecialAttack,
    Hit,
    Parrying,
    Buff,
    Etc,
    Evade,
}

public enum ActionType
{
    Gaurd,
    Attack,
    Evade,
    Buff
}
public enum CombatStatus
{
    Normal,
    DoingSomething,
    Attacking,
    RockOn_Normal,
    RockOn_Attacking,
    RockOn_DoingSomething,
}

public enum AttackType
{
    LightAttack,
    HeavyAttack,
    // 맞는 순간 SetMotion 발동? 
    SpecialAttack,
    SealAttack,
    RangedAttack,
    ObjectAttack,
    PrisonAttack,
    None,
    LastAttack,
}

public enum InteractionType
{
    Player,
    Etc
}

public enum DamageIntensity
{
    Light,
    Heavy,
    Deadly
}

public enum MonsterType
{
    None,
    Normal,
    Rare,
    Elite,
    Legendary,
    Boss
}

public enum GameLaunchStatus
{
    WaitingForPlayersToConnect,
    WaitingForPlayersToInitialize,
    WaitingForPlayersResponses,
    WaitingForJoin,
    ReadyToLaunch,
    UnableToLaunch
}

public enum SceneIndex
{
    Scene_Booting,
    Scene_Loading,
    Scene_Lobby,
    Scene_Dungeon_01
}

public enum GameState
{
    Idle,
    WaitForReady,
    GameStart,
    InGame,
    GameEnd
}

public enum DamageEffectType
{
    Hit = 0,
    InEvitableHit,
    SealHit,
    MagicCircleHit,
    PrisonHit,
    EmblemHit,
    SwordWaveHit,
}

public enum Direction
{
    F,
    B,
    L,
    R,
    FR,
    FL,
    BR,
    BL
}

public enum ScriptColor
{
    Red,
    Blue,
    White,
    Black,
}
[BlackboardEnum]
public enum Boss1Pattern3Phase
{
    Prepare,
    ForcePull,
    Chase,
    Attack,
    SummonMagicCircle,
    Circlebreak,
    SummonPrison,
    Break,
    NormalAttack,

}



// NOTE: LEGACY STATE
[BlackboardEnum]
public enum Boss1PentagramPattern
{
    Prepare,
    Targetting,
    Attacking,
    Rush,
    Break,
}

[BlackboardEnum]
public enum Boss1IllusionPattern
{
    Prepare,
    Wait,
    Move,
    Break,
}

public enum ObjectPoolType
{
    Hit,
    Explosion,
    Projectile,
    Seal,
    FirstBoss,
    Buff,
}

public enum InputType
{
    Shift,
    Q,
    E,
    R,
    Space,
    RightClick,
}