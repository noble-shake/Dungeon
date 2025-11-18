using UnityEngine;

[CreateAssetMenu(fileName = "Projectile 1", menuName = "Projectiles/ProjectileObject")]
public class ProjectileStructure : ScriptableObject
{
    public ProjectileObject VFXObject;
    public ProjectileType MoveMethod;
    public int PoolCount;
    public float Speed;
    public int PenetrateCounter;
    public CharacterType[] characterTypes;
    public bool isParryable;
    public bool isReflectable;
    public int projectileIndex;

    [Header("Damage")]
    public int AttackDamage;
    public int AttackGroggyDamage;
    public AttackType RangedAttackType;

    [Header("Object Pool Data")]
    public ObjectPoolType objectPoolType;
    public int index;

    public IProjectile GetMoveMethod(ProjectileObject projectile)
    {
        switch (MoveMethod)
        {
            default:
            case ProjectileType.RayMove:
                return new RayProjectile(projectile);
            case ProjectileType.AnimMove:
                return new AnimProjectile(projectile);
        }
    }

}