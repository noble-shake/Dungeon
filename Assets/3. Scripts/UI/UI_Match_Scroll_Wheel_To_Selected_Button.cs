using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Match_Scroll_Wheel_To_Selected_Button : MonoBehaviour
{
    [SerializeField] GameObject currentSelected;
    [SerializeField] GameObject previousSelected;
    [SerializeField] RectTransform currentSelectedTransform;

    [SerializeField] RectTransform contentPanel;
    [SerializeField] ScrollRect schrollRect;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        currentSelected = EventSystem.current.currentSelectedGameObject;

        if (currentSelected != null)
        {
            if (currentSelectedTransform != previousSelected)
            {
                previousSelected = currentSelected;
                currentSelectedTransform = currentSelected.GetComponent<RectTransform>();
                SnapTo(currentSelectedTransform);
            }
        }
    }

    private void SnapTo(RectTransform target)
    {
        Canvas.ForceUpdateCanvases();

        Vector2 newPosition = (Vector2)schrollRect.transform.InverseTransformPoint(contentPanel.position) - (Vector2)(schrollRect.transform.InverseTransformPoint(target.position));

        newPosition.x = 0;

        contentPanel.anchoredPosition = newPosition;
    }
}
