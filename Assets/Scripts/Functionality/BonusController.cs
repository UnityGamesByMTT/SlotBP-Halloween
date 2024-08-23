using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class BonusController : MonoBehaviour
{
    [SerializeField]
    private GameObject Bonus_Object;
    [SerializeField]
    private SlotBehaviour slotManager;
    [SerializeField]
    private List<CoffinGame> BonusCases;
    [SerializeField]
    private AudioController _audioManager;

    [SerializeField]
    private List<int> CaseValues;

    [SerializeField] private TMP_Text TotalWin_text;

    int index = 0;

    internal bool isOpening;
    internal bool isFinished;
    private double totalWin;
    internal double bet { get; private set; }

    internal void GetCaseList(List<int> values, double betperline)
    {
        index = 0;
        CaseValues.Clear();
        CaseValues.TrimExcess();
        CaseValues = values;
        bet = betperline;


        StartBonus();
    }

    internal void setTotalWin(double amount) {
        totalWin += amount * bet;

        if (TotalWin_text) TotalWin_text.text = totalWin.ToString("f2");

    }

    internal void GameOver()
    {
        slotManager.CheckPopups = false;
        _audioManager.playBgAudio("normal");
        foreach (CoffinGame cases in BonusCases)
        {
            cases.ResetCase();
        }
        isFinished = false;
        TotalWin_text.text = "0.00";
        totalWin = 0;
        if (Bonus_Object) Bonus_Object.SetActive(false);
    }

    internal int GetValue()
    {
        int value = 0;

        value = CaseValues[index];

        index++;

        return value;
    }

    

    private void StartBonus()
    {
        _audioManager.playBgAudio("bonus");
        if (Bonus_Object) Bonus_Object.SetActive(true);
    }
}
