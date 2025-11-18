using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class LobbyAudioManager : AudioEmiterManager
{
    [SerializeField, HideInInspector] StudioEventEmitter Emitter;
    EventInstance eInstance;
    public List<EventReference> CharacterSelectSound;
    public List<EventReference> GameStartSound;
    public int CurrentSelect = -1;

    private void Start()
    {
        Emitter = GetComponent<StudioEventEmitter>();
        // SceneManager.sceneUnloaded += OffLobbyMusicBySceneUnloaded;
        Debug.Log("Start: SceneLoaded1");

        // OnLobbyMusicPlaying();
        CurrentSelect = -1;
    }

    public void SFXCharacterSelect(int Param)
    {
        if (Emitter == null) return;
        if (CurrentSelect == Param) return;
        CurrentSelect = Param;
        try
        {
            EventInstance eInstance = RuntimeManager.CreateInstance(CharacterSelectSound[Param]);
            eInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

            eInstance.start();

            AddEvents(eInstance);

            SkipActionEvents(eInstance);
        }
        catch (FMODUnity.EventNotFoundException e)
        {
            Debug.LogError("Event File Not Found in FMOD.");
        }

    }

    public void SFXStart()
    {
        if (Emitter == null) return;
        EventInstance eInstance = RuntimeManager.CreateInstance(GameStartSound[CurrentSelect]);
        try
        {
            eInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

            eInstance.start();

            AddEvents(eInstance);

            SkipActionEvents(eInstance);
        }
        catch (FMODUnity.EventNotFoundException e)
        {
            Debug.LogError("Event File Not Found in FMOD.");
        }
    }

}
