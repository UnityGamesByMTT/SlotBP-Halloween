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
    [SerializeField]
    private GameObject[] PageList;
    [SerializeField]
    private Button Next_Button;
    [SerializeField]
    private Button Previous_Button;
    //[SerializeField] private Button[] paginationButtonGrp;

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
    [SerializeField] private GameObject WinPopup_Object;
    [SerializeField] private Image Win_Image;
    [SerializeField] private TMP_Text Win_Text;
    [SerializeField] private Sprite BigWin_Sprite;
    [SerializeField] private Sprite HugeWin_Sprite;
    [SerializeField] private Sprite MegaWin_Sprite;

    [Header("Splash Screen")]
    [SerializeField] private GameObject spalsh_screen;
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private Image progressbar;



    [Header("scripts")]
    [SerializeField] private SlotBehaviour slotManager;
    [SerializeField] private AudioController audioController;
    [SerializeField] private SocketIOManager socketManager;


    [Header("settings PopUp")]
    [SerializeField] private GameObject SettingsPopUpObject;
    [SerializeField] private Button Setting_button;
    [SerializeField] private Button Setting_close_button;
    [SerializeField] private Slider SoundSlider;
    [SerializeField] private Slider MusicSlider;

    [Header("Quit popup")]
    [SerializeField] private GameObject QuitPopupObject;
    [SerializeField] private Button yes_button;
    [SerializeField] private Button no_button;
    [SerializeField] private Button cancel_button;
    [SerializeField] private Button Quit_button;

    [Header("LowBalance Popup")]
    [SerializeField] private Button LBExit_Button;
    [SerializeField] private GameObject LBPopup_Object;


    [Header("Disconnection Popup")]
    [SerializeField] private Button CloseDisconnect_Button;
    [SerializeField] private GameObject DisconnectPopup_Object;

    [Header("AnotherDevice Popup")]
    [SerializeField] private Button CloseAD_Button;
    [SerializeField] private GameObject ADPopup_Object;

    private bool isExit = false;
    private int paginationCounter = 0;

    private void Awake()
    {
        if (spalsh_screen) spalsh_screen.SetActive(true);
        StartCoroutine(LoadingRoutine());
    }

    private void Start()
    {
        if (PaytableExit_Button) PaytableExit_Button.onClick.RemoveAllListeners();
        if (PaytableExit_Button) PaytableExit_Button.onClick.AddListener(delegate { ClosePopup(PaytablePopup_Object); ResetInfoUI(); });

        if (Info_Button) Info_Button.onClick.RemoveAllListeners();
        if (Info_Button) Info_Button.onClick.AddListener(delegate { OpenPopup(PaytablePopup_Object); });


        if (Next_Button) Next_Button.onClick.RemoveAllListeners();
        if (Next_Button) Next_Button.onClick.AddListener(delegate { TurnPage(true); });

        if (Previous_Button) Previous_Button.onClick.RemoveAllListeners();
        if (Previous_Button) Previous_Button.onClick.AddListener(delegate { TurnPage(false); });

        if (SoundSlider) SoundSlider.onValueChanged.AddListener(delegate { ToggleSound(); });
        if (MusicSlider) MusicSlider.onValueChanged.AddListener(delegate { ToggleMusic(); });

        if (Quit_button) Quit_button.onClick.RemoveAllListeners();
        if (Quit_button) Quit_button.onClick.AddListener(delegate { OpenPopup(QuitPopupObject); });

        if (no_button) no_button.onClick.RemoveAllListeners();
        if (no_button) no_button.onClick.AddListener(delegate { ClosePopup(QuitPopupObject); });

        if (cancel_button) cancel_button.onClick.RemoveAllListeners();
        if (cancel_button) cancel_button.onClick.AddListener(delegate { ClosePopup(QuitPopupObject); });

        if (yes_button) yes_button.onClick.RemoveAllListeners();
        if (yes_button) yes_button.onClick.AddListener(CallOnExitFunction);

        if (LBExit_Button) LBExit_Button.onClick.RemoveAllListeners();
        if (LBExit_Button) LBExit_Button.onClick.AddListener(delegate { ClosePopup(LBPopup_Object); });

        if (CloseDisconnect_Button) CloseDisconnect_Button.onClick.RemoveAllListeners();
        if (CloseDisconnect_Button) CloseDisconnect_Button.onClick.AddListener(CallOnExitFunction);

        if (Setting_button) Setting_button.onClick.RemoveAllListeners();
        if (Setting_button) Setting_button.onClick.AddListener(delegate { OpenPopup(SettingsPopUpObject); });

        if (Setting_close_button) Setting_close_button.onClick.RemoveAllListeners();
        if (Setting_close_button) Setting_close_button.onClick.AddListener(delegate { ClosePopup(SettingsPopUpObject); });

        if (CloseAD_Button) CloseAD_Button.onClick.RemoveAllListeners();
        if (CloseAD_Button) CloseAD_Button.onClick.AddListener(CallOnExitFunction);

        ResetInfoUI();
    }


    internal void LowBalPopup()
    {
        OpenPopup(LBPopup_Object);
    }

    private IEnumerator LoadingRoutine()
    {
        StartCoroutine(LoadingTextAnimate());
        float fillAmount = 0.7f;
        progressbar.DOFillAmount(fillAmount, 2f).SetEase(Ease.Linear);
        yield return new WaitForSecondsRealtime(2f);
        yield return new WaitUntil(() => !socketManager.isLoading);
        progressbar.DOFillAmount(1, 1f).SetEase(Ease.Linear);
        yield return new WaitForSecondsRealtime(1f);
        if (spalsh_screen) spalsh_screen.SetActive(false);
        StopCoroutine(LoadingTextAnimate());
    }

    private IEnumerator LoadingTextAnimate()
    {
        while (true)
        {
            if (loadingText) loadingText.text = "Loading.";
            yield return new WaitForSeconds(0.5f);
            if (loadingText) loadingText.text = "Loading..";
            yield return new WaitForSeconds(0.5f);
            if (loadingText) loadingText.text = "Loading...";
            yield return new WaitForSeconds(0.5f);
        }
    }

    internal void PopulateWin(int value, double amount)
    {
        switch (value)
        {
            case 1:
                if (Win_Image) Win_Image.sprite = BigWin_Sprite;
                break;
            case 2:
                if (Win_Image) Win_Image.sprite = HugeWin_Sprite;
                break;
            case 3:
                if (Win_Image) Win_Image.sprite = MegaWin_Sprite;
                break;

        }
            StartPopupAnim(amount, false);


    }

    private void StartPopupAnim(double amount, bool jackpot = false)
    {
        audioController.PlaywitchAudio();
        int initAmount = 0;

        if (WinPopup_Object) WinPopup_Object.SetActive(true);

        if (MainPopup_Object) MainPopup_Object.SetActive(true);

        DOTween.To(() => initAmount, (val) => initAmount = val, (int)amount, 5f).OnUpdate(() =>
        {
                if (Win_Text) Win_Text.text = initAmount.ToString();

        });

        DOVirtual.DelayedCall(6f, () =>
        {
            audioController.StopWitchAudio();
            ClosePopup(WinPopup_Object);
            Win_Text.text="";
            slotManager.CheckPopups = false;
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
        if(!DisconnectPopup_Object.activeSelf){

        if (MainPopup_Object) MainPopup_Object.SetActive(false);
        }
    }

    internal void InitialiseUIData(string SupportUrl, string AbtImgUrl, string TermsUrl, string PrivacyUrl, Paylines symbolsText)
    {
        PopulateSymbolsPayout(symbolsText);
    }
    private void PopulateSymbolsPayout(Paylines paylines)
    {
        for (int i = 0; i < SymbolsText.Length; i++)
        {
            string text = null;
            if (paylines.symbols[i].Multiplier[0][0] != 0)
            {
                text += "5x - " + paylines.symbols[i].Multiplier[0][0];
            }
            if (paylines.symbols[i].Multiplier[1][0] != 0)
            {
                text += "\n4x - " + paylines.symbols[i].Multiplier[1][0];
            }
            if (paylines.symbols[i].Multiplier[2][0] != 0)
            {
                text += "\n3x - " + paylines.symbols[i].Multiplier[2][0];
            }
            if (SymbolsText[i]) SymbolsText[i].text = text;
        }

    }

    private void TurnPage(bool type)
    {
        if (audioController) audioController.PlayButtonAudio();

        if (type)
        {
            paginationCounter++;
            if(paginationCounter >= PageList.Length - 1)
            {
                NextPrevButton(1);
            }
            else
            {
                NextPrevButton(2);
            }
        }
        else
        {
            paginationCounter--;
            if(paginationCounter <= 0)
            {
                NextPrevButton(0);
            }
            else
            {
                NextPrevButton(2);
            }
        }


        GoToPage(paginationCounter);


    }

    private void ToggleMusic()
    {
        float value = MusicSlider.value;
        audioController.ToggleMute(value, "bg");

    }

    private void ToggleSound()
    {

        float value = SoundSlider.value;
        if (audioController) audioController.ToggleMute(value, "button");
        if (audioController) audioController.ToggleMute(value, "wl");
        if (audioController) audioController.ToggleMute(value, "ghost");


    }


    internal void ADfunction()
    {
        OpenPopup(ADPopup_Object); 
    }

    internal void DisconnectionPopup(bool isReconnection)
    {
        //if (isReconnection)
        //{
        //    OpenPopup(ReconnectPopup_Object);
        //}
        //else
        //{
        //ClosePopup(ReconnectPopup_Object);
        if (!isExit)
        {
            OpenPopup(DisconnectPopup_Object);
        }
        //}
    }

    private void GoToPage(int index)
    {
        if(index < PageList.Length)
        {
            for(int i = 0; i < PageList.Length; i++)
            {
                if(i == index)
                {
                    PageList[i].SetActive(true);
                }
                else
                {
                    PageList[i].SetActive(false);
                }
            }
        }
    }

    private void NextPrevButton(int m_config)
    {
        switch (m_config)
        {
            case 0:
                Next_Button.interactable = true;
                Previous_Button.interactable = false;
                break;
            case 1:
                Next_Button.interactable = false;
                Previous_Button.interactable = true;
                break;
            case 2:
                Next_Button.interactable = true;
                Previous_Button.interactable = true;
                break;
        }
    }

    private void ResetInfoUI()
    {
        paginationCounter = 0;
        NextPrevButton(0);
        GoToPage(paginationCounter);
    }

    private void CallOnExitFunction()
    {
        isExit = true;
        audioController.PlayButtonAudio();
        socketManager.CloseWebSocket();
        Application.ExternalCall("window.parent.postMessage", "onExit", "*");
    }
}
