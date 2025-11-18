using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Seal Hit Damage")]
public class TakeSealHitEffect : DamageEffect
{
    public override void ProcessEffect(CharacterManager character)
    {
        if (character.characterNetworkManager.isInvulnerable.Value) return;

        if (character.isDead.Value) return;

        // 오너 아니어도 실행 됨.
        PlayDamageSFX(character);
        // 오너 아니어도 실행 됨.
        PlayDamageVFX(character);
        SealHitProcess(character);
    }

    private void SealHitProcess(CharacterManager character)
    {
        if (character.IsOwner == false)
            return;
        character.TryGetComponent(out PlayerManager player);
        player.playerCombatManager.HitCountToRelase--;
        if (player.playerCombatManager.HitCountToRelase <= 0)
        {
            player.playerNetworkManager.isSealed.Value = false;
        }
    }

    public override void ProcessEffectOnLocal(CharacterManager character)
    {
        if (character.characterNetworkManager.isInvulnerable.Value) return;

        if (character.isDead.Value) return;

        PlayDamageSFX(character);

        PlayDamageVFX(character);

    }
}
