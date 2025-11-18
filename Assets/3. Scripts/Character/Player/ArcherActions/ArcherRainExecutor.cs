using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class ArcherRainExecutor : MonoBehaviour
{
    [SerializeField] private bool isTriggered;
    [SerializeField] private float curTime;
    [SerializeField] public Collider ArrowCollider;
    [SerializeField] public List<AICharacterManager> enemies;
   
    public DamageCollider damageCollider;

    public void Init(GameObject _object)
    {
        damageCollider.characterAttacking = _object.GetComponent<PlayerManager>();
    }

    IEnumerator HitSystem()
    {
        int count = 0;
        while (count < 25)
        {
            damageCollider.EnableDamageCollider();
            yield return new WaitForSeconds(0.2f);
            damageCollider.DisableDamageCollider();
            count++;
        }
        yield return null;

        Destroy(gameObject);
    }

    private void Start()
    {
        enemies = new List<AICharacterManager>();
        StartCoroutine(HitSystem());
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<CharacterManager>() != null)
        {
            AICharacterManager target = other.GetComponentInParent<AICharacterManager>();
            if (target == null) return;
            if (enemies.Contains(target) == false && target.characterGroup == CharacterType.Monster)
            {
                enemies.Add(target);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<CharacterManager>() != null)
        {
            AICharacterManager target = other.GetComponentInParent<AICharacterManager>();
            if (target == null) return;
            if (enemies.Contains(target) == true)
            {
                enemies.Remove(target);
            }
        }
    }

}
