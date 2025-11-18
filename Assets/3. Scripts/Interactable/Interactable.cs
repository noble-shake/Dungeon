using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Interactable : NetworkBehaviour
{
    public string interactableText;
    [SerializeField] protected Collider interactableCollider;
    [SerializeField] protected bool hostOnlyInteractable = false; // true일 때, 오브젝트는 다른 플레이어들에 의해 상호작용되지 않음.

    protected virtual void Awake()
    {
        // 의도적으로 오브젝트에 따라서 콜라이더를 붙이지 않을 때가 있음. 
        if (interactableCollider == null)
            interactableCollider = GetComponent<Collider>();
    }

    protected virtual void Start()
    {

    }

    public virtual void Interact(PlayerManager player)
    {
        Debug.Log("YOU HAVE INTERACTED!");

        if (!player.IsOwner)
            return;

        interactableCollider.enabled = false;

        // player.playerInteractionManager.RemoveInteractionToList(this);

        HUD_UIManager.instance.popUpUIManager.CloseAllPopUpWindows();

    }

    // Rigidbody를 넣어 줘야 자식 오브젝트의 트리거 이벤트가 발동된다.
    // Rigidbody를 넣는 순간 
    public virtual void OnTriggerEnter(Collider other)
    {
        PlayerManager player = other.GetComponent<PlayerManager>();

        if (player != null)
        {
            if (!player.playerNetworkManager.IsHost && hostOnlyInteractable)
                return;

            if (!player.IsOwner)
                return;

            // player.playerInteractionManager.AddInteractionToList(this);
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        PlayerManager player = other.GetComponent<PlayerManager>();

        if (player != null)
        {
            if (!player.playerNetworkManager.IsHost && hostOnlyInteractable)
                return;

            if (!player.IsOwner)
                return;

            // player.playerInteractionManager.RemoveInteractionToList(this);

            HUD_UIManager.instance.popUpUIManager.CloseAllPopUpWindows();
        }
    }
}
