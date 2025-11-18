using UnityEngine;


public class BlinkEffect : MonoBehaviour
{
    public Material material;
    public Color blinkColor = Color.white;
    public float blinkDuration = 0.1f;

    private Color originalColor;

    public bool blinkButton = false;

    void Start()
    {
        // if (material.HasProperty("_EmissionColor"))
        // {
        //     originalColor = material.GetColor("_EmissionColor");
        // }
    }
    void Update()
    {
        if (blinkButton)
        {
            blinkButton = false;
            Blink();
        }
    }

    public void Blink()
    {
        StartCoroutine(BlinkRoutine());
    }

    private System.Collections.IEnumerator BlinkRoutine()
    {
        if (material.HasProperty("_EmissionColor"))
        {
            material.SetColor("_EmissionColor", blinkColor);
            yield return new WaitForSeconds(blinkDuration);
            material.SetColor("_EmissionColor", originalColor);
        }
    }
}
