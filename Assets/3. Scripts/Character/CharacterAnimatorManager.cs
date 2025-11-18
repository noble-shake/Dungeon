using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;
// using UnityEditor.Animations;
using UnityEngine;


// 우선 캐릭터 -> 플레이어냐 아니냐 
// 플레이어든 AI든 charactercontroller가 붙어 있음.
// 둘 다 근본적으로 characterController을 활용해서 움직여주고 있음.

// 이동 -> 플레이어의 경우에는 걷기, 뛰기 등의 애니메이션의 루트모션을 활용하고 있지 않음.
// AI의 경우에는 애니메이션 루트모션을 활용하고 있음.
// 
public class CharacterAnimatorManager : NetworkBehaviour
{
    CharacterManager character;

    protected int vertical;
    protected int horizontal;

    [Header("Flags")]
    public bool applyRootMotion = false;

    [Header("실행중인 애니메이션 타입")]
    public AnimationType currentAnimationType;

    [Header("Damage Animations")]
    public string lastDamageAnimationPlayed;

    [Header("미디엄 피격 애니메이션")]
    [SerializeField] string hit_Forward_Medium_01 = "Hit_Forward_Medium_01";
    [SerializeField] string hit_Forward_Medium_02 = "Hit_Forward_Medium_02";
    [SerializeField] string hit_Backward_Medium_01 = "Hit_Backward_Medium_01";
    [SerializeField] string hit_Backward_Medium_02 = "Hit_Backward_Medium_02";
    [SerializeField] string hit_Left_Medium_01 = "Hit_Left_Medium_01";
    [SerializeField] string hit_Left_Medium_02 = "Hit_Left_Medium_02";
    [SerializeField] string hit_Right_Medium_01 = "Hit_Right_Medium_01";
    [SerializeField] string hit_Right_Medium_02 = "Hit_Right_Medium_02";
    public List<string> forward_Medium_Damage = new List<string>();
    public List<string> backward_Medium_Damage = new List<string>();
    public List<string> left_Medium_Damage = new List<string>();
    public List<string> right_Medium_Damage = new List<string>();

