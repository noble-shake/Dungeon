using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour
{
    [Header("클래스별 아이콘 스프라이트")]
    [SerializeField] private Sprite[] classImageList;
    [SerializeField] private Image sourceImage;
    public ulong clientID;

    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetIconPerClass(PlayerClass characterClass)
    {
        if (characterClass == PlayerClass.Selecting)
        {
            sourceImage.sprite = classImageList[classImageList.Length - 1];
            return;
        }
        sourceImage.sprite = classImageList[(int)characterClass];
    }
}
