using UnityEngine;

public class RayProjectile : IProjectile
{
    private ProjectileObject owner;
    private Transform trs;
    float DestroyTime;

    public RayProjectile(ProjectileObject owner)
    {
        Init(owner, owner.transform);
        
    }

    // 메인 브랜치 = 곰탕맨 + 노블 
    // 메인 -> 노블 

    public void Init(ProjectileObject proj, Transform ownTrs)
    {
        owner = proj;
        trs = ownTrs;
        DestroyTime = 3f;
    }

    public void Move()
    {
        DestroyTime -= Time.fixedDeltaTime;
        if (DestroyTime < 0f)
        {
            owner.Remove();
            DestroyTime = 3f;
        }

        trs.position += trs.forward * owner.Speed * Time.fixedDeltaTime;
    }
}