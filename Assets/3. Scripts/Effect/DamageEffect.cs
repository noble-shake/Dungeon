using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : InstantEffect
{
    [Header("Character Causing Damage")]
    public CharacterManager characterCausingDamage;

    [Header("Received Attack Type")]
    public AttackType attackType;

    [Header("Damage")]
    public float physicalDamage = 0;
    public float magicDamage = 0;
    public float fireDamage = 0;
    public float lighteningDamage = 0;
    public float holyDamage = 0f;

    [Header("Final Damage")]
    public int finalDamageDealt = 0;

    [Header("Groggy")]
    public float groggyDamage = 0;
    public bool poiseIsBroken = false;

    [Header("Direction Damage Taken From")]
    public float angleHitFrom;
    public Vector3 contactPoint;

    [Header("Animation")]
    public float baseStaminaDamage = 0;
    public float finalStaminaDamage = 0;


    [Header("Animation")]
    public bool playDamageAnimation = true;
    public bool manuallySelectDamageAnimation = false;
    public string damageAnimation;

    [Header("VFX")]
    public ObjectPoolType vfxObjectPoolType;
    public int poolIndex;

    [Header("Sound FX")]
    public bool willPlayDamageSFX = true;
    public AudioClip elementalDamageSoundFX;

    protected virtual void PlayDamageVFX(CharacterManager character)
    {
        // GameObject pooledVFX = characterCausingDamage.characterCombatManager.hitEffectPerClass;
        GameObject pooledObject = ObjectPoolManager.Singleton.GetObject(vfxObjectPoolType, poolIndex);
        // GameObject pooledObject = ObjectPoolManager.Singleton.GetObject(pooledVFX);
        pooledObject.transform.position = contactPoint;
        ParticleSystem effect = pooledObject.GetComponent<ParticleSystem>();
        effect.Play();
        ObjectPoolManager.Singleton.WaitAndReturnVFX(pooledObject, ObjectPoolManager.Singleton.GetPrefab(vfxObjectPoolType, poolIndex)).Forget();
    }
    protected virtual void PlayDamageSFX(CharacterManager character)
    {
        if (characterCausingDamage == null) return;

        characterCausingDamage.TryGetComponent<EnemyAudioManager>(out EnemyAudioManager enemy_emitter);
        if (enemy_emitter != null)
        {
            enemy_emitter.SFXHeavyHit(character.transform);
            character.GetComponent<PlayerEmitter>().SFXVoiceHit();
        }

        //characterCausingDamage.TryGetComponent<AudioEmiterManager>(out AudioEmiterManager emitter);
        //if (emitter != null) emitter.SFXHit(character.transform);
        

        //AudioClip physicalDamageSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.physicalDamageSFX);
        //character.characterSoundFXManager.PlaySoundFX(physicalDamageSFX);
    }
}
