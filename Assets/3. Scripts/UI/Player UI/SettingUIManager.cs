using Unity.Netcode;
using UnityEngine;

public class SettingUIManager : MonoBehaviour
{
    // 다시 인풋 살려야 함. 
    public void ContinueButton()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        InputManager.instance.active = true;
        gameObject.SetActive(false);
    }


    public void ExitButton()
    {
        if (GameManager.Instance.IsHost)
        {
            NetworkManager.Singleton.Shutdown();
            SceneLoadManager.Instance.LoadRegularScene("Scene_Booting");
        }
        else
        {
            NetworkManager.Singleton.Shutdown();
        }
    }
}
