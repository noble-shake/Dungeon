using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TitleScreenLoadMenuInputManager : MonoBehaviour
{
    PlayerControls playerControls;

    [Header("Title Screen Inputs")]
    [SerializeField] bool deleteCharacterSlot = false;

    private void Update()
    {
        if (deleteCharacterSlot)
        {
            deleteCharacterSlot = false;
            TitleScreenManager.instance.AttempToDeleteCharacterSlot();
        }
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.UI.Next.performed += context =>
            {
                Debug.Log("스페이스 바가 입력 됨.");
                deleteCharacterSlot = true;
            };

        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
}
