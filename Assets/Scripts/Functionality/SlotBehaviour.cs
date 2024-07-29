using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;

public class SlotBehaviour : MonoBehaviour
{
    [SerializeField]
    private RectTransform mainContainer_RT;

    [Header("Sprites")]
    [SerializeField]
    private Sprite[] myImages;

    [Header("Slot Images")]
    [SerializeField]
    private List<SlotImage> images;
    [SerializeField]
    private List<SlotImage> Tempimages;

    [Header("Slots Objects")]
    [SerializeField]
    private GameObject[] Slot_Objects;
    [Header("Slots Elements")]
    [SerializeField]
    private LayoutElement[] Slot_Elements;

    [Header("Slots Transforms")]
    [SerializeField]
    private Transform[] Slot_Transform;

    [Header("Line Button Objects")]
    [SerializeField]
    private List<GameObject> StaticLine_Objects;

    [Header("Buttons")]
    [SerializeField]
    private Button SlotStart_Button;
    [SerializeField]
    private Button MaxBet_Button;
    [SerializeField]
    private Button Lines_Button;
    [SerializeField]
    private Button AutoSpin_Button;
    [SerializeField] private Button AutoSpinStop_Button;
    [SerializeField] private Button BetOne_button;
    [SerializeField] private Button GambleButton;

    [Header("Animated Sprites")]
    [SerializeField]
    private Sprite[] Symbol1;
    [SerializeField]
    private Sprite[] Symbol3;
    [SerializeField]
    private Sprite[] Symbol4;
    [SerializeField]
    private Sprite[] Symbol5;
    [SerializeField]
    private Sprite[] Symbol6;
    [SerializeField]
    private Sprite[] Symbol7;
    [SerializeField]
    private Sprite[] Symbol8;
    [SerializeField]
    private Sprite[] Symbol9;
    [SerializeField]
    private Sprite[] Symbol10;
    [SerializeField]
    private Sprite[] Symbol2;
    [SerializeField]
    private Sprite[] Symbol11;

    [Header("Miscellaneous UI")]
    [SerializeField]
    private int[] Lines_num;
    [SerializeField]
    private TMP_Text Balance_text;
    [SerializeField]
    private TMP_Text TotalBet_text;
    [SerializeField]
    private Image Lines_Image;
    [SerializeField]
    private TMP_Text TotalWin_text;
    [SerializeField] private TMP_Text BetperLine_text;
    [SerializeField]
    private Sprite AutoSpinHover_Sprite;
    [SerializeField]
    private Sprite AutoSpin_Sprite;
    [SerializeField]
    private Image AutoSpin_Image;
    [SerializeField]
    private TMP_Text Lines_text;

    [SerializeField]
    private Sprite[] Lines_Sprites;
    private int LineCounter = 0;

    [Header("Misc Animations")]
    [SerializeField]
    private GameObject GhostIdle_Object;
    [SerializeField]
    private GameObject GhostLaughing_Object;
    [SerializeField]
    private ImageAnimation GhostIdle_Anim;
    [SerializeField]
    private ImageAnimation GhostLaugh_Anim;



    int tweenHeight = 0;

    [SerializeField]
    private GameObject Image_Prefab;

    [SerializeField]
    private PayoutCalculation PayCalculator;

    private List<Tweener> alltweens = new List<Tweener>();

    [SerializeField]
    private List<ImageAnimation> TempList;

    [SerializeField]
    private int IconSizeFactor = 100;
    [SerializeField] private int SpacingFactor;

    private int numberOfSlots = 5;

    [SerializeField]
    int verticalVisibility = 3;

    [SerializeField]
    private UIManager uiManager;
    [SerializeField]
    private BonusController _bonusManager;
    [SerializeField]
    private SocketIOManager SocketManager;
    [SerializeField] private AudioController audioController;
    private GameObject Gamble;
    [SerializeField] private GambleController gambleController;

    private Coroutine AutoSpinRoutine = null;
    private Coroutine tweenroutine = null;
    private bool IsAutoSpin = false;
    [SerializeField] private bool IsSpinning = false;
    private int BetCounter = 0;
    internal bool CheckPopups = false;
    private double currentBalance = 0;
    private double currentTotalBet = 0;

