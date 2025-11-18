using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Boss1 Prison Hit Damage")]
public class TakeBoss1PrisonEffect : DamageEffect
{
    public override void ProcessEffect(CharacterManager character)
    {

        if (character.isDead.Value == true) return;

        // 오너 아니어도 실행 됨.
        //PlayDamageSFX(character);
        // 오너 아니어도 실행 됨.
        //PlayDamageVFX(character);
        PrisonHitProcess(character);
    }

    private void PrisonHitProcess(CharacterManager character)
    {
        if (character.IsOwner == false)
            return;
        character.TryGetComponent(out WarriorGolemPattern3PrisonObject prisonPillar);
        if (prisonPillar == null) return;

        CalculateDamage(character);

    }

    public override void ProcessEffectOnLocal(CharacterManager character)
    {
        if (character.characterNetworkManager.isInvulnerable.Value) return;

        if (character.isDead.Value) return;

        PlayDamageSFX(character);

        PlayDamageVFX(character);

    }

    private void CalculateDamage(CharacterManager character)
    {
        if (!character.IsOwner) return;

        finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lighteningDamage + holyDamage);

        if (finalDamageDealt <= 0)
        {
            finalDamageDealt = 1;
        }
        Debug.Log("final Damage : " + finalDamageDealt);
        character.TryGetComponent(out WarriorGolemPattern3PrisonObject prisonPillar);
        if (prisonPillar == null) return;

        prisonPillar.CurrentHP.Value -= finalDamageDealt;

    }

}
