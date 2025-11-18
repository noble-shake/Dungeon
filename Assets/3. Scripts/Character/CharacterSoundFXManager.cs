using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSoundFXManager : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Damage Grunts")]
    [SerializeField] protected AudioClip[] damageGrunts;

    [Header("Attack Grunts")]
    [SerializeField] protected AudioClip[] attackGrunts;

    [Header("FootSteps")]
    [SerializeField] protected AudioClip[] footSteps;


    // 방법 2
    // public AudioClip[] footSteps;
    // public AudioClip[] footStepsDirt;

    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySoundFX(AudioClip soundFX, float volume = 1, bool randomizePitch = true, float pitchRandom = 0.1f)
    {

        //audioSource.PlayOneShot(soundFX, volume); // 1이 최대임;
        //audioSource.clip = soundFX;
        //audioSource.Play();
        // audioSource.pitch = 1;

        // if (randomizePitch)
        // {
        //     audioSource.pitch += Random.Range(-pitchRandom, pitchRandom);
        // }
    }

    public void PlayRollSoundFX()
    {

    }

    public virtual void PlayDamageGrunt()
    {
        if (damageGrunts.Length > 0)
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(damageGrunts));
    }

    public virtual void PlayAttackGrunt()
    {
        if (attackGrunts != null)
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(attackGrunts));
    }

    public virtual void PlayFootStepSoundFX()
    {
        if (footSteps.Length > 0)
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(footSteps));
    }

    public virtual void PlayStanceBreakSoundFX()
    {
        // audioSource.PlayOneShot(WorldSoundFXManager.instance.stanceBreakSFX);
    }

}
