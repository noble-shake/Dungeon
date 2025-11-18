using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FireOnAnimation : MonoBehaviour
{
    [SerializeField] GameObject[] fireFXList;

    // 왼쪽은 -90도, 오른쪽은 90도 돌아가야 함.
    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;
    public float rotationDuration = 1f;

    public void EnableFireFx(int index)
    {
        fireFXList[index].SetActive(true);
    }

    public void RotateDoor()
    {
        leftDoor.transform.DORotate(new Vector3(0, -90, 0), rotationDuration, RotateMode.LocalAxisAdd);

        // rightDoor를 1초 동안 90도 회전
        rightDoor.transform.DORotate(new Vector3(0, 90, 0), rotationDuration, RotateMode.LocalAxisAdd);
    }

    public void GoToMainScene()
    {
        // yield return SceneLoadManager.Instance.FadeOut();
        SceneLoadManager.Instance.LoadRegularScene("Scene_Booting", false);
    }
}
