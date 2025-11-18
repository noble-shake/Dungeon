using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class TitleAudioManager : AudioEmiterManager
{
    [SerializeField, HideInInspector] StudioEventEmitter Emitter;
    [SerializeField] EventReference TitleSoundRef;

    private void Start()
    {
        Emitter = GetComponent<StudioEventEmitter>();

        OnTitlePlaying();
    }

    public void OnTitlePlaying()
    { 
        // Loop Sound
    }

    public void OnAnyPressedKey()
    {
        // EventInstance eInstance = RuntimeManager.CreateInstance();
    }
}
