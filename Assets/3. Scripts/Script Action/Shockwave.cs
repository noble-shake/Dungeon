using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

// 1안 Physics.OverlapSphere 를 활용한 방법 
// 2안 데미지 콜라이더를 활용하는 방법.
public class Shockwave : MonoBehaviour
{
    [SerializeField] ObjectPoolType objectPoolType;
    [SerializeField] int index;

    [SerializeField] DamageCollider damageCollider;
    [SerializeField] float vfxDistance = 1f;

    // 애니메이션 이벤트로 걸어주는 함수
    // 충격파 이펙트 + 0.2초간 데미지콜라이더 활성화 
    public void MakeShockwave()
    {
        PlayShockwaveVFX();
        WaitAndDisableDamageCollider(0.2f).Forget();
    }

    public async UniTaskVoid WaitAndDisableDamageCollider(float duration)
    {
        damageCollider.EnableDamageCollider();
        await UniTask.WaitForSeconds(duration);
        damageCollider.DisableDamageCollider();
    }

    public void PlayShockwaveVFX()
    {
        GameObject pooledObject = ObjectPoolManager.Singleton.GetObject(objectPoolType, index);

        // GameObject pooledObject = ObjectPoolManager.Singleton.GetObject(shockwaveEffectPrefab);
        pooledObject.transform.position = transform.position + transform.forward * vfxDistance;
        ParticleSystem effect = pooledObject.GetComponent<ParticleSystem>();
        effect.Play();

        ObjectPoolManager.Singleton.WaitAndReturnVFX(pooledObject, ObjectPoolManager.Singleton.GetPrefab(objectPoolType, index)).Forget();
    }

    public void EnableDamageCollider()
    {
        damageCollider.EnableDamageCollider();
        // WaitAndDisableDamageCollider(0.5f).Forget();
    }

    public void DisableDamageCollider()
    {
        damageCollider.DisableDamageCollider();
    }

    public void SetShockWaveDamage(float damage, float groggyDamage)
    {
        damageCollider.physicalDamage = damage;
        damageCollider.groggyDamage = groggyDamage;
    }
}