    private void Start()
    {

        if (SlotStart_Button) SlotStart_Button.onClick.RemoveAllListeners();
        if (SlotStart_Button) SlotStart_Button.onClick.AddListener(delegate { StartSlots(); });

        if (MaxBet_Button) MaxBet_Button.onClick.RemoveAllListeners();
        if (MaxBet_Button) MaxBet_Button.onClick.AddListener(MaxBet);

        if (Lines_Button) Lines_Button.onClick.RemoveAllListeners();
        if (Lines_Button) Lines_Button.onClick.AddListener(delegate { ToggleLine(); });

        if (Lines_Image) Lines_Image.sprite = Lines_Sprites[8];
        LineCounter = 0;

        if (GhostLaughing_Object) GhostLaughing_Object.SetActive(false);
        if (GhostIdle_Object) GhostIdle_Object.SetActive(true);

        if (AutoSpin_Button) AutoSpin_Button.onClick.RemoveAllListeners();
        if (AutoSpin_Button) AutoSpin_Button.onClick.AddListener(AutoSpin);

        if (AutoSpinStop_Button) AutoSpinStop_Button.onClick.RemoveAllListeners();
        if (AutoSpinStop_Button) AutoSpinStop_Button.onClick.AddListener(StopAutoSpin);

        if (BetOne_button) BetOne_button.onClick.RemoveAllListeners();
        if (BetOne_button) BetOne_button.onClick.AddListener(OnBetOne);
    }

    private void AutoSpin()
    {
        if (!IsAutoSpin)
        {

            IsAutoSpin = true;
            if (AutoSpinStop_Button) AutoSpinStop_Button.gameObject.SetActive(true);
            if (AutoSpin_Button) AutoSpin_Button.gameObject.SetActive(false);

            if (AutoSpinRoutine != null)
            {
                StopCoroutine(AutoSpinRoutine);
                AutoSpinRoutine = null;
            }
            AutoSpinRoutine = StartCoroutine(AutoSpinCoroutine());

        }



    }

    internal void SetInitialUI()
    {
        BetCounter = SocketManager.initialData.Bets.Count - 1;
        LineCounter = SocketManager.initialData.LinesCount.Count - 1;
        if (Lines_text) Lines_text.text = SocketManager.initialData.LinesCount[LineCounter].ToString();
        PayCalculator.SetButtonActive(SocketManager.initialData.LinesCount[LineCounter]);
        if (TotalBet_text) TotalBet_text.text = (SocketManager.initialData.Bets[BetCounter] * SocketManager.initialData.Lines.Count).ToString();
        if (TotalWin_text) TotalWin_text.text = SocketManager.playerdata.currentWining.ToString();
        if (Balance_text) Balance_text.text = SocketManager.playerdata.Balance.ToString();
        if (BetperLine_text) BetperLine_text.text = (SocketManager.initialData.Bets[BetCounter]).ToString();
        currentBalance = SocketManager.playerdata.Balance;
        currentTotalBet = SocketManager.initialData.Bets[BetCounter] * SocketManager.initialData.Lines.Count;
        CompareBalance();

        uiManager.InitialiseUIData(SocketManager.initUIData.AbtLogo.link, SocketManager.initUIData.AbtLogo.logoSprite, SocketManager.initUIData.ToULink, SocketManager.initUIData.PopLink, SocketManager.initUIData.paylines);
    }

    private void StopAutoSpin()
    {
        if (IsAutoSpin)
        {
            IsAutoSpin = false;
            if (AutoSpinStop_Button) AutoSpinStop_Button.gameObject.SetActive(false);
            if (AutoSpin_Button) AutoSpin_Button.gameObject.SetActive(true);
            StartCoroutine(StopAutoSpinCoroutine());
        }

    }

    private IEnumerator AutoSpinCoroutine()
    {

        while (IsAutoSpin)
        {
            StartSlots(IsAutoSpin);
            yield return tweenroutine;


        }
    }

