using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIUndeadManager : AICharacterManager
{
    [Header("언데드 타입")]
    public MonsterType type = MonsterType.None; //인스펙터에서 할당이 이루어져야 함.

    protected override void Awake()
    {
        base.Awake();
        if (type == MonsterType.None)
        {
            Debug.LogError("타입에 대한 할당이 이루어지지 않았습니다.");
        }
    }

}
