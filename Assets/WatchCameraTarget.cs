using UnityEngine;

public class WatchCameraTarget : MonoBehaviour
{
    private Transform synchronizedTransform;
    private Vector3 velocity = Vector3.zero;
    private void Update()
    {
        if (synchronizedTransform != null)
        {
            transform.position = Vector3.SmoothDamp(transform.position, synchronizedTransform.position, ref velocity, 0.1f);
        }
    }
    public void SetTransform(PlayerManager player)
    {
        if (player != null)
        {
            synchronizedTransform = player.casualFollowTransform.transform;
        }
        else
            synchronizedTransform = null;
    }
}
