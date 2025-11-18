using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FireGettingBigger : MonoBehaviour
{
    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 1f);
    }
}
