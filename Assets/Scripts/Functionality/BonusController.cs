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

    int index = 0;

    internal void GetCaseList(List<int> values)
    {
        index = 0;
        CaseValues.Clear();
        CaseValues.TrimExcess();
        CaseValues = values;

        foreach (CoffinGame cases in BonusCases)
        {
            cases.ResetCase();
        }

        for (int i = 0; i < CaseValues.Count; i++)
        {
            if (CaseValues[i] == -1)
            {
                CaseValues.RemoveAt(i);
                CaseValues.Add(-1);
            }
        }

        StartBonus();
    }


    internal void GameOver()
    {
        slotManager.CheckPopups = false;
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
        if (Bonus_Object) Bonus_Object.SetActive(true);
    }
}
