using System.Globalization;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Boss1 MagicCircle Hit Damage")]
public class TakeBoss1MagicCircleEffect : DamageEffect
{
    public override void ProcessEffect(CharacterManager character)
    {

        if (character.isDead.Value) return;

        // 오너 아니어도 실행 됨.
        PlayDamageSFX(character);
        // 오너 아니어도 실행 됨.
        PlayDamageVFX(character);
        MagicCircleHitProcess(character);
    }

    private void MagicCircleHitProcess(CharacterManager character)
    {
        if (character.IsOwner == false)
            return;
        character.TryGetComponent(out WarriorGolemPattern3Component boss);
        if (boss == null) return;

        float tossVal = Random.Range(0f, 1f);
        if (tossVal < 0.3f) return;

        boss.DropServerRpc(character.transform.position + Vector3.up);




        //circleObject.SetObjectServerRpc();


    }

    public override void ProcessEffectOnLocal(CharacterManager character)
    {
        if (character.characterNetworkManager.isInvulnerable.Value) return;

        if (character.isDead.Value) return;

        PlayDamageSFX(character);

        PlayDamageVFX(character);

    }
}
