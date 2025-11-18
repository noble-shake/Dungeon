using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickJoinButton : MonoBehaviour
{
    [SerializeField] private GameObject quickJoinQueuePopUp;

    public void UI_QuickJoin()
    {
        // TODO : 퀵조인 팝업창을 띄워야 함.
        quickJoinQueuePopUp.SetActive(true);
        GameNetworkManager.Instance.QuickJoin(quickJoinQueuePopUp);
    }
}
