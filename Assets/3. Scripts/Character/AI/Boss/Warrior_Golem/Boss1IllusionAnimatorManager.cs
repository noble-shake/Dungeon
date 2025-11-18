using System.Collections;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

public class Boss1illusionAnimatorManager : AICharacterAnimatorManager
{
    AICharacterManager aiBossCharacter;
    public Vector3 OriginPos;
    public bool isProjectileThrow;


    protected override void Awake()
    {
        base.Awake();

        aiBossCharacter = GetComponent<AICharacterManager>();
    }

    protected override void OnAnimatorMove()
    {
        if (aiBossCharacter.aiCharacterAnimatorManager.applyRootMotion)
        {
            AnimatorClipInfo[] clipInfo = aiBossCharacter.animator.GetCurrentAnimatorClipInfo(2); // Wheelwind

            if (clipInfo.Length != 0)
            {
                if (clipInfo[0].clip.name == "GreatSword_Whirlwind_Loop_Root")
                {

                    Vector3 targetPos;
                    if (GetComponent<BehaviorGraphAgent>().BlackboardReference.GetVariableValue("TargetEdge", out targetPos))
                    {
                        aiBossCharacter.characterController.Move(aiBossCharacter.transform.forward * Time.deltaTime * ((OriginPos - targetPos) / 5f).magnitude);
                        aiBossCharacter.transform.rotation *= aiBossCharacter.animator.deltaRotation;

                        if (isProjectileThrow == false && Vector3.Distance(aiBossCharacter.transform.position, Vector3.Lerp(OriginPos, targetPos, 0.5f)) < 1f)
                        {
                            Debug.Log("Projecttile Throw!!!");
                            isProjectileThrow = true;
                        }

                    }


                }
            }


        }

    }
}
