using UnityEngine;

public class WarriorGolemPattern3CircleObjectCollider : MonoBehaviour
{
    [SerializeField] WarriorGolemPattern3CircleObject manager;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerManager>() == null) return;
        manager.OnOrbTriggered(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PlayerManager>() == null) return;
        manager.OnOrbTriggered(other);
    }
}
