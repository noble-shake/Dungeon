using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AICharacterNetworkManager : CharacterNetworkManager
{
    AICharacterManager aiCharacter;

    protected override void Awake()
    {
        base.Awake();
        
        aiCharacter = GetComponent<AICharacterManager>();
    }
    public virtual void OnStanceBroken(bool previousStatus, bool newStatus)
    {
        aiCharacter.aiCharacterCombatManager.isGroggy = newStatus;
    }
}
