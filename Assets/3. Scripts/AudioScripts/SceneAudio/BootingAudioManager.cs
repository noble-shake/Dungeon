using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class BootingAudioManager : AudioEmiterManager
{
    [SerializeField, HideInInspector] StudioEventEmitter Emitter;
    EventInstance eInstance;
    private void Start()
    {
        Emitter = GetComponent<StudioEventEmitter>();

        // OnTitlePlaying();
    }

    //public void OnTitlePlaying()
    //{
    //    // Loop Sound
    //    if (Emitter == null) return;
    //    eInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)TitleSFX.BackgroundMusic]);

    //    eInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
    //    eInstance.start();
        
    //}

    //public void OffLobbyMusic()
    //{
    //    eInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    //}

}
