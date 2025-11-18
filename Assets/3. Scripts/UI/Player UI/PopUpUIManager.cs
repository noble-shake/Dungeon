
using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PopUpUIManager : MonoBehaviour
{
    [Header("Message Pop Up")]
    [SerializeField] GameObject popUpMessageGameObject;
    [SerializeField] TextMeshProUGUI popUpMessageText;

    [Header("You Died Pop Up")]
    [SerializeField] GameObject youDiedPopUpGameObject;
    [SerializeField] TextMeshProUGUI youDiedPopUpBackGroundText;
    [SerializeField] TextMeshProUGUI youDiedPopUpText;
    [SerializeField] CanvasGroup youDiedPopUpCanvasGroup;

    [Header("Boss Defeated Pop Up")]
    [SerializeField] GameObject bossDefeatedPopUpGameObject;
    [SerializeField] TextMeshProUGUI bossDefeatedPopUpBackGroundText;
    [SerializeField] TextMeshProUGUI bossDefeatedPopUpText;
    [SerializeField] CanvasGroup bossDefeatedPopUpCanvasGroup;

    [Header("Rpc Message Pop Up")]
    [SerializeField] GameObject rpcMessagePopUpGameObject;
    [SerializeField] TextMeshProUGUI rpcMessagePopUpBackGroundText;
    [SerializeField] TextMeshProUGUI rpcMessagePopUpText;
    [SerializeField] CanvasGroup rpcMessagePopUpCanvasGroup;

    private Color ReturnColor(ScriptColor scriptColor)
    {
        switch (scriptColor)
        {
            case ScriptColor.Red:
                return Color.red;
            case ScriptColor.Blue:
                return Color.blue;
            case ScriptColor.White:
                return Color.white;
            case ScriptColor.Black:
                return Color.black;
            default:
                return Color.white;
        }
    }

    public void SendPlayerMessagePopUp(string messageText, ScriptColor scriptColor = ScriptColor.White)
    {
        HUD_UIManager.instance.popUpWindowIsOpen = true;
        popUpMessageText.text = messageText;
        popUpMessageText.color = ReturnColor(scriptColor);
        popUpMessageGameObject.SetActive(true);
    }


    public void SendYouDiedPopUp(string messageText = "You Died", ScriptColor scriptColor = ScriptColor.White)
    {
        youDiedPopUpGameObject.SetActive(true);
        youDiedPopUpBackGroundText.characterSpacing = 0;
        if (string.IsNullOrEmpty(messageText))
        {
            youDiedPopUpText.text = "You Died";
            youDiedPopUpBackGroundText.text = "You Died";
        }
        else
        {
            youDiedPopUpText.text = messageText;
            youDiedPopUpBackGroundText.text = messageText;
        }
        youDiedPopUpText.color = ReturnColor(scriptColor);
        youDiedPopUpBackGroundText.color = ReturnColor(scriptColor);

        StartCoroutine(StretchPopUpTextOverTime(youDiedPopUpBackGroundText, 8, 19f));
        StartCoroutine(FadeInPopUpOverTime(youDiedPopUpCanvasGroup, 5));
        StartCoroutine(WaitThenFadeOutPopUpOverTime(youDiedPopUpCanvasGroup, 2, 5));
    }

    public void SendBossDefeatedPopUp(string bossDefeatedMessage, ScriptColor scriptColor = ScriptColor.White)
    {
        bossDefeatedPopUpText.text = bossDefeatedMessage;
        bossDefeatedPopUpBackGroundText.text = bossDefeatedMessage;

        bossDefeatedPopUpText.color = ReturnColor(scriptColor);
        bossDefeatedPopUpBackGroundText.color = ReturnColor(scriptColor);

        bossDefeatedPopUpGameObject.SetActive(true);
        bossDefeatedPopUpBackGroundText.characterSpacing = 0;
        StartCoroutine(StretchPopUpTextOverTime(bossDefeatedPopUpBackGroundText, 8, 19f));
        StartCoroutine(FadeInPopUpOverTime(bossDefeatedPopUpCanvasGroup, 5));
        StartCoroutine(WaitThenFadeOutPopUpOverTime(bossDefeatedPopUpCanvasGroup, 2, 5));
    }

    public void SendRpcMessagePopUp(string graceRestoredMessage, ScriptColor scriptColor = ScriptColor.White)
    {
        rpcMessagePopUpText.color = ReturnColor(scriptColor);
        rpcMessagePopUpBackGroundText.color = ReturnColor(scriptColor);

        rpcMessagePopUpGameObject.SetActive(true);
        rpcMessagePopUpText.text = graceRestoredMessage;
        rpcMessagePopUpBackGroundText.text = graceRestoredMessage;

        rpcMessagePopUpGameObject.SetActive(true);
        rpcMessagePopUpBackGroundText.characterSpacing = 0;
        StartCoroutine(StretchPopUpTextOverTime(rpcMessagePopUpBackGroundText, 8, 19f));
        StartCoroutine(FadeInPopUpOverTime(rpcMessagePopUpCanvasGroup, 5));
        StartCoroutine(WaitThenFadeOutPopUpOverTime(rpcMessagePopUpCanvasGroup, 2, 6));
    }

    private IEnumerator StretchPopUpTextOverTime(TextMeshProUGUI text, float duration, float stretchAmount)
    {
        if (duration > 0f)
        {
            text.characterSpacing = 0;
            float timer = 0;

            yield return null;

            while (duration < timer)
            {
                text.characterSpacing = Mathf.Lerp(text.characterSpacing, stretchAmount, duration * (Time.deltaTime / 20));
                yield return null;
            }
        }
    }

    private IEnumerator FadeInPopUpOverTime(CanvasGroup canvas, float duration)
    {
        if (duration > 0)
        {
            canvas.alpha = 0;
            float timer = 0;

            yield return null;

            while (timer < duration)
            {
                timer = timer + Time.deltaTime;
                canvas.alpha = Mathf.Lerp(canvas.alpha, 1, duration * Time.deltaTime);
                yield return null;

            }
        }

        canvas.alpha = 1;

        yield return null;
    }

    private IEnumerator WaitThenFadeOutPopUpOverTime(CanvasGroup canvas, float duration, float delay)
    {
        if (duration > 0)
        {
            while (delay > 0)
            {
                delay = delay - Time.deltaTime;
                yield return null;
            }
            canvas.alpha = 0;
            float timer = 0;

            yield return null;

            while (timer < duration)
            {
                timer = timer + Time.deltaTime;
                canvas.alpha = Mathf.Lerp(canvas.alpha, 0, duration * Time.deltaTime);
                yield return null;

            }
        }

        canvas.alpha = 0;

        yield return null;
    }

    public void CloseAllPopUpWindows()
    {
        popUpMessageGameObject?.SetActive(false);
        HUD_UIManager.instance.popUpWindowIsOpen = false;
    }
}
