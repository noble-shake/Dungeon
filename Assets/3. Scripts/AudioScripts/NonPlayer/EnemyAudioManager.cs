using FMOD.Studio;
using FMODUnity;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class EnemyAudioManager : AudioEmiterManager
{
    [SerializeField, HideInInspector] StudioEventEmitter Emitter;
    string AttackParamName = "Boss_Combo";

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        Emitter = GetComponent<StudioEventEmitter>();
    }

    public void SFXAttack01(int Param = 1)
    {

        try
        {
            EventInstance eInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)WarriorGolemSFX.Attack01]);
            eInstance.setParameterByName(AttackParamName, Param);

            eInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

            eInstance.start();

            AddEvents(eInstance);
        }
        catch (FMODUnity.EventNotFoundException e)
        {
            Debug.LogError("Event File Not Found in FMOD.");
        }
    }
    public void SFXAttack02(int Param = 1)
    {
        try {
            EventInstance eInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)WarriorGolemSFX.Attack02]);
            eInstance.setParameterByName(AttackParamName, Param);

            eInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

            eInstance.start();

            AddEvents(eInstance);
        }
        catch (FMODUnity.EventNotFoundException e)
        {
            Debug.LogError("Event File Not Found in FMOD.");
        }

    }
    public void SFXAttack03(int Param = 1)
    {
        try 
        {
            EventInstance eInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)WarriorGolemSFX.Attack03]);
            eInstance.setParameterByName(AttackParamName, Param);

            eInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

            eInstance.start();

            AddEvents(eInstance);
        }
        catch (FMODUnity.EventNotFoundException e)
        {
            Debug.LogError("Event File Not Found in FMOD.");
        }

    }
    public void SFXAttack04(int Param = 1)
    {
        try 
        {
            EventInstance eInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)WarriorGolemSFX.Attack04]);
            eInstance.setParameterByName(AttackParamName, Param);

            eInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

            eInstance.start();

            AddEvents(eInstance);
        }
        catch (FMODUnity.EventNotFoundException e)
        {
            Debug.LogError("Event File Not Found in FMOD.");
        }

    }
    public void SFXAttack05(int Param = 1)
    {
        try
        {
            EventInstance eInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)WarriorGolemSFX.Attack05]);
            eInstance.setParameterByName(AttackParamName, Param);

            eInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

            eInstance.start();

            AddEvents(eInstance);
        }
        catch (FMODUnity.EventNotFoundException e)
        {
            Debug.LogError("Event File Not Found in FMOD.");
        }

    }

    public override void SFXHit(Transform trs)
    {
        if (Emitter == null) return;
        try
        {
            EventInstance eInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)WarriorGolemSFX.Hit]);

            eInstance.set3DAttributes(RuntimeUtils.To3DAttributes(trs));


            eInstance.start();

            eInstance.release();
        }
        catch (FMODUnity.EventNotFoundException e)
        {
            Debug.LogError("Event File Not Found in FMOD.");
        }
    }

    public override void SFXHeavyHit(Transform trs)
    {
        if (Emitter == null) return;

        try
        {
            EventInstance eInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)WarriorGolemSFX.HeavyHit]);

            eInstance.set3DAttributes(RuntimeUtils.To3DAttributes(trs));


            eInstance.start();

            eInstance.release();
        }
        catch (FMODUnity.EventNotFoundException e)
        {
            Debug.LogError("Event File Not Found in FMOD.");
        }
    }
}
