using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEffectManager : MonoBehaviour
{
    CharacterManager character;

    [Header("VFX")]
    [SerializeField] GameObject bloodSplatterVFX;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public virtual void ProcessInstantEffct(InstantEffect effect)
    {
        effect.ProcessEffect(character);
    }
    public virtual void ProcessInstantEffectOnLocal(InstantEffect effect)
    {
        effect.ProcessEffectOnLocal(character);
    }
    public virtual void ProcessInstantEffctOnServer(InstantEffect effect)
    {
        effect.ProcessEffectOnServer(character);
    }

    public void PlayBloodSplatterVFX(Vector3 contactPoint)
    {
        if (bloodSplatterVFX != null)
        {
            GameObject bloodSplatter = Instantiate(bloodSplatterVFX, contactPoint, Quaternion.identity);
        }
        else
        {
            GameObject bloodSplatter = Instantiate(WorldCharacterEffectManager.instance.bloodSplatterVFX, contactPoint, Quaternion.identity);
        }
    }
}
