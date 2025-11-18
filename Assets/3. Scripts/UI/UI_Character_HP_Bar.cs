using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

// UI Stat Bar와 동일하지만, 월드 스페이스에서 사라질 때도 있다는 점이 다름.
public class UI_Character_HP_Bar : UI_StatBar
{
    private CharacterManager character;
    private AICharacterManager aiCharacter;
    private PlayerManager playerCharacter;

    [SerializeField] bool displayCharacterNameOnDamage = false;
    [SerializeField] float defaultTimeBeforeBarDisappear = 3;
    [SerializeField] float hideTimer = 0;
    [SerializeField] int currentDamageTaken = 0;
    [SerializeField] TextMeshProUGUI characterName;
    [SerializeField] TextMeshProUGUI characterDamage;
    [HideInInspector] public int oldHealthValue = 0;

    protected override void Awake()
    {
        base.Awake();

        character = GetComponentInParent<CharacterManager>();
        if (character != null)
        {
            aiCharacter = character as AICharacterManager;
            playerCharacter = character as PlayerManager;
        }
    }

    private void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.forward);

        if (hideTimer > 0)
        {
            hideTimer -= Time.deltaTime;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    protected override void Start()
    {
        base.Start();

        gameObject.SetActive(false);
    }

    public override void SetStat(float newHealthValue)
    {
        currentDamageTaken = 0;
        // 데미지를 받았을 시 캐릭터의 이름을 표시한다.
        if (displayCharacterNameOnDamage)
        {
            characterName.enabled = true;
            if (aiCharacter != null)
                characterName.text = aiCharacter.characterName;

            if (playerCharacter != null)
                characterName.text = playerCharacter.playerNetworkManager.characterName.Value.ToString();
        }

        slider.maxValue = character.characterNetworkManager.maxHp.Value;

        currentDamageTaken = Mathf.RoundToInt(currentDamageTaken + (oldHealthValue - newHealthValue));

        // 데미지를 받은 게 아니라 체력 회복을 했을 경우
        if (currentDamageTaken < 0)
        {
            currentDamageTaken = Mathf.Abs(currentDamageTaken);
            characterDamage.text = "+ " + currentDamageTaken.ToString();
        }
        // 데미지를 받은 경우
        else
        {
            characterDamage.text = "- " + currentDamageTaken.ToString();
        }

        slider.value = newHealthValue;
        if (character.characterNetworkManager.currentHp.Value != character.characterNetworkManager.maxHp.Value)
        {
            hideTimer = defaultTimeBeforeBarDisappear;
            gameObject.SetActive(true);
        }


    }
}
