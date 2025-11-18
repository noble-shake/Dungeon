using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonAudioManager : AudioEmiterManager
{
    [SerializeField, HideInInspector] StudioEventEmitter Emitter;
    EventInstance eInstance;
    public int PlayerMaxHP;
    public int BossMaxHP;
    bool EventOffCheck;

    protected override void Awake()
    {
        BossMaxHP = 5000;
        PlayerMaxHP = 400; //temporary
        base.Awake();
    }

    protected override void Start()
    {
        Emitter = GetComponent<StudioEventEmitter>();
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        OnDungeonSoundPlaying();
    }

    public void OnSceneUnloaded(Scene scene)
    {
        if(scene.buildIndex != 5) OffDungeonSoundPlaying();
    }

    public void OnDungeonSoundPlaying()
    {
        // Loop Sound
        if (Emitter == null) return;

        try
        {
            //Emitter.EventReference = EventRefDictionary[(int)DungeonSFX.Background];
            //Emitter.EventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
            //Emitter.Play();

            //eInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)DungeonSFX.Background]);

            //eInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
            //eInstance.start();
        }
        catch (FMODUnity.EventNotFoundException e) 
        {
            EventOffCheck = true;
        }

        if (EventOffCheck) return;
        StartCoroutine(delayControl());
    }

    public void OffDungeonSoundPlaying()
    {
        if (EventOffCheck) return;
        // Loop Sound
        if (Emitter == null) return;

        Emitter.Stop();
    }

    IEnumerator delayControl()
    {
        yield return new WaitForSeconds(0.2f);
        Emitter.EventInstance.setParameterByName("Boss_Health", 100);
        Emitter.EventInstance.setParameterByNameWithLabel("GimmickStates", Dungeon01BackgroundSFX.GM_Clearandhold.ToString());
        Emitter.EventInstance.setParameterByName("Player_Health", 100);

        yield return null;
    }

    public void SFXAdjustBossHP(int old, int value)
    {
        if (EventOffCheck) return;
        // EventInstance eInstance = RuntimeManager.CreateInstance();
        Emitter.EventInstance.setParameterByName("Boss_Health", ((float)value / (float)BossMaxHP) * 100);
    }

    public enum Dungeon01BackgroundSFX
    {
        GM_Clearandhold = 0,
        Gimmick1 = 1,
        Gimmick2 = 2,
        Gimmick3 = 3,
        LastGimmick = 4,
    }

    public void Dungeon01SFXAdjustGimmikState(Dungeon01BackgroundSFX _SFX = Dungeon01BackgroundSFX.GM_Clearandhold)
    {
        if (EventOffCheck) return;
        Emitter.EventInstance.setParameterByNameWithLabel("GimmickStates", _SFX.ToString());
    }

    public void SFXAdjustPlayerHP(int old, int value)
    {
        if (EventOffCheck) return;
        Debug.Log($"Health Value Changed {value} / {PlayerMaxHP} ");
        Emitter.EventInstance.setParameterByName("Player_Health", ((float)value / (float)PlayerMaxHP) * 100);
    }
}
