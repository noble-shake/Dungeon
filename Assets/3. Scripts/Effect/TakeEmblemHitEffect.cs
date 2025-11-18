using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Emblem Hit Damage")]
public class TakeEmblemHitEffect : DamageEffect
{
    private bool isPerfomingAction;
    private bool disableCollider;
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

        AIWarriorGolemCharacterManager boss = character as AIWarriorGolemCharacterManager;

        finalDamageDealt = Mathf.RoundToInt(physicalDamage);

        if (finalDamageDealt <= 0)
        {
            finalDamageDealt = 1;
        }
        Debug.Log($"가한 사람 {characterCausingDamage?.name}, 받은 사람 {character.name}, 데미지 : {finalDamageDealt}");

        character.characterCombatManager.poiseResetTimer = character.characterCombatManager.defaultPoiseResetTime;

        // 사실 이 조건이 때리는 순간 체크 되어야 하는 게 맞음.
        if (boss.emblemPattern.emblemSettingReady)
        {
            Debug.Log("세팅이 됐습니다.");
            PlayerManager player = characterCausingDamage as PlayerManager;
            if (boss.emblemPattern.CheckValidClass((int)player.characterClass))
            {
                Debug.Log("클래스가 일치합니다.");
                boss.emblemPattern.RemoveIcon((int)player.characterClass);
                character.characterNetworkManager.currentHp.Value -= finalDamageDealt;
            }
            else
            {
                Debug.Log("클래스가 일치하지 않습니다.");
                character.characterNetworkManager.currentHp.Value += 200;
            }
        }
        else
        {
            Debug.Log("세팅이 아직 안 됐습니다.");
        }
    }

}
