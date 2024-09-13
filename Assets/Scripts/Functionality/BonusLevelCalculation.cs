using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BonusLevelCalculation : MonoBehaviour
{
    [SerializeField]
    private GameObject[] Prize_Columns;
    [SerializeField]
    private GameObject[] Prize_Texts;
    [SerializeField]
    private GameObject[] Prize_Dangers;
    [SerializeField]
    private Button[] Prize_Buttons;
    [SerializeField]
    private GameObject[] LeftArr_Objects;
    [SerializeField]
    private GameObject[] RightArr_Objects;
    [SerializeField]
    private GameObject SunLogoNormal;
    [SerializeField]
    private GameObject SunLogoFadeIn;
    [SerializeField]
    private GameObject SunLogoFadeOut;
    [SerializeField]
    private GameObject Multiplier_Object;
    [SerializeField]
    private Image Multiplier_Image;
    [SerializeField]
    private Sprite[] Multiplier_Sprites;

    private int arrowNum = 2;
    private int boxesOpened = 0;
    private int multiplier = 0;

    Coroutine SunRoutine = null;

    private void Start()
    {
        for(int i = 8; i > 4; i--)
        {
            if (Prize_Buttons[i]) Prize_Buttons[i].interactable = true;
        }
        if (LeftArr_Objects[arrowNum]) LeftArr_Objects[arrowNum].SetActive(true);
        if (RightArr_Objects[arrowNum]) RightArr_Objects[arrowNum].SetActive(true);

        if (SunLogoNormal) SunLogoNormal.SetActive(true);
        if (SunLogoFadeOut) SunLogoFadeOut.SetActive(false);
        if (SunLogoFadeIn) SunLogoFadeIn.SetActive(false);
        if (Multiplier_Object) Multiplier_Object.SetActive(false);
        if (Multiplier_Image) Multiplier_Image.sprite = Multiplier_Sprites[0];
    }

    private void OnEnable()
    {
        if(SunRoutine != null)
        {
            StopCoroutine(SunRoutine);
            SunRoutine = null;
        }
        SunRoutine = StartCoroutine(SunLogoRoutine());
    }

    private IEnumerator SunLogoRoutine()
    {
        bool isSun = true;
        while (true)
        {
            isSun = !isSun;
            if(isSun)
            {
                if (SunLogoNormal) SunLogoNormal.SetActive(false);
                if (SunLogoFadeOut) SunLogoFadeOut.SetActive(true);
                if (Multiplier_Object) Multiplier_Object.SetActive(true);
                yield return new WaitForSeconds(0.3f);
                if (SunLogoFadeOut) SunLogoFadeOut.SetActive(false);
            }
            else
            {
                if (SunLogoFadeIn) SunLogoFadeIn.SetActive(true);
                yield return new WaitForSeconds(0.3f);
                if (Multiplier_Object) Multiplier_Object.SetActive(false);
                if (SunLogoFadeIn) SunLogoFadeIn.SetActive(false);
                if (SunLogoNormal) SunLogoNormal.SetActive(true);
            }
            yield return new WaitForSeconds(3);
        }
    }

    public void CheckBox(int num)
    {
        if (Prize_Columns[num]) Prize_Columns[num].GetComponent<ImageAnimation>().StartAnimation();
        if (Prize_Buttons[num]) Prize_Buttons[num].interactable = false;
        DOVirtual.DelayedCall(0.5f, () =>
        {
            if (Prize_Columns[num]) Prize_Columns[num].SetActive(false);
            if (Prize_Texts[num]) Prize_Texts[num].SetActive(true);
            boxesOpened++;
            if (boxesOpened == 3)
            {
                for (int i = 8; i > 4; i--)
                {
                    if (Prize_Buttons[i]) Prize_Buttons[i].interactable = false;
                }
                NextLineUp(1);
            }
            else if(boxesOpened == 5)
            {
                for (int i = 4; i > 1; i--)
                {
                    if (Prize_Buttons[i]) Prize_Buttons[i].interactable = false;
                }
                NextLineUp(0);
            }
            else if(boxesOpened == 6)
            {
                multiplier++;
                if (Multiplier_Image) Multiplier_Image.sprite = Multiplier_Sprites[multiplier];
                //close the game
            }
        });
    }

    private void NextLineUp(int line)
    {
        if(line == 1)
        {
            for (int i = 4; i > 1; i--)
            {
                if (Prize_Buttons[i]) Prize_Buttons[i].interactable = true;
            }
        }
        else
        {
            for (int i = 1; i > -1; i--)
            {
                if (Prize_Buttons[i]) Prize_Buttons[i].interactable = true;
            }
        }
        if (LeftArr_Objects[arrowNum]) LeftArr_Objects[arrowNum].SetActive(false);
        if (RightArr_Objects[arrowNum]) RightArr_Objects[arrowNum].SetActive(false);
        arrowNum--;
        if (LeftArr_Objects[arrowNum]) LeftArr_Objects[arrowNum].SetActive(true);
        if (RightArr_Objects[arrowNum]) RightArr_Objects[arrowNum].SetActive(true);
        multiplier++;
        if (Multiplier_Image) Multiplier_Image.sprite = Multiplier_Sprites[multiplier];
    }

    private void OnDisable()
    {
        if (SunRoutine != null)
        {
            StopCoroutine(SunRoutine);
            SunRoutine = null;
        }
    }
}
