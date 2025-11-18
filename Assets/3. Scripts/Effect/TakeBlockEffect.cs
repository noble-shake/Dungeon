using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Blocked Damage")]
public class TakeBlockEffect : DamageEffect
{
    public override void ProcessEffect(CharacterManager character)
    {
        if (character.characterNetworkManager.isInvulnerable.Value) return;
        base.ProcessEffect(character);

        if (character.isDead.Value) return;

        Debug.Log("방어 이펙트 프로세스 실행");

        CalculateDamage(character);

        CalculateStaminaDamage(character);

        PlayGuardAnimationBasedOnPoiseDamage(character);

        CheckForGuardBreak(character);

        PlayDamageSFX(character);

        PlayDamageVFX(character);
    }

    private void CalculateDamage(CharacterManager character)
    {
        if (!character.IsOwner) return;


        if (characterCausingDamage != null)
        {

        }

        physicalDamage -= physicalDamage * (character.characterStatsManager.blockingPhysicalAbsorption / 100);

        // 일반적으로 가드 시 물리 데미지는 0으로 고정한다.
        physicalDamage = 0;

        magicDamage -= magicDamage * (character.characterStatsManager.blockingMagicAbsorption / 100);
        fireDamage -= fireDamage * (character.characterStatsManager.blockingFireAbsorption / 100);
        lighteningDamage -= lighteningDamage * (character.characterStatsManager.blockingLighteningAbsorption / 100);
        holyDamage -= holyDamage * (character.characterStatsManager.blockingHolyAbsorption / 100);


        finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lighteningDamage + holyDamage);

        if (finalDamageDealt <= 0)
        {
            finalDamageDealt = 1;
        }

        character.characterNetworkManager.currentHp.Value -= finalDamageDealt;

        Debug.Log("입은 데미지 : " + finalDamageDealt);
    }

    private void CalculateStaminaDamage(CharacterManager character)
    {
        if (!character.IsOwner)
            return;

        finalStaminaDamage = baseStaminaDamage;

        float staminaDamageAbsorption = finalStaminaDamage * (character.characterStatsManager.blockingStability / 100);
        float staminaDamageAfterAbsorption = finalStaminaDamage - staminaDamageAbsorption;

        character.characterNetworkManager.currentStamina.Value -= staminaDamageAfterAbsorption;

        Debug.Log("소모된 스태미나 : " + staminaDamageAfterAbsorption);
    }

    private void PlayGuardAnimationBasedOnPoiseDamage(CharacterManager character)
    {
        if (!character.IsOwner) return;

        if (character.isDead.Value) return;

        DamageIntensity damageIntensity = WorldUtilityManager.instance.GetDamageIntensityBasedOnPoiseDamage(groggyDamage);

        switch (damageIntensity)
        {
            case DamageIntensity.Light:
                damageAnimation = "Block_Normal_01";
                break;
            case DamageIntensity.Heavy:
                damageAnimation = "Block_Normal_01";
                break;
            default:
                damageAnimation = "Block_Normal_01";
                break;
        }

        character.characterAnimatorManager.lastDamageAnimationPlayed = damageAnimation;
        character.characterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Hit, damageAnimation, true);
    }

    private void CheckForGuardBreak(CharacterManager character)
    {
        // TODO : 사운드 재생

        if (!character.IsOwner) return;

        // 가드할 때마다 스태미나가 닳는데 이 때 스태미나가 0 이하가 되면 바로 가드브레이크 애니메이션이 발동한다.
        if (character.characterNetworkManager.currentStamina.Value <= 0)
        {
            character.characterAnimatorManager.PlayAnimationOnNetwork(AnimationType.Hit, "Guard_Break_01", true);
            character.characterNetworkManager.isGaurding.Value = false;
        }
    }


}
