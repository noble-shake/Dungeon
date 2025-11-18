using DG.Tweening;
using System;
using UnityEngine;

public class ScreenUIManager : MonoBehaviour
{
    public RectTransform top;
    public RectTransform bottom;

    public void ZoomInScreen()
    {
        Vector2 currentTopSize = top.sizeDelta;
        Vector2 currentBottomSize = bottom.sizeDelta;

        top.DOSizeDelta(new Vector2(currentTopSize.x, 160), 0.6f).SetEase(Ease.Linear);
        bottom.DOSizeDelta(new Vector2(currentBottomSize.x, 160), 0.6f).SetEase(Ease.Linear);
    }

    public void ZoomOutScreen()
    {
        Vector2 currentTopSize = top.sizeDelta;
        Vector2 currentBottomSize = bottom.sizeDelta;

        top.DOSizeDelta(new Vector2(currentTopSize.x, 0), 0.6f).SetEase(Ease.Linear);
        bottom.DOSizeDelta(new Vector2(currentBottomSize.x, 0), 0.6f).SetEase(Ease.Linear);
    }
}
