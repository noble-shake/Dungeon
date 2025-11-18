using UnityEngine;
using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;

public class AudioEmiterManager : MonoBehaviour
{
    [SerializeField] public SFXType ownType;
    [SerializeField] public List<AudioEventScriptable> EventRefList;
    [SerializeField] public Dictionary<int, EventReference> EventRefDictionary;
    public List<EventInstance> CurrentEventList;

    protected virtual void Awake()
    {
        EventRefDictionary = new Dictionary<int, EventReference>();
        CurrentEventList = new List<EventInstance>();
        foreach (AudioEventScriptable so in EventRefList)
        {
            EventRefDictionary[so.GetEnums(ownType)] = so.GetEventRef();
        }
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        EventCheck();

    }

    private void EventCheck()
    {
        if (CurrentEventList.Count == 0) return;

        foreach (EventInstance instance in CurrentEventList)
        {
            if (instance.isValid())
            {
                FMOD.Studio.PLAYBACK_STATE playbackState;
                instance.getPlaybackState(out playbackState);
                if (playbackState == FMOD.Studio.PLAYBACK_STATE.STOPPED)
                {
                    instance.release();
                    instance.clearHandle();
                    CurrentEventList.Remove(instance);
                }
            }
        }
    }

    public void AddEvents(EventInstance eventInstance)
    {
        if (CurrentEventList.Contains(eventInstance)) return;
        CurrentEventList.Add(eventInstance);
    }

    public void SkipActionEvents(EventInstance priorityInstance)
    {
        if (CurrentEventList.Count == 0) return;

        foreach (EventInstance instance in CurrentEventList)
        {
            if (instance.Equals(priorityInstance)) continue;

            if (instance.isValid())
            {
                FMOD.Studio.PLAYBACK_STATE playbackState;
                instance.getPlaybackState(out playbackState);
                if (playbackState == FMOD.Studio.PLAYBACK_STATE.PLAYING)
                {
                    instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                }
            }
        }
    }

    public virtual void SFXHit(Transform trs)
    { 
    
    }

    public virtual void SFXHit(Transform trs, int Param)
    {

    }

    public virtual void SFXHeavyHit(Transform trs)
    {

    }

    public virtual void SFXVoiceHit()
    { 
        
    }

}
