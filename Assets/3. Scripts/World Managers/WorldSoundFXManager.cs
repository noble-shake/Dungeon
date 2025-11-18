using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSoundFXManager : MonoBehaviour
{
    public static WorldSoundFXManager instance;

    [Header("Boss Track")]
    [SerializeField] AudioSource bossIntroPlayer;
    [SerializeField] AudioSource bossLoopPlayer;

    [Header("Damage Sounds")]
    public AudioClip[] physicalDamageSFX;

    [Header("Action Sounds")]
    public AudioClip rollSFX;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void PlayBossTrack(AudioClip introTrack, AudioClip loopTrack)
    {
        bossIntroPlayer.volume = 1;
        bossIntroPlayer.clip = introTrack;
        bossIntroPlayer.loop = false;
        bossIntroPlayer.Play();

        bossLoopPlayer.volume = 1;
        bossLoopPlayer.clip = loopTrack;
        bossLoopPlayer.loop = true;
        bossLoopPlayer.PlayDelayed(bossIntroPlayer.clip.length);
    }

    public AudioClip ChooseRandomSFXFromArray(AudioClip[] array)
    {
        if (array.Length == 0) return null;
        int index = Random.Range(0, array.Length);

        return array[index];
    }

    public void StopBossMusic()
    {
        // StartCoroutine(FadeOutBossMusicThenStop());
    }

    private IEnumerator FadeOutBossMusicThenStop()
    {
        bossIntroPlayer.Stop();

        while (bossLoopPlayer.volume > 0)
        {
            bossIntroPlayer.volume -= Time.deltaTime;
            bossLoopPlayer.volume -= Time.deltaTime;
            yield return null;
        }

        bossIntroPlayer.Stop();
        bossLoopPlayer.Stop();
    }

    // public AudioClip ChooseRandomFootStepSoundBasedOnGround(GameObject steppedOnObject, CharacterManager character)
    // {
    // switch (steppedOnObject.tag)
    // {
    //     case "Dirt":
    //         return ChooseRandomSFXFromArray(character.characterSoundFXManager.footStepsDirt);
    //     case "Stone":
    //         return ChooseRandomSFXFromArray(character.characterSoundFXManager.footStepsDirt);
    //     default:
    //         return null;
    // }
    // }
}