    private IEnumerator StopAutoSpinCoroutine()
    {
        yield return new WaitUntil(() => !IsSpinning);
        ToggleButtonGrp(true);
        if (AutoSpinRoutine != null || tweenroutine != null)
        {
            StopCoroutine(AutoSpinRoutine);
            StopCoroutine(tweenroutine);
            tweenroutine = null;
            AutoSpinRoutine = null;
            StopCoroutine(StopAutoSpinCoroutine());
        }
    }

    void OnBetOne()
    {
        if (audioController) audioController.PlayButtonAudio();

        if (BetCounter < SocketManager.initialData.Bets.Count - 1)
        {
            BetCounter++;
        }
        else
        {
            BetCounter = 0;
        }
        Debug.Log("Index:" + BetCounter);

        currentTotalBet = SocketManager.initialData.Bets[BetCounter] * SocketManager.initialData.Lines.Count;
        if (BetperLine_text) BetperLine_text.text = (SocketManager.initialData.Bets[BetCounter]).ToString();
        if (TotalBet_text) TotalBet_text.text = (SocketManager.initialData.Bets[BetCounter] * SocketManager.initialData.Lines.Count).ToString();
        CompareBalance();

    }

    private void MaxBet()
    {
        if (audioController) audioController.PlayButtonAudio();
        BetCounter = SocketManager.initialData.Bets.Count - 1;
        currentTotalBet = SocketManager.initialData.Bets[BetCounter] * SocketManager.initialData.Lines.Count;

        if (TotalBet_text) TotalBet_text.text = currentTotalBet.ToString();
        if (BetperLine_text) BetperLine_text.text = SocketManager.initialData.Bets[BetCounter].ToString();

        if (currentTotalBet < currentBalance)
            CompareBalance();
    }

