using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    private ParticleSystem particleGameobject;
    private List<ParticleSystem> particleList;

    private void Awake()
    {
        // AI의 경우 직접 할당하기 때문에 Awake에서 처리 가능. 
        particleGameobject = GetComponent<ParticleSystem>();
        particleList = particleGameobject.GetComponentsInChildren<ParticleSystem>().ToList();
    }

    private void Start()
    {
        foreach (var particle in particleList)
        {
            particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    // 
    public void Initialize(GameObject particleObject)
    {
        // AI의 경우 직접 할당해주기 때문에 쓸 일이 없음.
        if (particleGameobject != null)
            return;

        particleGameobject = particleObject.GetComponent<ParticleSystem>();
        particleList = particleGameobject.GetComponentsInChildren<ParticleSystem>().ToList();


    }
    public void StopParticle()
    {
        foreach (var particle in particleList)
        {
            particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    public void PlayParticle()
    {
        foreach (var particle in particleList)
        {
            particle.Play();
        }
    }
}
