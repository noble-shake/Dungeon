using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIManager : MonoBehaviour
{
    [Header("Stat Bars")]
    [SerializeField] UI_StatBar healthBar;
    [SerializeField] UI_StatBar staminaBar;

    [Header("Boss")]
    [SerializeField] UI_StatBar bossHpBar;
    [SerializeField] string bossName;
    [SerializeField] GameObject classIconGroupGameobject;
    [SerializeField] Transform bossPanel;
    [Header("Companions")]
    [SerializeField] GameObject companionGroupGameobject;

    [Header("Skill Panel")]
    [SerializeField] Transform skillPanel;
    Tween rightClickTween;
    public Sprite[] skillDescImageList;
    public Image skillDescImage;

    [Header("SwordWave Pattern")]
    [SerializeField] Transform swordwavePanel;
    [SerializeField] int leftHitCount = -1;

    [Header("Emblem Pattern")]
    [SerializeField] Transform emblemPanel;
    [SerializeField] RectTransform[] iconPositionList;

    // [Header("Skills")]
    // [SerializeField] 


    // 현재 퀵슬롯은 사용되지 않고 있음.
    [Header("Quick Slots")]
    [SerializeField] Image rightWeaponQuickSlotIcon;
    [SerializeField] Image leftWeaponQuickSlotIcon;
    private Coroutine rightclickCoroutine;


    // 체력바의 value가 바뀌는 게 아니라, 체력바의 maxValue가 바뀌는 경우일 때 호출 됨.
    // 체력바 자체의 길이를 조정해줘야 되기 때문에 이렇게 씀. 
    public void RefreshHUD()
    {
        if (healthBar.gameObject.activeSelf)
        {
            healthBar.gameObject.SetActive(false);
            healthBar.gameObject.SetActive(true);
        }
        // if (staminaBar.gameObject.activeSelf)
        // {
        //     staminaBar.gameObject.SetActive(false);
        //     staminaBar.gameObject.SetActive(true);
        // }
        if (bossHpBar.gameObject.activeSelf)
        {
            bossHpBar.gameObject.SetActive(false);
            bossHpBar.gameObject.SetActive(true);
        }
    }


    // 플레이어의 체력 변화에 따라서 slider의 value를 바꿔줌.
    public void SetNewHealthValue(int previousValue, int newValue)
    {
        healthBar.SetStat(Mathf.RoundToInt(newValue));
    }

    // 체력 바 슬라이더의 maxValue를 플레이어의 최대 체력으로 변경함.
    // 이렇게 해놓고 체력 바 슬라이더의 value에 현재 체력 값을 넣어주면 알아서 사이즈에 맞게끔 슬라이더가 변경됨.
    public void SetMaxHealthValue(int maxHealth)
    {
        healthBar.SetMaxStat(maxHealth);
    }


    public void SetNewStaminaValue(float previousValue, float newValue)
    {
        staminaBar.SetStat(newValue);
    }


    public void SetMaxStaminaValue(int maxStamina)
    {
        staminaBar.SetMaxStat(maxStamina);
    }

    public void SetNewBossHpValue(int previousValue, int newValue)
    {
        Debug.Log($"보스가 입은 데미지 : {previousValue - newValue}");
        Debug.Log($"보스 남은 체력 : {newValue}");
        bossHpBar.SetStat(newValue);
    }

    public void SetMaxBossHpValue(int newValue)
    {
        bossHpBar.SetMaxStat(newValue);
        Debug.Log($"보스 최대체력 설정 {newValue}");
    }

    public void SetRightWeaponQuickSlotIcon(int weaponID)
    {
        Weapon weapon = WorldItemDatabase.instance.GetWeaponByID(weaponID);

        if (weapon == null)
        {
            rightWeaponQuickSlotIcon.enabled = false;
            rightWeaponQuickSlotIcon.sprite = null;
            return;
        }

        if (weapon.itemIcon == null)
        {
            Debug.Log("Item has no icon");
            rightWeaponQuickSlotIcon.enabled = false;
            rightWeaponQuickSlotIcon.sprite = null;
            return;
        }

        rightWeaponQuickSlotIcon.sprite = weapon.itemIcon;
        rightWeaponQuickSlotIcon.enabled = true;
    }

    public void SetLeftWeaponQuickSlotIcon(int weaponID)
    {
        Weapon weapon = WorldItemDatabase.instance.GetWeaponByID(weaponID);

        if (weapon == null)
        {
            leftWeaponQuickSlotIcon.enabled = false;
            leftWeaponQuickSlotIcon.sprite = null;
            return;
        }

        if (weapon.itemIcon == null)
        {
            Debug.Log("Item has no icon");
            leftWeaponQuickSlotIcon.enabled = false;
            leftWeaponQuickSlotIcon.sprite = null;
            return;
        }

        leftWeaponQuickSlotIcon.sprite = weapon.itemIcon;
        leftWeaponQuickSlotIcon.enabled = true;
    }

    public void EnableBossUI(AIBossCharacterManager boss)
    {
        bossHpBar.gameObject.SetActive(true);
    }

    public void DisableBossUI(AIBossCharacterManager boss)
    {
        bossHpBar.gameObject.SetActive(false);
    }




    // 맨 처음 스폰 될 때? 
    public void SetClassIcon(PlayerClass playerClass)
    {
        foreach (Transform child in classIconGroupGameobject.transform)
        {
            child.gameObject.SetActive(false);
        }

        int classIconIndex = (int)playerClass;
        classIconGroupGameobject.transform.GetChild(classIconIndex).gameObject.SetActive(true);
    }

    public void SetLockOnHpBar(bool condition)
    {
        bossHpBar.SetLockOnHpBar(condition);
    }

    // 플레이어가 연결됐을 때, 해당 플레이어의 UI를 활성화 시켜준다.
    // 비활성화된 UI는 초기화의 타겟 대상이 된다. 따로 삭제해줄 필요 없음.
    public void AddCompanion(PlayerManager playerManager)
    {
        foreach (Transform child in companionGroupGameobject.transform)
        {
            if (child.gameObject.activeSelf == false)
            {
                child.gameObject.SetActive(true);
                // 체력, 아이콘을 초기화해준다.
                child.GetComponent<CompanionInfo>().Initialize(playerManager);

                return;
            }
        }
    }

    // 플레이어가 연결이 끊어졌을 때, 해당 플레이어의 UI를 비활성화 시켜준다.
    public void RemoveCompanion(PlayerManager playerManager)
    {
        foreach (Transform child in companionGroupGameobject.transform)
        {
            if (child.GetComponent<CompanionInfo>().NetworkObjectId == playerManager.NetworkObjectId)
            {
                child.gameObject.SetActive(false);
                return;
            }
        }
    }

    public void EmblemPatternProcess(int playerClass)
    {
        // 화면 가운데에 크게 인자로 받은 엠블럼을 띄우고
        int count = 0;
        foreach (Transform emblem in emblemPanel.transform)
        {
            emblem.gameObject.SetActive(true);

            foreach (Transform icon in emblem.transform)
            {
                icon.gameObject.SetActive(false);
            }
            emblem.GetChild(playerClass).gameObject.SetActive(true);
            RectTransform rect = emblem.GetComponent<RectTransform>();

            rect.DOScale(2.5f, 0f).SetEase(Ease.Linear);
            rect.localPosition = new Vector2(0, 0);

            // iconPositionList에서 이동할 목표 위치 가져오기
            if (count < iconPositionList.Length)
            {
                RectTransform targetPositionRect = iconPositionList[count];

                // World Position 계산
                Vector3 worldPosition = targetPositionRect.position;

                // World Position을 rect의 부모 기준 Local Position으로 변환
                Vector3 localPosition = rect.parent.InverseTransformPoint(worldPosition);

                // rect를 iconPositionList의 위치로 이동
                rect.DOLocalMove(localPosition, 1f).SetEase(Ease.InOutSine).SetDelay(3f);
                rect.DOScale(1f, 1f).SetEase(Ease.Linear).SetDelay(3f);
            }
            // DOVirtual.DelayedCall(4f, ()=> 
            count++;
        }
        // 그 엠블럼이 작아지며 4개가 정해진 위치로 간다.
        // 
    }

    public bool CheckActivatedEmblem()
    {
        foreach (Transform emblem in emblemPanel.transform)
        {
            if (emblem.gameObject.activeSelf)
                return true;
        }
        return false;
    }

    public bool IfActivatedIconExists()
    {
        for (int i = 3; i >= 0; i--)
        {
            if (emblemPanel.GetChild(i).gameObject.activeSelf)
                return true;
        }
        return false;
    }

    public void RemoveIcon()
    {
        for (int i = 3; i >= 0; i--)
        {
            if (emblemPanel.GetChild(i).gameObject.activeSelf)
            {
                emblemPanel.GetChild(i).gameObject.SetActive(false);
                break;
            }
        }
    }

    public void SetSwordWaveIcon(int parriedWaveHitCountThreshold)
    {
        leftHitCount = parriedWaveHitCountThreshold;
        int count = parriedWaveHitCountThreshold / 4;
        if (parriedWaveHitCountThreshold % 4 != 0)
        {
            count++;
        }
        for (int i = 0; i < count; i++)
        {
            swordwavePanel.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void RemoveSwordWaveIcon()
    {
        leftHitCount--;
        Debug.Log($"남은 타격 횟수 : {leftHitCount}");
        // 20개 있다고 치자
        // 5개 생성
        // 인덱스는 0부터 4

        if (leftHitCount == 0)
        {
            print("내가 과연 여기서 실행되고 있을까?");
            swordwavePanel.GetChild(0).gameObject.SetActive(false);
            return;
        }

        // 레프트힛 카운트가 4의 배수가 될 때마다 하나씩 지운다.
        if (leftHitCount % 4 == 0 && leftHitCount > 0)
        {
            swordwavePanel.GetChild(leftHitCount / 4).gameObject.SetActive(false);
            return;
        }
        if (leftHitCount % 4 != 0 && leftHitCount > 0)
        {
            Debug.Log("여기로 오는 건 맞지?");
            // rectTransform의 회전각도를 빠르게 두트윈으로 바꿔서 흔들리는 효과를 준다.
            RectTransform rect = swordwavePanel.GetChild(leftHitCount / 4).GetComponent<RectTransform>();
            rect.DORotate(new Vector3(0, 0, -20), 0.1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                rect.DORotate(new Vector3(0, 0, 20), 0.1f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    rect.DORotate(new Vector3(0, 0, 0), 0.1f).SetEase(Ease.Linear);
                });
            });
        }
    }

    public void CoolDownProcess(InputType inputType, float seconds)
    {
        switch (inputType)
        {
            case InputType.Shift:
                BlockShiftUIForSeconds(seconds);
                break;
            case InputType.Q:
                BlockQUIForSeconds(seconds);
                break;
            case InputType.R:
                BlockRUIForSeconds(seconds);
                break;
            case InputType.Space:
                BlockSpacebarUIForSeconds(seconds);
                break;
            case InputType.RightClick:
                BlockRightClickUIForSeconds(seconds);
                // rightclickCoroutine = StartCoroutine(BlockRightClickUICoroutine(seconds));
                break;
            default:
                break;
        }
    }

    private void BlockShiftUIForSeconds(float seconds)
    {
        // 알파값이 200까지 갔다가 seconds동안 천천히 0으로 돌아오는 코드
        skillPanel.transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<Image>().DOFade(0.9f, 0).SetEase(Ease.Linear);
        skillPanel.transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<Image>().DOFade(0, 0).SetEase(Ease.Linear).SetDelay(seconds);
        // seconds동안 남은 시간을 초 단위로 표시해준다. 0일 때는 표시해주지 않는다.
        StartCoroutine(UpdateCountdownText(skillPanel.transform.GetChild(0).GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>(), seconds));

    }
    private void BlockSpacebarUIForSeconds(float seconds)
    {
        skillPanel.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Image>().DOFade(0.9f, 0).SetEase(Ease.Linear);
        skillPanel.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Image>().DOFade(0, 0).SetEase(Ease.Linear).SetDelay(seconds);
        StartCoroutine(UpdateCountdownText(skillPanel.transform.GetChild(0).GetChild(1).GetChild(3).GetComponent<TextMeshProUGUI>(), seconds));
    }

    private void BlockQUIForSeconds(float seconds)
    {
        skillPanel.transform.GetChild(0).GetChild(2).GetChild(2).GetComponent<Image>().DOFade(0.9f, 0).SetEase(Ease.Linear);
        skillPanel.transform.GetChild(0).GetChild(2).GetChild(2).GetComponent<Image>().DOFade(0, 0).SetEase(Ease.Linear).SetDelay(seconds);
        StartCoroutine(UpdateCountdownText(skillPanel.transform.GetChild(0).GetChild(2).GetChild(3).GetComponent<TextMeshProUGUI>(), seconds));

    }

    private void BlockRUIForSeconds(float seconds)
    {
        skillPanel.transform.GetChild(0).GetChild(3).GetChild(2).GetComponent<Image>().DOFade(0.9f, 0).SetEase(Ease.Linear);
        skillPanel.transform.GetChild(0).GetChild(3).GetChild(2).GetComponent<Image>().DOFade(0, 0).SetEase(Ease.Linear).SetDelay(seconds);
        StartCoroutine(UpdateCountdownText(skillPanel.transform.GetChild(0).GetChild(3).GetChild(3).GetComponent<TextMeshProUGUI>(), seconds));

    }

    private void BlockRightClickUIForSeconds(float seconds)
    {
        if (rightClickTween != null)
        {
            rightClickTween.Kill();
            rightClickTween = null;
        }
        skillPanel.transform.GetChild(0).GetChild(4).GetChild(2).GetComponent<Image>().DOFade(0.9f, 0).SetEase(Ease.Linear);
        rightClickTween = skillPanel.transform.GetChild(0).GetChild(4).GetChild(2).GetComponent<Image>().DOFade(0, 0).SetEase(Ease.Linear).SetDelay(seconds);
        rightclickCoroutine = StartCoroutine(UpdateCountdownText(skillPanel.transform.GetChild(0).GetChild(4).GetChild(3).GetComponent<TextMeshProUGUI>(), seconds));
    }

    public void CompleteCoolDownRightClick()
    {

        if (rightClickTween != null && rightClickTween.IsActive())
        {
            Debug.Log("여기에서 2차로 쿨타임 UI 초기화");
            rightClickTween.Kill();
            rightClickTween = null;
            skillPanel.transform.GetChild(0).GetChild(4).GetChild(2).GetComponent<Image>().DOFade(0, 0).SetEase(Ease.Linear);
        }
        if (rightclickCoroutine != null)
        {
            StopCoroutine(rightclickCoroutine);
            rightclickCoroutine = null;
            skillPanel.transform.GetChild(0).GetChild(4).GetChild(3).GetComponent<TextMeshProUGUI>().text = string.Empty;
        }
    }

    private IEnumerator UpdateCountdownText(TextMeshProUGUI textMeshProUGUI, float seconds)
    {
        // 초 단위로 카운트다운
        for (int i = Mathf.CeilToInt(seconds); i > 0; i--)
        {
            textMeshProUGUI.text = i.ToString(); // 텍스트 업데이트
            yield return new WaitForSeconds(1f); // 1초 대기
        }

        // 카운트다운이 끝난 후 텍스트를 비우거나 초기화
        textMeshProUGUI.text = string.Empty;
    }
}
