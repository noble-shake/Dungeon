using UnityEngine;

public interface IInteractable
{
    public void Interact(); // 해당 객체가 상호작용 시 해야되는 일들을 다룬다.
    public string GetInteractionAnimation(); // 상호작용 시 실행시켜야 하는는 애니메이션을 반환한다.
    public string GetInteractionMessage(); // 해당 객체가 상호작용 가능할 때 팝업에 뜰 메세지를 보여준다.
    public float GetInteractionTime(); // 해당 객체를 상호작용 시키기 위해 필요한 시간
    public InteractionType GetInteractionType(); // 해당 객체의 상호작용 타입을 반환한다.
    public bool CanInteract();

}
