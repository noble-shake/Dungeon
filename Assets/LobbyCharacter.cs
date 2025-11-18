using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LobbyCharacter : NetworkBehaviour
{
    [ClientRpc]
    public void EnableCharacterClientRpc(int characterIndex)
    {
        foreach (Transform model in transform)
        {
            model.gameObject.SetActive(false);
        }
        // playerCharacter.GetComponent<NetworkAnimator>().Animator = playerCharacter.transform.GetChild(index).GetComponent<Animator>();
        transform.GetChild(characterIndex).gameObject.SetActive(true);
    }

    [ClientRpc]
    public void DisableCharacterClientRpc()
    {
        foreach (Transform model in transform)
        {
            model.gameObject.SetActive(false);
        }
    }
}
