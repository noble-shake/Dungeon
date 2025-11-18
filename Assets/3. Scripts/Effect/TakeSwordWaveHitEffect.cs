using UnityEngine;
[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take SwordWave Hit Damage")]

public class TakeSwordWaveHitEffect : DamageEffect
{
    public override void ProcessEffect(CharacterManager damagedCharacter)
    {
        if (damagedCharacter.characterNetworkManager.isInvulnerable.Value) return;

        if (damagedCharacter.isDead.Value) return;

        // 오너 아니어도 실행 됨.
        PlayDamageSFX(damagedCharacter);
        // 오너 아니어도 실행 됨.
        PlayDamageVFX(damagedCharacter);

        // 오너만 실행한다.
        CalculateDamage(damagedCharacter);

    }

    public override void ProcessEffectOnServer(CharacterManager character)
    {
        if (character.characterNetworkManager.isInvulnerable.Value) return;

        if (character.isDead.Value) return;

    }

    public override void ProcessEffectOnLocal(CharacterManager character)
    {
        if (character.characterNetworkManager.isInvulnerable.Value) return;

        if (character.isDead.Value) return;

        PlayDamageSFX(character);

        PlayDamageVFX(character);


    }

    // 여기에 따로 체력 조건을 걸어줘도 됨. 
    private void CalculateDamage(CharacterManager character)
    {
        if (!character.IsOwner) return;


        finalDamageDealt = Mathf.RoundToInt(physicalDamage);

        if (finalDamageDealt <= 0)
        {
            finalDamageDealt = 1;
        }
        Debug.Log($"가한 사람 {characterCausingDamage?.name}, 받은 사람 {character.name}, 데미지 : {finalDamageDealt}");

        character.characterCombatManager.poiseResetTimer = character.characterCombatManager.defaultPoiseResetTime;

        AIWarriorGolemCharacterManager boss = character as AIWarriorGolemCharacterManager;
        if (boss != null)
            boss.swordWavePattern.ParryiedWaveHitCount++;
        PlayerManager player = character as PlayerManager;
        if (player != null)
        {
            // 봉인 해제.
            player.playerCombatManager.HitCountToRelase--;
            if (player.playerCombatManager.HitCountToRelase <= 0)
            {
                // player.playerNetworkManager.isPartiallyInvulnerable.Value = false;
                // sealed가 서버 권한이라 RPC로 풀어준다. 
                player.playerNetworkManager.SetIsSealedServerRpc(false);
            }
        }

    }
}
