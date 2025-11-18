using UnityEngine;
using UnityEngine.UI;

public class AudioControlManager : MonoBehaviour
{
    public static AudioControlManager Instance;
    public FMOD.Studio.VCA VCAMaster;
    public FMOD.Studio.VCA VCABGM;
    public FMOD.Studio.VCA VCASFX;
    public FMOD.Studio.VCA VCAVoice;

    public Slider MasterSlider;
    public Slider BGMSlider;
    public Slider SFXSlider;
    public Slider VoiceSlider;

    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        VCAMaster = FMODUnity.RuntimeManager.GetVCA("vca:/ALL");
        VCABGM = FMODUnity.RuntimeManager.GetVCA("vca:/BGM");
        VCASFX = FMODUnity.RuntimeManager.GetVCA("vca:/SFX");
        VCAVoice = FMODUnity.RuntimeManager.GetVCA("vca:/DIA");

        MasterSlider.onValueChanged.AddListener(SetMasterVolume);
        BGMSlider.onValueChanged.AddListener(SetBGMVolume);
        SFXSlider.onValueChanged.AddListener(SetSFXVolume);
        VoiceSlider.onValueChanged.AddListener(SetVoiceVolume);

        gameObject.SetActive(false);
    }

    public void SetMasterVolume(float volume)
    {
        VCAMaster.setVolume(volume);
    }

    public void SetBGMVolume(float volume)
    {
        VCABGM.setVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        VCASFX.setVolume(volume);
    }

    public void SetVoiceVolume(float volume)
    {
        VCAVoice.setVolume(volume);
    }

}