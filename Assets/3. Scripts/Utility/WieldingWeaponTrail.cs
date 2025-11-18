using UnityEngine;

public class WieldingWeaponTrail : MonoBehaviour
{
    private TrailRenderer trailRenderer;

    private void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    public void EnableTrailRenderer()
    {
        trailRenderer.enabled = true;
    }

    public void DisableTrailRenderer()
    {
        trailRenderer.enabled = false;
    }
    void Start()
    {

    }

    void LateUpdate()
    {

    }

}
