using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class PlayerEmitter : AudioEmiterManager
{
    StudioListener listener;
    [SerializeField] StudioEventEmitter emitter;
    [HideInInspector] private PlayerManager playerManager;
    [HideInInspector] private bool ownCheck;
    
    protected void Awake()
    {
        base.Awake();

        
    }

    protected void Start()
    {
        base.Start();

        TryGetComponent<StudioEventEmitter>(out emitter);
        if (InGameAudioManager.Instance != null)
        {
            // listener = InGameAudioManager.Instance.GetComponent<StudioListener>();
            listener = Camera.main.GetComponent<StudioListener>();
            listener.AttenuationObject = this.gameObject;
        }

        playerManager = GetComponentInParent<PlayerManager>();
        if (playerManager.IsOwner) ownCheck = true;
    }

    protected void Update()
    {
        base.Update();
    }

    public void SFXNormalAttack(int Param)
    {
        if (emitter == null) return;
        try
        {
            Param = Mathf.Clamp(Param, 1, 4);

            EventInstance eInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)PlayerSFX.NormalAttack]);
            eInstance.setParameterByName("Player_Combo", Param);

            eInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

            eInstance.start();

            AddEvents(eInstance);

            if (!ownCheck) return;

            EventInstance voiceInstance;
            switch (Param)
            {
                case 1:
                    voiceInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)PlayerSFX.VNormalAttack01]);

                    voiceInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

                    voiceInstance.start();

                    AddEvents(voiceInstance);
                    break;
                case 2:
                    voiceInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)PlayerSFX.VNormalAttack02]);

                    voiceInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

                    voiceInstance.start();

                    AddEvents(voiceInstance);
                    break;
                case 3:
                    voiceInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)PlayerSFX.VNormalAttack03]);

                    voiceInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

                    voiceInstance.start();

                    AddEvents(voiceInstance);
                    break;
                case 4:
                    voiceInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)PlayerSFX.VNormalAttack04]);

                    voiceInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

                    voiceInstance.start();

                    AddEvents(voiceInstance);
                    break;
            }



        }
        catch (FMODUnity.EventNotFoundException e)
        {
            Debug.LogError("Event File Not Found in FMOD.");
        }
    }

    public void SFXEvade()
    {
        if (emitter == null) return;

        try {
            EventInstance eInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)PlayerSFX.Evade]);

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

    EventInstance MoveInstance;
    public void SFXMove(float volume)
    {
        if (emitter == null) return;
        FMOD.Studio.PLAYBACK_STATE playbackState;
        MoveInstance.getPlaybackState(out playbackState);
        if (playbackState == FMOD.Studio.PLAYBACK_STATE.STOPPED)
        {
            try
            {
                MoveInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)PlayerSFX.Move]);
                MoveInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

                MoveInstance.setVolume(volume);
                MoveInstance.start();

                MoveInstance.release();
            }
            catch (FMODUnity.EventNotFoundException e)
            {
                Debug.LogError("Event File Not Found in FMOD.");
            }
        }

    }

    public void SFXDead()
    {
        if (emitter == null) return;

        try
        {
            EventInstance eInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)PlayerSFX.Dead]);

            eInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

            eInstance.start();

            AddEvents(eInstance);

        }
        catch (FMODUnity.EventNotFoundException e)
        {
            Debug.LogError("Event File Not Found in FMOD.");
        }


    }

    public void SFXSpecialMove01()
    {
        if (emitter == null) return;

        try
        {
            EventInstance eInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)PlayerSFX.SpecialAttack1]);

            eInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

            eInstance.start();

            AddEvents(eInstance);

            if (!ownCheck) return;

            EventInstance voiceInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)PlayerSFX.VSpecialAttack01]);

            voiceInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

            voiceInstance.start();

            AddEvents(voiceInstance);
        }
        catch (FMODUnity.EventNotFoundException e)
        {
            Debug.LogError("Event File Not Found in FMOD.");
        }

    }

    public void SFXSpecialMove02()
    {
        if (emitter == null) return;

        try
        {
            EventInstance eInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)PlayerSFX.SpecialAttack2]);

            eInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

            eInstance.start();

            AddEvents(eInstance);

            if (!ownCheck) return;

            EventInstance voiceInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)PlayerSFX.VSpecialAttack02]);

            voiceInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

            voiceInstance.start();

            AddEvents(voiceInstance);

        }
        catch (FMODUnity.EventNotFoundException e)
        {
            Debug.LogError("Event File Not Found in FMOD.");
        }
    }

    public override void SFXHit(Transform trs)
    {
        if (emitter == null) return;

        try
        {
            EventInstance eInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)PlayerSFX.Hit]);

            eInstance.set3DAttributes(RuntimeUtils.To3DAttributes(trs));

            eInstance.start();

            eInstance.release();
        }
        catch (FMODUnity.EventNotFoundException e)
        {
            Debug.LogError("Event File Not Found in FMOD.");
        }

    }

    public override void SFXHit(Transform trs, int Param)
    {
        if (emitter == null) return;


        try
        {
            EventInstance eInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)PlayerSFX.Hit]);
            eInstance.setParameterByName("Player_Hit_Strength", Param);

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
        if (emitter == null) return;
        try 
        { 
            EventInstance eInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)PlayerSFX.HeavyHit]);

            eInstance.set3DAttributes(RuntimeUtils.To3DAttributes(trs));

            eInstance.start();

            eInstance.release();
        }
        catch (FMODUnity.EventNotFoundException e)
        {
            Debug.LogError("Event File Not Found in FMOD.");
        }
    }

    public override void SFXVoiceHit()
    {
        if (emitter == null) return;
        try
        {
            if (!ownCheck) return;

            EventInstance eInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)PlayerSFX.VHit]);

            eInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

            eInstance.start();

            eInstance.release();
        }
        catch (FMODUnity.EventNotFoundException e)
        {
            Debug.LogError("Event File Not Found in FMOD.");
        }
    }


    public void SFXParry()
    {
        if (emitter == null) return;

        try { 
            EventInstance eInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)PlayerSFX.Parry]);

            eInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

            eInstance.start();
            
            eInstance.release();

            if (!ownCheck) return;

            EventInstance voiceInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)PlayerSFX.VParry]);

            voiceInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

            voiceInstance.start();

            voiceInstance.release();

        }
        catch (FMODUnity.EventNotFoundException e)
        {
            Debug.LogError("Event File Not Found in FMOD.");
        }
    }

    public void SFXParryCounter()
    {
        if (emitter == null) return;

        try
        {

            if (!ownCheck) return;

            EventInstance voiceInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)PlayerSFX.CounterAttack]);

            voiceInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

            voiceInstance.start();

            voiceInstance.release();

        }
        catch (FMODUnity.EventNotFoundException e)
        {
            Debug.LogError("Event File Not Found in FMOD.");
        }
    }

    public void SFXRebirth()
    {
        if (emitter == null) return;

        try
        {
            EventInstance eInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)PlayerSFX.Rebirth]);

            eInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

            eInstance.start();

            AddEvents(eInstance);
        }
        catch (FMODUnity.EventNotFoundException e)
        {
            Debug.LogError("Event File Not Found in FMOD.");
        }

    }

    public void SFXTryRebirth()
    {
        if (emitter == null) return;

        try
        {
            EventInstance eInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)PlayerSFX.OtherRebirth]);

            eInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

            eInstance.start();

            AddEvents(eInstance);
        }
        catch (FMODUnity.EventNotFoundException e)
        {
            Debug.LogError("Event File Not Found in FMOD.");
        }

    }


    // Warrior Move
    public void SFXDash()
    {

        if (emitter == null) return;
        
        try
        {
            EventInstance eInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)PlayerSFX.Dash]);

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

    public void SFXBuff()
    {
        if (emitter == null) return;
        try 
        {
            EventInstance eInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)PlayerSFX.Buff]);

            eInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

            eInstance.start();

            AddEvents(eInstance);
        }
        catch (FMODUnity.EventNotFoundException e)
        {
            Debug.LogError("Event File Not Found in FMOD.");
        }
    }

    public void SFXCutsceneVoice()
    {
        if (emitter == null) return;
        try
        {
            if (!ownCheck) return;

            EventInstance eInstance = RuntimeManager.CreateInstance(EventRefDictionary[(int)PlayerSFX.CutScene]);

            eInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

            eInstance.start();
            eInstance.release();
            //AddEvents(eInstance);
        }
        catch (FMODUnity.EventNotFoundException e)
        {
            Debug.LogError("Event File Not Found in FMOD.");
        }
    }


}