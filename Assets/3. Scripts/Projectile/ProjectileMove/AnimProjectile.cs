using UnityEngine;
using static UnityEngine.ParticleSystem;

public class AnimProjectile : IProjectile
{
    private ProjectileObject owner;
    private Transform trs;
    ParticleSystem particle;

    public AnimProjectile(ProjectileObject owner)
    {

        Init(owner, owner.transform);
        
    }

    public void Init(ProjectileObject proj, Transform ownTrs)
    {
        owner = proj;
        trs = ownTrs;

        particle = owner.VFX.GetComponent<ParticleSystem>();
        var main = particle.main;
        main.loop = false;

        //if(owner.GetComponent<CapsuleCollider>() != null) owner.GetComponent<CapsuleCollider>().center = Vector3.zero;
        //if(owner.GetComponent<BoxCollider>() != null) owner.GetComponent<BoxCollider>().center = Vector3.zero;
        //if(owner.GetComponent<SphereCollider>() != null) owner.GetComponent<SphereCollider>().center = Vector3.zero;
    }

    public void Move()
    {
        if (particle.isPlaying == false)
        {
            owner.Remove();
        }

        //if (owner.GetComponent<CapsuleCollider>() != null) owner.GetComponent<CapsuleCollider>().center += new Vector3(0, 0, 1f) * Time.fixedDeltaTime; ;
        //if (owner.GetComponent<BoxCollider>() != null) owner.GetComponent<BoxCollider>().center += new Vector3(0, 0, 1f) * Time.fixedDeltaTime; ;
        //if (owner.GetComponent<SphereCollider>() != null) owner.GetComponent<SphereCollider>().center += new Vector3(0, 0, 1f) * Time.fixedDeltaTime; ;
        // owner.GetComponent<CapsuleCollider>().center += new Vector3(0, 0, 1f) * Time.fixedDeltaTime;

        owner.transform.position = owner.GetComponent<ProjectileCollider>().characterAttacking.transform.position;
    }
}