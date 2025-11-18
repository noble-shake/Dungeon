using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionManager : MonoBehaviour, IInteractable
{
    PlayerManager player;

    [SerializeField] private List<IInteractable> currentInteractableActions;
    [SerializeField] private float interactionTimeOfMine;
    [SerializeField] private float interactionTimeOfOther;
    [SerializeField] private float elapsedTime;

    [SerializeField] private bool isInteracting = false;
    [SerializeField] private InteractionType interactionType = InteractionType.Player;
    [SerializeField] private string animationWhenBeingInteracted = "Pray";
    [SerializeField] private string animationToInteract = "";
    [SerializeField] private string interactionMessage = "E 키를 눌러 부활";
    public bool IsInteracting { get => isInteracting; set => isInteracting = value; }

    private void Awake()
    {
        player = GetComponentInParent<PlayerManager>();
    }

    private void Start()
    {
        currentInteractableActions = new List<IInteractable>();
    }

    private void FixedUpdate()
    {
        if (!player.IsOwner)
            return;

        // 인터랙션 체크 작업을 잠시 쉬어줄 때.
        // if (!PlayerUIManager.instance.menuWindowIsOpen && !PlayerUIManager.instance.popUpWindowIsOpen)
        CheckForInteractable();
    }

    private void CheckForInteractable()
    {
        if (currentInteractableActions.Count == 0)
        {
            interactionTimeOfOther = 0;
            animationToInteract = string.Empty;
            HUD_UIManager.instance.popUpUIManager.CloseAllPopUpWindows();
            return;
        }
        if (currentInteractableActions[0] == null)
        {
            currentInteractableActions.RemoveAt(0);
            return;
        }

        if (currentInteractableActions[0] != null)
        {
            HUD_UIManager.instance.popUpUIManager.SendPlayerMessagePopUp(currentInteractableActions[0].GetInteractionMessage());
            interactionTimeOfOther = currentInteractableActions[0].GetInteractionTime();
            animationToInteract = currentInteractableActions[0].GetInteractionAnimation();

        }

    }

    private void RefreshInteractionList()
    {
        for (int i = currentInteractableActions.Count - 1; i > -1; i--)
        {
            if (currentInteractableActions[i] == null)
                currentInteractableActions.RemoveAt(i);
        }
    }

    private void Update()
    {
        if (isInteracting)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > interactionTimeOfOther)
            {
                InteractWithOther();
            }
        }
        else
        {
            elapsedTime = 0;
        }
    }

    public void PlayInteractionAnimation()
    {
        player.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Etc, animationToInteract, true);
    }

    public void InteractWithOther()
    {
        if (currentInteractableActions.Count == 0) return;

        if (currentInteractableActions[0] != null)
        {
            InteractionType interactionType = currentInteractableActions[0].GetInteractionType();
            // switch (interactionType)
            // {
            //     case InteractionType.Player:
            //         // player.playerAnimatorManager.PlayAnimationOnNetwork(AnimationType.Etc, animationToInteract, true);
            //         break;
            // }
            currentInteractableActions[0].Interact();
            interactionTimeOfOther = 0;
            animationToInteract = string.Empty;
            RemoveInteractionToList(currentInteractableActions[0]);
            HUD_UIManager.instance.popUpUIManager.CloseAllPopUpWindows();
        }

        // }
        // 해야될 일은 단순하게 말하자면, 기도하는 애니메이션을 네트워크 상에서 실행한다.
        // 내가 E 키를 떼면, 기도를 중단해야 합니다. 
    }

    public InteractionType GetInteractionType()
    {
        return interactionType;
    }

    public void Interact()
    {
        // 플레이어의 경우 상호작용의 형태가 부활임. 
        if (!player.isDead.Value) return;
        // 해당 클래스 내에서 RPC를 만들어서 호출하지 않고 PlayerManager에 RPC를 생성해서 호출해주는 이유가 있음.
        // 해당 클래스는 플레이어 오브젝트 자식객체에 붙어있기 때문에 NetworkObject가 될 수 없고 NetworkBehaviour를 상속 받을 수 없음.
        // 고로 RPC 또한 만들어 쓸 수 없음.
        player.ReviveServerRpc();
    }

    public void AttempToInteract()
    {
        if (player.isPerformingAction) return;
        if (!player.playerLocomotionManager.isGrounded) return;
        // 상호작용 중에 맞으면 어떻게 되지?
        // E 키를 떼주면 정상적으로 취소되긴 하는데... 
        if (CanInteract())
        {
            isInteracting = true;
            player.playerNetworkManager.isInteracting.Value = true;
        }

    }

    public void CancelInteracting()
    {
        isInteracting = false;
        player.playerNetworkManager.isInteracting.Value = false;
    }

    public void AddInteractionToList(IInteractable interactable)
    {
        RefreshInteractionList();
        if (!currentInteractableActions.Contains(interactable))
            currentInteractableActions.Add(interactable);
    }

    public void RemoveInteractionToList(IInteractable interactable)
    {
        if (currentInteractableActions.Contains(interactable))
            currentInteractableActions.Remove(interactable);
        RefreshInteractionList();
    }

    // 내가, 살아있는 상태로, 다른 상호작용 가능한 객체에 갔을 때 발동.
    private void OnTriggerEnter(Collider other)
    {
        // 죽어있을 때 상호작용 리스트도 지워줘야 할 듯? 

        // 플레이어가 죽어있는 상태라면 다른 놈들과 상호작용하지 말 것. 
        if (player.isDead.Value) return;
        IInteractable interactable = other.GetComponent<IInteractable>();
        switch (interactable?.GetInteractionType())
        {
            case InteractionType.Player:
                if (!other.GetComponentInParent<CharacterManager>().isDead.Value)
                {
                    return;
                }
                break;
        }
        AddInteractionToList(interactable);

    }

    private void OnTriggerExit(Collider other)
    {

        IInteractable interactable = other.GetComponent<IInteractable>();
        RemoveInteractionToList(interactable);
    }

    public string GetInteractionAnimation()
    {
        return animationWhenBeingInteracted;
    }

    public string GetInteractionMessage()
    {
        return interactionMessage;
    }

    public float GetInteractionTime()
    {
        return interactionTimeOfMine;
    }

    public bool CanInteract()
    {
        if (currentInteractableActions.Count == 0) return false;
        if (currentInteractableActions[0] != null)
        {
            return true;
        }
        return false;
    }
}
