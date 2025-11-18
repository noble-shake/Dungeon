using UnityEngine;

public enum ProjectileType
{ 
    RayMove,
    AnimMove,
}


public interface IProjectile
{
    public void Init(ProjectileObject proj, Transform ownTrs);

    public void Move();
}