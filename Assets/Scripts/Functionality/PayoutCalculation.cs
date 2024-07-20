using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class PayoutCalculation : MonoBehaviour
{
    [SerializeField]
    private int x_Distance;
    [SerializeField]
    private int y_Distance;

    [SerializeField]
    private Transform LineContainer;
    [SerializeField]
    private GameObject[] Lines_Object;

    internal int LineCount;

    internal int currrentLineIndex;

    internal List<int> DontDestroy = new List<int>();

    [SerializeField] private Button[] left_buttons;
    [SerializeField] private Button[] right_buttons;
    
    [SerializeField]
    private Vector2 InitialLinePosition = new Vector2(-315, 100);
    [SerializeField] internal List<int> LineList;
    [SerializeField] internal List<int> DontDestroyLines = new List<int>();
    GameObject TempObj = null;

    
    internal void GeneratePayoutLinesBackend(int lineIndex = -1, int linecounter = 0, bool isStatic = false)
    {
        //ResetLines();
        if (lineIndex >= 0)
        {
            if (Lines_Object[lineIndex]) Lines_Object[lineIndex].SetActive(true);
            return;
        }

        if (isStatic)
        {
            TempObj = Lines_Object[lineIndex];
        }
        for (int i = 0; i < linecounter; i++)
        {
            Lines_Object[i].SetActive(true);
        }
    }


    internal void ResetStaticLine()
    {
        if(TempObj!=null)
        {
            TempObj.SetActive(false);
            TempObj = null;
        }
    }

    internal void ResetLines()
    {
        foreach (GameObject child in Lines_Object)
        {
            child.SetActive(false);
        }
    }

    internal void SetButtonActive(int LineCounter)
    {
        currrentLineIndex = LineCounter;

        for (int i = 0; i < LineCounter; i++)
        {
            Lines_Object[i].SetActive(true);

            left_buttons[i].interactable = true;
            right_buttons[i].interactable = true;
        }


        for (int j = LineCounter; j < left_buttons.Length; j++)
        {
            left_buttons[j].interactable = false;
            right_buttons[j].interactable = false;
        }
    }

}
