using UnityEngine;

public class CandleEffect : MonoBehaviour
{
    [SerializeField] GameObject Frame;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<illusionIdentifier>() != null)
        { 
            Frame.SetActive(true);
        }
    }
}
