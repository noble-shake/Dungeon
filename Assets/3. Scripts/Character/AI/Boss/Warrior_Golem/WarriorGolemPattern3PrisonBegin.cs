using UnityEngine;

public class WarriorGolemPattern3PrisonBegin: MonoBehaviour
{
    public Transform trs;

    private void Update()
    {
        transform.position = trs.position;
    }
}
