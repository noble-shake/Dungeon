using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantEffect : ScriptableObject
{
    [Header("Effect ID")]
    public int instantEffectID;

    public virtual void ProcessEffect(CharacterManager character)
    {

    }
    public virtual void ProcessEffectOnLocal(CharacterManager character)
    {

    }
    public virtual void ProcessEffectOnServer(CharacterManager character)
    {

    }
}
