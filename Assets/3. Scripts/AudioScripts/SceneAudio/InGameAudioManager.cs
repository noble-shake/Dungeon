using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class InGameAudioManager : AudioEmiterManager
{
    public static InGameAudioManager Instance;
    [SerializeField, HideInInspector] StudioEventEmitter Emitter;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
