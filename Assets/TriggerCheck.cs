using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class TriggerCheck : MonoBehaviour
{
    MagicCircleObject magicCircleObject;

    void Awake()
    {
        magicCircleObject = GetComponentInParent<MagicCircleObject>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent(out PlayerManager player);
        if (player == null)
            return;
        magicCircleObject.OnPlayerEnter(player);
    }


    void OnTriggerExit(Collider other)
    {
        other.TryGetComponent(out PlayerManager player);
        if (player == null)
            return;
        magicCircleObject.OnPlayerExit(player);
    }

    void OnTriggerStay(Collider other)
    {
        magicCircleObject.Stay(other);
    }
}
