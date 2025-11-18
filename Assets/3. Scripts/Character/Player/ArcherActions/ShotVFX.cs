using UnityEngine;

public class ShotVFX : MonoBehaviour
{
    [SerializeField] public float ArrowSpeed;
    [SerializeField] private float LifeTime;

    private void Update()
    {
        transform.position += transform.forward * ArrowSpeed * Time.deltaTime;
        LifeTime -= Time.deltaTime;
        if(LifeTime < 0f) Destroy(gameObject);
    }
}