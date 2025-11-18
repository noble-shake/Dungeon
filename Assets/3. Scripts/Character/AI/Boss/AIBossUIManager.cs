using UnityEngine;

public class AIBossUIManager : MonoBehaviour
{
    private AIBossCharacterManager boss;

    [Header("UI")]
    public GameObject bossStatBarOnHud;
    private void Awake()
    {
        boss = GetComponent<AIBossCharacterManager>();
    }

    public void EnableBossStatBar()
    {
        bossStatBarOnHud.SetActive(true);
    }

    public void DisableBossStatBar()
    {
        bossStatBarOnHud.SetActive(false);
    }

    public void Initialize()
    {
        
    }

}
