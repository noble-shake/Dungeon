using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFootStepSFXMaker : MonoBehaviour
{
    CharacterManager character;

    AudioSource audioSource;
    GameObject steppedOnObject;

    private bool hasTouchedGround = false;
    private bool hasPlayedFootStepSFX = false;
    [SerializeField] float distanceToGround = 0.05f;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        character = GetComponentInParent<CharacterManager>();
    }

    private void FixedUpdate()
    {
        CheckForFootSteps();
    }

    // 발소리를 내야 되는지 판단한다.
    private void CheckForFootSteps()
    {
        if (character == null) return;

        if (!character.characterNetworkManager.isMoving.Value) return;

        RaycastHit hit;

        // 발이 지형지물이랑 닿아있는지 체크한다.
        if (Physics.Raycast(transform.position, character.transform.TransformDirection(Vector3.down), out hit, 0.05f, WorldUtilityManager.instance.GetEnvironmentLayers()))
        {
            hasTouchedGround = true;

            if (!hasPlayedFootStepSFX)
                steppedOnObject = hit.transform.gameObject;
        }
        else
        {
            hasTouchedGround = false;
            hasPlayedFootStepSFX = false;
            steppedOnObject = null;
        }

        if (hasTouchedGround && !hasPlayedFootStepSFX)
        {
            hasPlayedFootStepSFX = true;
            PlayFootStepSoundFX();
        }
    }

    private void PlayFootStepSoundFX()
    {
        // 밟고 있는 것에 따라 다른 소리 재생.
        character.characterSoundFXManager.PlayFootStepSoundFX();
    }




}
