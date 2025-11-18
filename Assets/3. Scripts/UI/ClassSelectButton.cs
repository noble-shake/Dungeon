using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ClassSelectButton : MonoBehaviour
{
    [SerializeField] private PlayerClass characterClass;
    [SerializeField] private bool isButtonPressed = false;

    // 해당 캐릭터에 설명되는 UI 등을 띄워준다? -> 3D 오브젝트 알아볼 것. 
    public void OnPointerEnter()
    {

    }

    // 버튼을 누르게 되면 무슨 일이 발생하는가? 네트워크 상에 내가 이 캐릭터를 골랐음이 전파된다.
    // 내가 이 캐릭터를 고르는 것이 서버에 의해 용인되어야 한다.  
    // 전파되면 다른 사람들의 UI 상에 해당 버튼을 비활성화한다. 
    public void OnClick()
    {
        foreach (Transform otherButton in transform.parent.transform)
        {
            if (otherButton.gameObject != gameObject)
            {
                otherButton.GetComponent<ClassSelectButton>().isButtonPressed = false;
            }
        }
        isButtonPressed = !isButtonPressed;
        LobbySettingManager.instance.SelectCharacterClass(characterClass, isButtonPressed);
    }
}