    [Header("라이트 피격 애니메이션")]
    [SerializeField] string hit_Forward_Light_01 = "Hit_Forward_Light_01";
    [SerializeField] string hit_Forward_Light_02 = "Hit_Forward_Light_02";
    [SerializeField] string hit_Backward_Light_01 = "Hit_Backward_Light_01";
    [SerializeField] string hit_Backward_Light_02 = "Hit_Backward_Light_02";
    [SerializeField] string hit_Left_Light_01 = "Hit_Left_Light_01";
    [SerializeField] string hit_Left_Light_02 = "Hit_Left_Light_02";
    [SerializeField] string hit_Right_Light_01 = "Hit_Right_Light_01";
    [SerializeField] string hit_Right_Light_02 = "Hit_Right_Light_02";
    public List<string> forward_Light_Damage = new List<string>();
    public List<string> backward_Light_Damage = new List<string>();
    public List<string> left_Light_Damage = new List<string>();
    public List<string> right_Light_Damage = new List<string>();

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");
    }

    protected virtual void Start()
    {
        forward_Medium_Damage.Add(hit_Forward_Medium_01);
        forward_Medium_Damage.Add(hit_Forward_Medium_02);

        backward_Medium_Damage.Add(hit_Backward_Medium_01);
        backward_Medium_Damage.Add(hit_Backward_Medium_02);

        left_Medium_Damage.Add(hit_Left_Medium_01);
        left_Medium_Damage.Add(hit_Left_Medium_02);

        right_Medium_Damage.Add(hit_Right_Medium_01);
        right_Medium_Damage.Add(hit_Right_Medium_02);


        forward_Light_Damage.Add(hit_Forward_Light_01);
        forward_Light_Damage.Add(hit_Forward_Light_02);

        backward_Light_Damage.Add(hit_Backward_Light_01);
        backward_Light_Damage.Add(hit_Backward_Light_02);

        left_Light_Damage.Add(hit_Left_Light_01);
        left_Light_Damage.Add(hit_Left_Light_02);

        right_Light_Damage.Add(hit_Right_Light_01);
        right_Light_Damage.Add(hit_Right_Light_02);
    }

    public string GetRandomAnimationFromList(List<string> animationList)
    {
        List<string> finalList = new List<string>();

        foreach (var item in animationList)
        {
            finalList.Add(item);
        }

        finalList.Remove(lastDamageAnimationPlayed);

        for (int i = finalList.Count - 1; i >= 0; i--)
        {
            if (finalList[i] == null)
            {
                finalList.RemoveAt(i);
            }
        }

        int randomValue = Random.Range(0, finalList.Count);

        return finalList[randomValue];
    }



    // 0.5는 걷기 1은 뛰기 2는 질주
    // 
    public void UpdateAnimatorMovementParameters(float horizontalMovement, float verticalMovement, bool sprinting, float dampTime = 0.1f)
    {
        float snappedHorizontal;
        float snappedVertical;

        if (horizontalMovement > 0 && horizontalMovement <= 0.5f)
            snappedHorizontal = 0.5f;
        else if (horizontalMovement > 0.5f && horizontalMovement <= 1f)
            snappedHorizontal = 1f;
        else if (horizontalMovement < 0 && horizontalMovement >= -0.5f)
            snappedHorizontal = -0.5f;
        else if (horizontalMovement < -0.5f && horizontalMovement >= -1f)
            snappedHorizontal = -1f;
        else
            snappedHorizontal = 0f;

        if (verticalMovement > 0 && verticalMovement <= 0.5f)
            snappedVertical = 0.5f;
        else if (verticalMovement > 0.5f && verticalMovement <= 1f)
            snappedVertical = 1f;
        else if (verticalMovement < 0 && verticalMovement >= -0.5f)
            snappedVertical = -0.5f;
        else if (verticalMovement < -0.5f && verticalMovement >= -1f)
            snappedVertical = -1f;
        else
            snappedVertical = 0f;
        if (sprinting)
            snappedVertical = 2;


        character.animator.SetFloat(horizontal, snappedHorizontal, dampTime, Time.deltaTime);
        character.animator.SetFloat(vertical, snappedVertical, dampTime, Time.deltaTime);

    }

    /// <summary>
    /// 네트워크 상에서 애니메이션을 실행시키는 함수.
    /// </summary>
    /// <param name="animationType"></param>
    /// <param name="targetAnimation"></param>
    /// <param name="isPerformingAction"></param>
    /// <param name="weapon"></param>
    /// <param name="attackType"></param>
    /// <param name="applyRootmotion"></param>
    /// <param name="canRotate"></param>
    /// <param name="canMove"></param>
    /// <param name="transitionNomalizedTime"></param>
    /// <param name="animatorSpeed"></param>
    public virtual void PlayAnimationOnNetwork(
        AnimationType animationType,
        string targetAnimation,
        bool isPerformingAction,
        Weapon weapon = null,
        AttackType attackType = AttackType.None,
        bool applyRootmotion = true,
        bool canRotate = false,
        bool canMove = false,
        bool canGaurd = false,
        float transitionNomalizedTime = 0.2f,
        float animatorSpeed = 1f,
        float damage = 0f,
        float groggyDamage = 0f)
    {
        Debug.Log($"{character.name}에서 {targetAnimation} 공격 애니메이션이 실행됩니다.");

        PlayerManager player = character as PlayerManager;
        character.animator.ResetTrigger("TransitionStart");

        switch (animationType)
        {
            case AnimationType.Attack:

                // 혹시나 공격 애니메이션을 실행하는데 무적상태라면 끄도록 한다.
                character.characterCombatManager.DisableIsInvulnerable();
                character.characterNetworkManager.isAttacking.Value = true;

                // 락온 중이라면 타겟을 향해 회전해야 한다. 가고자 하는 방향과 좀 비슷하다면? 
                if (character.characterNetworkManager.isLockedOn.Value)
                {
                    if (player != null)
                    {
                        Vector3 position = character.characterCombatManager.currentTarget.transform.position;

                        Vector3 direction = character.characterCombatManager.currentTarget.transform.position - character.transform.position;
                        direction.y = 0; // Y축 회전만 고려 (수평 회전)
                        Quaternion targetRotation = Quaternion.LookRotation(direction);
                        Vector3 targetEulerAngles = targetRotation.eulerAngles;

                        // DORotate 사용
                        character.transform.DORotate(targetEulerAngles, 0.1f).SetEase(Ease.Linear);
                        // DoLookAt을 쓰면 트랜스폼 즉시 변화라 몬스터들을 통과함.
                        // character.transform.DOLookAt(position, 0.1f).SetEase(Ease.Linear);
                    }
                }
                else // 락온 중이 아니라면 입력한 방향대로 움직인다.
                {
                    if (player != null)
                    {
                        Debug.Log("회전 함수 실행");
                        //player.playerLocomotionManager.PlayerRotationToWardsInputDirection(0.1f);
                        player.playerLocomotionManager.PlayerRotationToWardsInputDirection(0.05f);
                    }
                }
                // 전사가 대시 후, 특수공격2 후 대시 후 2타 공격이 나가게끔 예외처리해줌.
                if (player?.CharacterClass == PlayerClass.Warrior)
                {
                    if (targetAnimation == "Main_Counter_Attack_01" || targetAnimation == "Main_Light_Attack_04")
                    {
                        character.characterCombatManager.lastAttackAnimationPerformed = "Main_Light_Attack_01";
                        character.animator.CrossFade(targetAnimation, transitionNomalizedTime);
                        break;
                    }
                }
                character.characterCombatManager.lastAttackAnimationPerformed = targetAnimation;
                character.animator.CrossFade(targetAnimation, transitionNomalizedTime);

                break;

            case AnimationType.SpecialAttack:
                character.characterNetworkManager.isAttacking.Value = true;
                character.animator.CrossFade(targetAnimation, transitionNomalizedTime);

                break;
            case AnimationType.Locomotion:
                (character.characterCombatManager as PlayerCombatManager)?.CloseAllDamageCollider();
                character.animator.CrossFade(targetAnimation, transitionNomalizedTime);

                break;

            case AnimationType.Hit:
                character.animator.Play(targetAnimation, 2, transitionNomalizedTime);

                break;

            case AnimationType.Parrying:
                character.characterCombatManager.lastAttackAnimationPerformed = "";
                character.characterNetworkManager.isParrying.Value = true;
                character.animator.Play(targetAnimation, 2, transitionNomalizedTime);

                break;
            case AnimationType.Buff:
                character.animator.CrossFade(targetAnimation, transitionNomalizedTime);
                break;

            case AnimationType.Evade:
                character.animator.Play(targetAnimation, 2, transitionNomalizedTime);
                break;
            default:
                character.animator.CrossFade(targetAnimation, transitionNomalizedTime);
                break;
        }

        (character.characterCombatManager as PlayerCombatManager)?.SetDamageOnDamageCollider(damage, groggyDamage);

        character.animator.speed = animatorSpeed;
        character.characterCombatManager.currentAttackType = attackType;
        character.characterAnimatorManager.applyRootMotion = applyRootmotion;
        character.characterAnimatorManager.currentAnimationType = animationType;

        // 패링 같은 경우 같은 애니메이션 스테이트로 다시 실행해야 하는데, 이 때 CrossFade는 애니메이션을 처음부터 실행하지 않기 때문.
        // if (character.animator.GetCurrentAnimatorStateInfo(2).IsName(targetAnimation))
        // {
        //     character.animator.Play(targetAnimation, 2, transitionNomalizedTime);
        // }
        // else
        // character.animator.CrossFade(targetAnimation, transitionNomalizedTime);
        character.isPerformingAction = isPerformingAction;
        character.characterLocomotionManager.canRotate = canRotate;
        character.characterLocomotionManager.canMove = canMove;

        character.characterNetworkManager.PlayAnimationOnNetworkServerRpc(NetworkManager.Singleton.LocalClientId, animationType, targetAnimation, applyRootmotion, transitionNomalizedTime, animatorSpeed);
    }

    // public void UpdateAnimatorController(AnimatorController weaponController)
    // {
    //     character.animator.runtimeAnimatorController = weaponController;
    // }

}
