using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TextTweening : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;

    [SerializeField] private float duration = 1f;
    [SerializeField] private float startValue = 1f;
    [SerializeField] private float endValue = 0.5f;
    private void Awake()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }
    void Start()
    {
        StartAlphaTween();
    }

    // 참고로 해당 함수는 중간에 값을 바꾼다고 바꾼 값이 참조가 되지 않는다.
    private void StartAlphaTween()
    {
        // 알파값을 0.5에서 1로, 다시 1에서 0.5로 변화시키는 트윈
        textMeshProUGUI.DOFade(endValue, duration).SetLoops(-1, LoopType.Yoyo).From(startValue);
    }
}
