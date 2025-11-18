using UnityEngine;

public class KnightBuffAction : MonoBehaviour
{
    private PlayerManager player;

    void Awake()
    {
        player = GetComponent<PlayerManager>();
    }


    // RPC 고려 해야함.
    public void GenerateShield()
    {

    }
}
