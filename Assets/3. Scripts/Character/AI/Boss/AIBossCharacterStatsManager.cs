using UnityEngine;

public class AIBossCharacterStatsManager : CharacterStatsManager
{
    [Header("Groggy Settings")]
    public float maxGroggy = 10;
    public float currentGroggy;
    [SerializeField] float groggyRegeneration = 15;
    [SerializeField] bool ignoreStanceBreak = false;

    [Header("Groggy Regeneration Timer")]
    [SerializeField] float groggyRegenerationTimer = 0;
    [SerializeField] float defaultRegenerationTime = 15;
    private float tickTimer = 0;
    public bool isGroggy = false;


}
