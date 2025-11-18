using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatBar : MonoBehaviour
{
    protected Slider slider;
    protected RectTransform rectTransform;
    [SerializeField] protected GameObject lockBackground;


    [Header("Bar Options")]
    [SerializeField] protected bool scaleBarLengthWithStats = true;
    protected float widthScaleMultiplier = 7;
    [SerializeField] protected bool isBoss = false;
    protected virtual void Awake()
    {
        slider = GetComponentInChildren<Slider>(true);
        rectTransform = GetComponent<RectTransform>();
    }

    protected virtual void Start()
    {

    }

    public virtual void SetStat(float newValue)
    {
        slider.value = newValue;
    }

    public virtual void SetMaxStat(int maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = maxValue;

        float newWidth;

        if (scaleBarLengthWithStats)
        {
            if (isBoss == false)
            {
                // 현재 최대체력의 7배만큼으로 길이를 늘려주되 화면 절반을 넘지 못한다.

                newWidth = Mathf.Min(maxValue * widthScaleMultiplier, Screen.width / 2f);
                rectTransform.sizeDelta = new Vector2(newWidth, rectTransform.sizeDelta.y);
            }
            else
            {
                // newWidth = Screen.width * 1 / 5f;
                // rectTransform.sizeDelta = new Vector2(newWidth, rectTransform.sizeDelta.y);
            }

            HUD_UIManager.instance.combatUIManager.RefreshHUD();
        }
    }

    public virtual void SetLockOnHpBar(bool condition)
    {
        if (condition == true)
        {
            lockBackground.SetActive(true);
        }
        else
        {
            lockBackground.SetActive(false);
        }
    }
}
