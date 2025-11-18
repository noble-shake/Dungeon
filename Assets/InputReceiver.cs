using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputReceiver : MonoBehaviour
{
    public PlayerControls playerControls;
    [SerializeField] Animator vcAnimator;
    [SerializeField] GameObject textUI;
    bool gameStart = false;


    private void Awake()
    {
        playerControls = new();

        playerControls.UI.AnyKey.performed += context => GameStart();
        playerControls.UI.LeftClick.performed += context => GameStart();
        playerControls.UI.RightClick.performed += context => GameStart();

        playerControls.Enable();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void GameStart()
    {
        if (!gameStart)
        {
            gameStart = true;
            textUI.SetActive(false);
            vcAnimator.SetTrigger("Game Start");
        }
    }
}