    private void ToggleLine()
    {
        if (audioController) audioController.PlayButtonAudio();
        LineCounter++;
        if (LineCounter == SocketManager.initialData.LinesCount.Count)
        {
            LineCounter = 0;
        }
        if (Lines_text) Lines_text.text = SocketManager.initialData.LinesCount[LineCounter].ToString();
        if (Lines_Image) Lines_Image.sprite = Lines_Sprites[LineCounter];
        PayCalculator.DontDestroy.Clear();
        PayCalculator.ResetLines();
        PayCalculator.GeneratePayoutLinesBackend(-1, SocketManager.initialData.LinesCount[LineCounter]);
        PayCalculator.SetButtonActive(SocketManager.initialData.LinesCount[LineCounter]);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && SlotStart_Button.interactable)
        {
            StartSlots();
        }
    }

    internal void PopulateInitalSlots(int number, List<int> myvalues)
    {
        PopulateSlot(myvalues, number);
    }

    internal void LayoutReset(int number)
    {
        if (Slot_Elements[number]) Slot_Elements[number].ignoreLayout = true;
        if (SlotStart_Button) SlotStart_Button.interactable = true;
    }

    private void PopulateSlot(List<int> values, int number)
    {
        if (Slot_Objects[number]) Slot_Objects[number].SetActive(true);
        for (int i = 0; i < 18; i++)
        {
            GameObject myImg = Instantiate(Image_Prefab, Slot_Transform[number]);
            images[number].slotImages.Add(myImg.transform.GetChild(0).GetComponent<Image>());
            images[number].slotImages[i].sprite = myImages[values[i]];
            PopulateAnimationSprites(images[number].slotImages[i].GetComponent<ImageAnimation>(), values[i]);
        }
        for (int k = 0; k < 2; k++)
        {
            GameObject mylastImg = Instantiate(Image_Prefab, Slot_Transform[number]);
            images[number].slotImages.Add(mylastImg.transform.GetChild(0).GetComponent<Image>());
            images[number].slotImages[images[number].slotImages.Count - 1].sprite = myImages[values[k]];
            PopulateAnimationSprites(images[number].slotImages[images[number].slotImages.Count - 1].GetComponent<ImageAnimation>(), values[k]);
        }
        if (mainContainer_RT) LayoutRebuilder.ForceRebuildLayoutImmediate(mainContainer_RT);
        tweenHeight = (18 * IconSizeFactor) - 280;
        GenerateMatrix(number);
    }

    private void PopulateAnimationSprites(ImageAnimation animScript, int val)
    {
        animScript.textureArray.Clear();
        animScript.textureArray.TrimExcess();
        switch (val)
        {
            case 0:
                for (int i = 0; i < Symbol1.Length; i++)
                {
                    animScript.textureArray.Add(Symbol1[i]);
                }
                break;
            case 1:
                for (int i = 0; i < Symbol3.Length; i++)
                {
                    animScript.textureArray.Add(Symbol3[i]);
                }
                break;
            case 2:
                for (int i = 0; i < Symbol4.Length; i++)
                {
                    animScript.textureArray.Add(Symbol4[i]);
                }
                break;
            case 3:
                for (int i = 0; i < Symbol5.Length; i++)
                {
                    animScript.textureArray.Add(Symbol5[i]);
                }
                break;
            case 4:
                for (int i = 0; i < Symbol6.Length; i++)
                {
                    animScript.textureArray.Add(Symbol6[i]);
                }
                break;
            case 5:
                for (int i = 0; i < Symbol7.Length; i++)
                {
                    animScript.textureArray.Add(Symbol7[i]);
                }
                break;
            case 6:
                for (int i = 0; i < Symbol8.Length; i++)
                {
                    animScript.textureArray.Add(Symbol8[i]);
                }
                break;
            case 7:
                for (int i = 0; i < Symbol9.Length; i++)
                {
                    animScript.textureArray.Add(Symbol9[i]);
                }
                break;
            case 8:
                for (int i = 0; i < Symbol10.Length; i++)
                {
                    animScript.textureArray.Add(Symbol10[i]);
                }
                break;
            case 9:
                for (int i = 0; i < Symbol2.Length; i++)
                {
                    animScript.textureArray.Add(Symbol2[i]);
                }
                break;
            case 10:
                for (int i = 0; i < Symbol11.Length; i++)
                {
                    animScript.textureArray.Add(Symbol11[i]);
                }
                break;
        }
    }

    private void StartSlots(bool autoSpin = false)
    {
        if (!autoSpin)
        {
            if (AutoSpinRoutine != null)
            {
                StopCoroutine(AutoSpinRoutine);
                StopCoroutine(tweenroutine);
                tweenroutine = null;
                AutoSpinRoutine = null;
            }

        }
        if (GhostLaughing_Object) GhostLaughing_Object.SetActive(false);
        if (GhostIdle_Object) GhostIdle_Object.SetActive(true);
        if (GhostIdle_Anim) GhostIdle_Anim.StartAnimation();

        if (audioController) audioController.PlayButtonAudio("spin");

        if (SlotStart_Button) SlotStart_Button.interactable = false;
        if (TempList.Count > 0)
        {
            StopGameAnimation();
        }
        PayCalculator.ResetLines();
        tweenroutine = StartCoroutine(TweenRoutine());
    }


    private IEnumerator TweenRoutine()
    {

        IsSpinning = true;

        if (currentBalance < currentTotalBet)
        {
            CompareBalance();
            if (IsAutoSpin)
            {

                StopAutoSpin();
                yield return new WaitForSeconds(1f);
            }
            yield break;
        }

        ToggleButtonGrp(false);
        gambleController.toggleDoubleButton(false);
        if (audioController) audioController.PlaySpinBonusAudio();
        for (int i = 0; i < numberOfSlots; i++)
        {
            InitializeTweening(Slot_Transform[i]);
            yield return new WaitForSeconds(0.1f);
        }

        double bet = 0;
        double balance = 0;
        try
        {
            bet = double.Parse(TotalBet_text.text);
        }
        catch (Exception e)
        {
            Debug.Log("Error while conversion " + e.Message);
        }

        try
        {
            balance = double.Parse(Balance_text.text);
        }
        catch (Exception e)
        {
            Debug.Log("Error while conversion " + e.Message);
        }

        balance = balance - bet;

        if (Balance_text) Balance_text.text = balance.ToString();

        SocketManager.AccumulateResult(BetCounter);

        yield return new WaitUntil(() => SocketManager.isResultdone);

        for (int j = 0; j < SocketManager.resultData.ResultReel.Count; j++)
        {
            List<int> resultnum = SocketManager.resultData.FinalResultReel[j]?.Split(',')?.Select(Int32.Parse)?.ToList();
            for (int i = 0; i < 5; i++)
            {
                if (images[i].slotImages[images[i].slotImages.Count - 5 + j]) images[i].slotImages[images[i].slotImages.Count - 5 + j].sprite = myImages[resultnum[i]];
                PopulateAnimationSprites(images[i].slotImages[images[i].slotImages.Count - 5 + j].gameObject.GetComponent<ImageAnimation>(), resultnum[i]);
            }
        }

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < numberOfSlots; i++)
        {
            yield return StopTweening(5, Slot_Transform[i], i);
        }

        yield return new WaitForSeconds(0.3f);
        if (audioController) audioController.StopApinBonusAudio();
        if (GhostIdle_Anim) GhostIdle_Anim.StopAnimation();
        CheckPayoutLineBackend(SocketManager.resultData.linesToEmit, SocketManager.resultData.FinalsymbolsToEmit, SocketManager.resultData.jackpot);
        KillAllTweens();

        //yield return new WaitForSeconds(2.5f);
        CheckPopups = true;

        //audioController.StopGhostAudio();
        currentBalance = SocketManager.playerdata.Balance;


        if (SocketManager.resultData.isBonus)
        {
            _bonusManager.GetCaseList(SocketManager.resultData.BonusResult, SocketManager.initialData.Bets[BetCounter]);

        }
        else if (SocketManager.resultData.WinAmout >= bet * 10 && SocketManager.resultData.WinAmout < bet * 15)
        {
            uiManager.PopulateWin(1, SocketManager.resultData.WinAmout);
        }
        else if (SocketManager.resultData.WinAmout >= bet * 15 && SocketManager.resultData.WinAmout < bet * 20)
        {
            uiManager.PopulateWin(2, SocketManager.resultData.WinAmout);
        }
        else if (SocketManager.resultData.WinAmout >= bet * 20)
        {
            uiManager.PopulateWin(3, SocketManager.resultData.WinAmout);
        }
        else if (SocketManager.resultData.WinAmout > 0 &&  SocketManager.resultData.WinAmout < bet * 10)
        {
            GhostIdle_Anim.gameObject.SetActive(false);
            if (GhostLaugh_Anim) GhostLaugh_Anim.gameObject.SetActive(true);
            GhostLaugh_Anim.StartAnimation();
            audioController.PlayWLAudio("win",1.1f);
            audioController.PlayGhostAudio(1.1f);
            yield return new WaitForSeconds(2f);
            audioController.StopWLAaudio();
            GhostLaugh_Anim.StopAnimation();
            GhostLaugh_Anim.gameObject.SetActive(false);
            GhostIdle_Anim.gameObject.SetActive(true);
            GhostIdle_Anim.StartAnimation();
            audioController.StopGhostAudio();
            CheckPopups = false;

        }
        else
        {

            CheckPopups = false;

        }

        yield return new WaitUntil(() => !CheckPopups);

        if (TotalWin_text) TotalWin_text.text = SocketManager.playerdata.currentWining.ToString();

        if (Balance_text) Balance_text.text = SocketManager.playerdata.Balance.ToString();

        print("checkpopups, " + CheckPopups);

        if (!IsAutoSpin)
        {
            if (SocketManager.playerdata.currentWining > 0 && SocketManager.playerdata.currentWining <= SocketManager.GambleLimit)
            {
                gambleController.gambleAmount=SocketManager.resultData.WinAmout;
                gambleController.toggleDoubleButton(true);
            }

            ToggleButtonGrp(true);
            IsSpinning = false;
        }
        else
        {
            yield return new WaitForSeconds(2f);
            IsSpinning = false;
        }


    }
    internal void CallCloseSocket()
    {
        SocketManager.CloseWebSocket();
    }


    internal void updateBalance(double gambleAmount=0,double winAmount=0)
    {
        // if (Balance_text) Balance_text.text = SocketManager.playerdata.Balance.ToString();
        if (Balance_text) Balance_text.text = (SocketManager.playerdata.Balance+(winAmount- gambleAmount)).ToString();
        if (TotalWin_text) TotalWin_text.text=winAmount.ToString();
    }

    private void CompareBalance()
    {
        if (currentBalance < currentTotalBet)
        {
            uiManager.LowBalPopup();
            if (AutoSpin_Button) AutoSpin_Button.interactable = false;
            if (SlotStart_Button) SlotStart_Button.interactable = false;
        }
        else
        {
            if (AutoSpin_Button) AutoSpin_Button.interactable = true;
            if (SlotStart_Button) SlotStart_Button.interactable = true;
        }
    }

    void ToggleButtonGrp(bool toggle)
    {

        if (SlotStart_Button) SlotStart_Button.interactable = toggle;
        if (Lines_Button) Lines_Button.interactable = toggle;
        if (MaxBet_Button) MaxBet_Button.interactable = toggle;
        if (AutoSpin_Button) AutoSpin_Button.interactable = toggle;
        if (BetOne_button) BetOne_button.interactable = toggle;

    }


    private void StartGameAnimation(GameObject animObjects)
    {
        int i = animObjects.transform.childCount;

        if (i > 0)
        {
            ImageAnimation temp = animObjects.GetComponent<ImageAnimation>();
            animObjects.transform.GetChild(0).gameObject.SetActive(true);

            temp.StartAnimation();

            TempList.Add(temp);
        }
        else
        {
            animObjects.GetComponent<ImageAnimation>().StartAnimation();

        }
    }

    private void StopGameAnimation()
    {
        for (int i = 0; i < TempList.Count; i++)
        {
            TempList[i].StopAnimation();
            if (TempList[i].transform.childCount > 0)
                TempList[i].transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void CheckPayoutLineBackend(List<int> LineId, List<string> points_AnimString, double jackpot = 0)
    {
        List<int> points_anim = null;
        if (LineId.Count > 0)
        {


            for (int i = 0; i < LineId.Count; i++)
            {
                PayCalculator.DontDestroy.Add(LineId[i]);
                PayCalculator.GeneratePayoutLinesBackend(LineId[i]);
            }

            if (jackpot > 0)
            {
                for (int i = 0; i < Tempimages.Count; i++)
                {
                    for (int k = 0; k < Tempimages[i].slotImages.Count; k++)
                    {
                        StartGameAnimation(Tempimages[i].slotImages[k].gameObject);
                    }
                }
            }
            else
            {
                for (int i = 0; i < points_AnimString.Count; i++)
                {
                    points_anim = points_AnimString[i]?.Split(',')?.Select(Int32.Parse)?.ToList();

                    for (int k = 0; k < points_anim.Count; k++)
                    {
                        if (points_anim[k] >= 10)
                        {
                            StartGameAnimation(Tempimages[(points_anim[k] / 10) % 10].slotImages[points_anim[k] % 10].gameObject);
                        }
                        else
                        {
                            StartGameAnimation(Tempimages[0].slotImages[points_anim[k]].gameObject);

                        }
                    }
                }
            }
        }



    }

    private void GenerateMatrix(int value)
    {
        for (int j = 0; j < 3; j++)
        {
            Tempimages[value].slotImages.Add(images[value].slotImages[images[value].slotImages.Count - 5 + j]);
        }
    }

    #region TweeningCode
    private void InitializeTweening(Transform slotTransform)
    {
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
        Tweener tweener = slotTransform.DOLocalMoveY(-tweenHeight, 0.2f).SetLoops(-1, LoopType.Restart).SetDelay(0);
        tweener.Play();
        alltweens.Add(tweener);
    }

    private IEnumerator StopTweening(int reqpos, Transform slotTransform, int index)
    {
        alltweens[index].Pause();
        int tweenpos = (reqpos * (IconSizeFactor + SpacingFactor)) - (IconSizeFactor + (2 * SpacingFactor));
        alltweens[index] = slotTransform.DOLocalMoveY(-tweenpos + 100 + (SpacingFactor > 0 ? SpacingFactor / 4 : 0), 0.5f).SetEase(Ease.OutElastic);
        yield return new WaitForSeconds(0.2f);
    }


    private void KillAllTweens()
    {
        for (int i = 0; i < numberOfSlots; i++)
        {
            alltweens[i].Kill();
        }
        alltweens.Clear();

    }
    #endregion

}

[Serializable]
public class SlotImage
{
    public List<Image> slotImages = new List<Image>(10);
}

