using UnityEngine;

public class WeaponTrailController : MonoBehaviour
{

    [SerializeField] private ParticleController rightParticle;
    [SerializeField] private ParticleController leftParticle;

    public ParticleController RightParticle { get => rightParticle; set => rightParticle = value; }
    public ParticleController LeftParticle { get => leftParticle; set => leftParticle = value; }

    public void EnableTrail()
    {
        if (RightParticle == null) return;
        RightParticle.PlayParticle();
    }

    public void DisableTrail()
    {
        if (RightParticle == null) return;
        RightParticle.StopParticle();
    }

    public void EnableOffTrail()
    {
        if (LeftParticle == null) return;
        LeftParticle.PlayParticle();
    }

    public void DisableOffTrail()
    {
        if (LeftParticle == null) return;
        LeftParticle.StopParticle();
    }
}
