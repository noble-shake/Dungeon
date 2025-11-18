using DG.Tweening;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class ThrowShield : MonoBehaviour
{
    private PlayerManager player;

    [SerializeField] GameObject originShieldPivot;
    [SerializeField] GameObject throwingShield;
    [SerializeField] float goTime = 1f;
    [SerializeField] float comeTime = 1f;
    [SerializeField] float disappearDistance = 1f;

    public Transform shieldSlot;

    private void Awake()
    {
        player = GetComponent<PlayerManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originShieldPivot = shieldSlot.GetChild(0).GetChild(0).gameObject;
        throwingShield = Instantiate(player.playerInventoryManager.currentLeftHandWeapon.weaponModel);
        throwingShield.transform.localScale = player.playerInventoryManager.currentLeftHandWeapon.weaponModel.transform.localScale * 0.01f;
        throwingShield.SetActive(false);
    }

    // 현재 장착된 방패의 프리팹이 있어야 함. 
    // 그 프리팹을 인스턴스화한걸 예비본으로 가지고 있어야 함.
    // 장착되어있는 방패를 비활성화한다. 
    // 방패는 우선 직선을 단순히 왕복한다. 
    public void ThrowShieldAction()
    {
        Debug.Log("실행됨 설마?");
        throwingShield.transform.position = originShieldPivot.transform.position;
        throwingShield.transform.rotation = originShieldPivot.transform.rotation;
        throwingShield.transform.rotation *= Quaternion.Euler(100, 100, 100);
        Debug.Log($"{throwingShield.transform.rotation}");
        Debug.Log($"{originShieldPivot.transform.rotation}");

        Vector3 originalPosition = throwingShield.transform.position;
        Vector3 targetPosition = player.transform.position + Vector3.up * originalPosition.y + player.transform.forward * 5f;

        originShieldPivot.SetActive(false);
        throwingShield.SetActive(true);
        // throwingShield.transform.DORotate(new Vector3(0, 0, 0), goTime).SetEase(Ease.OutQuad);
        throwingShield.transform.DOMove(targetPosition, goTime).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                // 플레이어를 따라오면서 돌아오게 설정
                throwingShield.transform.DOMove(shieldSlot.transform.position + player.transform.forward * disappearDistance, comeTime).SetEase(Ease.Linear)
                    .OnUpdate(() =>
                    {
                        // 매 프레임마다 목표 위치를 플레이어의 현재 위치로 업데이트
                        throwingShield.transform.DOMove(shieldSlot.transform.position + player.transform.forward * disappearDistance, comeTime);
                    })
                    .OnComplete(() =>
                    {
                        print("내가 과연 여기서 실행되고 있을까?");
                        // 방패가 원래 위치로 돌아왔을 때 실행한다.
                        originShieldPivot.SetActive(true);
                        throwingShield.SetActive(false);
                    });
            });


    }
}
