using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryManager : CharacterInventoryManager
{
    // 오른손이 주무기이며 애니메이션 컨트롤러를 가지고 있음.

    public Weapon currentRightHandWeapon;
    public Weapon currentLeftHandWeapon;

    public WarriorGolemPattern3CircleObject circleObject;


}
