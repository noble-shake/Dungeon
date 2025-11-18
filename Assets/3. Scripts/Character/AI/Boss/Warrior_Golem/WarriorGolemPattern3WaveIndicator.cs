using Cysharp.Threading.Tasks;
using DTT.AreaOfEffectRegions;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class WarriorGolemPattern3WaveIndicator : MonoBehaviour
{
    public WarriorGolemPattern3Component BossComponent;
    public AIBossCharacterAnimatorManager BossAnimator;
    private List<CharacterManager> Players;
    public CircleRegion Indicator;
    public ParticleSystem Explosion;
    public GameObject Aura;

    public DamageCollider damageCollider;

    public void Init(GameObject _object)
    {
        BossComponent = _object.GetComponent<WarriorGolemPattern3Component>();
        BossAnimator = _object.GetComponent<AIBossCharacterAnimatorManager>();
        damageCollider.characterAttacking = _object.GetComponent<AIBossCharacterManager>();
    }

    public float FillProgress { get { return Indicator.FillProgress; } set { Indicator.FillProgress = value; } }

    private void Start()
    {
        Players = new List<CharacterManager>();

    }

    public void HitTargets()
    {
        WaitAndDisableDamageCollider(0.5f).Forget();
    }

    public async UniTaskVoid WaitAndDisableDamageCollider(float duration)
    {
        damageCollider.EnableDamageCollider();
        await UniTask.WaitForSeconds(duration);
        damageCollider.DisableDamageCollider();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<CharacterManager>() != null)
        {
            CharacterManager target = other.GetComponentInParent<CharacterManager>();
            if (Players.Contains(target) == false && target.characterGroup == CharacterType.Player)
            { 
                Players.Add(target);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<CharacterManager>() != null)
        {
            CharacterManager target = other.GetComponentInParent<CharacterManager>();
            if (Players.Contains(target) == true)
            { 
                Players.Remove(target);
            }
        }


    }

}
