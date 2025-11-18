using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "A.I/Actions/AIAttackAction")]
public class AICharacterAttackAction : ScriptableObject
{
    [Header("Attack")]
    [SerializeField] private string attackAnimation;

    [Header("Combo Action")]
    public AICharacterAttackAction comboAction; // 이 다음에 올 수 있는 콤보 공격

    [Header("Action Values")]
    [SerializeField] AttackType attackType;
    public int[] attackDamageList;
    public int attackWeight = 50; // 숫자가 클수록 이 액션이 선택될 확률이 증가함. 
    [Tooltip("공격 애니메이션의 끝에서 다음 액션을 실행하기 전까지의 시간")]
    public float actionRecoveryTime = 1f;
    public float minimumAttackAngle = -35f;
    public float maximumAttackAngle = 35f;
    public float minimumAttackDistance = 0f;
    public float maximumAttackDistance = 2f;
    public float animatorSpeed = 1f;
    public float transitionNomalizedTime = 0.2f;

    // 이 공격이 속하는 공격 그룹.
    public AttackGroup attackGroup = AttackGroup.None;

    public string AttackAnimation { get => attackAnimation; set => attackAnimation = value; }

    public void AttempToPerformAction(AICharacterManager aiCharacter)
    {
        aiCharacter.characterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Attack, AttackAnimation, true,
        attackType: attackType,
        transitionNomalizedTime: transitionNomalizedTime,
        animatorSpeed: animatorSpeed);
    }
}
