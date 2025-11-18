using UnityEngine;

public class WorldVisualFXManager : MonoBehaviour
{
    public static WorldVisualFXManager instance;

    // VFX를 분류한다면? 플레이어가 쓰는거, 보스가 쓰는거.
    // 
    public GameObject[] projectile;
    public GameObject[] explosion;
    public GameObject[] playerAttackEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
