using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;


public class UIManager : MonoBehaviour
{

    [Header("Menu UI")]
    [SerializeField]
    private Button Info_Button;

    [Header("Popus UI")]
    [SerializeField]
    private GameObject MainPopup_Object;

    [Header("Paytable Popup")]
    [SerializeField]
    private GameObject PaytablePopup_Object;
    [SerializeField]
    private Button PaytableExit_Button;
    [SerializeField]
    private Image Info_Image;
    [SerializeField]
    private TMP_Text[] SymbolsText;
    [SerializeField]
    private TMP_Text[] SpecialSymbolsText;
    [SerializeField] private GameObject[] PageList;
    [SerializeField]
    private Button Next_Button;
    [SerializeField]
    private Button Previous_Button;
    private int paginationCounter = 0;
    [SerializeField] private Button[] paginationButtonGrp;

    [Header("Card Bonus Game")]
    [SerializeField]
    private Button DoubleBet_Button;
    [SerializeField]
    private GameObject BonusPanel;
    [SerializeField]
    private GameObject CardGame_Panel;
    [SerializeField]
    private GameObject CoffinGame_Panel;

    [Header("Win Popup")]
    [SerializeField]
    private GameObject WinPopup_Object;
    [SerializeField]
    private TMP_Text Win_Text;

    [SerializeField]
    private Button GameExit_Button;

    [SerializeField]
    private SlotBehaviour slotManager;

    [SerializeField] private AudioController audioController;


    private void Start()
    {
        if (PaytableExit_Button) PaytableExit_Button.onClick.RemoveAllListeners();
        if (PaytableExit_Button) PaytableExit_Button.onClick.AddListener(delegate { ClosePopup(PaytablePopup_Object); });

        if (Info_Button) Info_Button.onClick.RemoveAllListeners();
        if (Info_Button) Info_Button.onClick.AddListener(delegate { OpenPopup(PaytablePopup_Object); });

        if (GameExit_Button) GameExit_Button.onClick.RemoveAllListeners();
        if (GameExit_Button) GameExit_Button.onClick.AddListener(CallOnExitFunction);

        if (Next_Button) Next_Button.onClick.RemoveAllListeners();
        if (Next_Button) Next_Button.onClick.AddListener(delegate { TurnPage(true); });

        if (Previous_Button) Previous_Button.onClick.RemoveAllListeners();
        if (Previous_Button) Previous_Button.onClick.AddListener(delegate { TurnPage(false); });

        if (Previous_Button) Previous_Button.interactable = false;

        if (PaytablePopup_Object) PageList[0].SetActive(true);

    }

    internal void PopulateWin(double amount)
    {
        int initAmount = 0;
        if (WinPopup_Object) WinPopup_Object.SetActive(true);
        if (MainPopup_Object) MainPopup_Object.SetActive(true);

        DOTween.To(() => initAmount, (val) => initAmount = val, (int)amount, 5f).OnUpdate(() =>
        {
            if (Win_Text) Win_Text.text = initAmount.ToString();
        });

        DOVirtual.DelayedCall(6f, () =>
        {
            if (WinPopup_Object) WinPopup_Object.SetActive(false);
            if (MainPopup_Object) MainPopup_Object.SetActive(false);
            slotManager.CheckBonusGame();
        });
    }

    private void OpenBonusGame(bool type)
    {
        if (audioController) audioController.PlayButtonAudio();
        if (type)
        {
            if (CardGame_Panel) CardGame_Panel.SetActive(true);
            if (CoffinGame_Panel) CoffinGame_Panel.SetActive(false);
        }
        else
        {
            if (CardGame_Panel) CardGame_Panel.SetActive(false);
            if (CoffinGame_Panel) CoffinGame_Panel.SetActive(true);
        }
        if (BonusPanel) BonusPanel.SetActive(true);
    }

    private void OpenPopup(GameObject Popup)
    {
        if (audioController) audioController.PlayButtonAudio();
        if (Popup) Popup.SetActive(true);
        if (MainPopup_Object) MainPopup_Object.SetActive(true);
    }

    private void ClosePopup(GameObject Popup)
    {

        if (audioController) audioController.PlayButtonAudio();

        if (Popup) Popup.SetActive(false);
        if (MainPopup_Object) MainPopup_Object.SetActive(false);
    }

    internal void InitialiseUIData(string SupportUrl, string AbtImgUrl, string TermsUrl, string PrivacyUrl, Paylines symbolsText)
    {
        PopulateSymbolsPayout(symbolsText);
    }
    private void PopulateSymbolsPayout(Paylines paylines)
    {
        for (int i = 0; i < paylines.symbols.Count; i++)
        {
            if (i < SymbolsText.Length)
            {
                string text = null;
                if (paylines.symbols[i].multiplier._5x != 0)
                {
                    text += paylines.symbols[i].multiplier._5x;
                }
                if (paylines.symbols[i].multiplier._4x != 0)
                {
                    text += "\n" + paylines.symbols[i].multiplier._4x;
                }
                if (paylines.symbols[i].multiplier._3x != 0)
                {
                    text += "\n" + paylines.symbols[i].multiplier._3x;
                }
                if (paylines.symbols[i].multiplier._2x != 0)
                {
                    text += "\n" + paylines.symbols[i].multiplier._2x;
                }
                if (SymbolsText[i]) SymbolsText[i].text = text;
            }
        }
    }

    private void TurnPage(bool type)
    {
        if (audioController) audioController.PlayButtonAudio();

        if (type)
            paginationCounter++;
        else
            paginationCounter--;


        GoToPage(paginationCounter - 1);


    }

    private void GoToPage(int index)
    {

        paginationCounter = index + 1;

        paginationCounter = Mathf.Clamp(paginationCounter, 1, 5);

        if (Next_Button) Next_Button.interactable = !(paginationCounter >= 5);

        if (Previous_Button) Previous_Button.interactable = !(paginationCounter <= 1);

        for (int i = 0; i < PageList.Length; i++)
        {
            PageList[i].SetActive(false);
        }


        PageList[paginationCounter - 1].SetActive(true);
    }

    private void CallOnExitFunction()
    {
        Application.ExternalCall("window.parent.postMessage", "onExit", "*");
    }
}
