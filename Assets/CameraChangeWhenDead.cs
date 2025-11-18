using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CameraChangeWhenDead : NetworkBehaviour
{
    public int currentWatchingIndex = 0;
    public int myIndex = 0;
    public Transform watchingCameraTransform;

    List<PlayerManager> list;
    private bool initialized = false;
    public void CameraChange()
    {
        // if (initialized == false)
        // {
        //     initialized = true;
        //     Initialize();
        // }
        currentWatchingIndex++;
        if (currentWatchingIndex == list.Count)
            currentWatchingIndex = 0;

        // 어떤 플레이어를 추적할지 정한다.
        CameraController.instance.ChangeCameraFollow(list[currentWatchingIndex]);

    }

    public void Initialize()
    {
        list = FindObjectsByType<PlayerManager>(FindObjectsSortMode.None).ToList();

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].OwnerClientId == OwnerClientId)
            {
                myIndex = i;
                currentWatchingIndex = myIndex;
                break;
            }
        }
    }
}
