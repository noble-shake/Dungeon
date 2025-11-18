using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCharacterEffectManager : MonoBehaviour
{
    public static WorldCharacterEffectManager instance;

    [Header("VFX")]
    public GameObject bloodSplatterVFX;

    [Header("Damage")]
    public TakeHitEffect takeHitEffect;
    public TakeInevitableHitEffect takeInevitableHitDamageEffect;
    public TakeSealHitEffect takeSealHitEffect;
    public TakeBoss1MagicCircleEffect takeBoss1MagicCircleEffect;
    public TakeBoss1PrisonEffect takeBoss1PrisonEffect;
    public TakeEmblemHitEffect takeEmblemHitEffect;
    public TakeSwordWaveHitEffect takeSwordWaveHitEffect;

    public List<DamageEffect> instanceEffectList;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        GenerateEffectIDs();
    }

    private void GenerateEffectIDs()
    {
        for (int i = 0; i < instanceEffectList.Count; i++)
        {
            instanceEffectList[i].instantEffectID = i;
        }
    }

}
