using UnityEngine;

public class WArriorGolemPattern3MagicCircleExplosion : MonoBehaviour
{
    public WarriorGolemPattern3MagicCircle _owner;

    private void OnTriggerEnter(Collider other)
    {
        _owner.ExplosionTrigger(other);
    }

    private void OnTriggerStay(Collider other)
    {
        _owner.ExplosionTrigger(other);
    }

    private void OnTriggerExit(Collider other)
    {
        _owner.ExplosionTriggerOut(other);
    }
}