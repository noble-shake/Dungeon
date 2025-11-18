using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectManager : CharacterEffectManager
{
    [Header("Debug Delete Later")]
    [SerializeField] InstantEffect effectToTest;
    [SerializeField] bool processEffect = false;

    // Update is called once per frame
    void Update()
    {
        if (processEffect)
        {
            processEffect = false;
            InstantEffect effect = Instantiate(effectToTest);
            ProcessInstantEffct(effect);
        }
    }

    
}
