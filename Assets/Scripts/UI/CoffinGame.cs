using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoffinGame : MonoBehaviour
{
    [SerializeField] private Button Coffin;
    [SerializeField] private Color32 text_color;
    [SerializeField] private TMP_Text text;
    [SerializeField] private ImageAnimation imageAnimation;
    [SerializeField] private BonusController _bonusManager;
    [SerializeField] private SocketIOManager SocketManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameObject Skeliton;
    [SerializeField] private AudioController audioController;
    [SerializeField] private double value = 0;

    //internal bool isOpen;

    void Start()
    {
        if (Coffin) Coffin.onClick.RemoveAllListeners();
        if (Coffin) Coffin.onClick.AddListener(OpenCase);
    }

    internal void ResetCase()
    {
        //isOpen = false;
        text.gameObject.SetActive(false);
        if (Skeliton.gameObject.activeSelf) Skeliton.gameObject.SetActive(false);
        imageAnimation.gameObject.SetActive(true);
        Coffin.interactable = true;

    }

    void OpenCase()
    {
        //if (isOpen) return;
        if (_bonusManager.isOpening) return;
        if (_bonusManager.isFinished) return;

        Coffin.interactable = false;
        PopulateCase();
        audioController.PlaySpinBonusAudio("bonus");
        imageAnimation.StartAnimation();
        StartCoroutine(setCase());
    }

    void PopulateCase()
    {
        value = _bonusManager.GetValue();
        if (value == 0)
        {
            text.text = "game over";
        }

        else
        {

            text.text = (_bonusManager.bet*value).ToString();

        }
    }

    IEnumerator setCase()
    {
        _bonusManager.isOpening = true;
        yield return new WaitUntil(() => !imageAnimation.isplaying);
        yield return new WaitForSeconds(0.3f);
        text.gameObject.SetActive(true);
        text.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, text_color);
        _bonusManager.isOpening = false;
        if(value>0)
        audioController.PlayWLAudio("bonuswin");
        else
        audioController.PlayWLAudio("bonuslose");
        _bonusManager.setTotalWin(value);
        if (text.text == "game over")
        {
            _bonusManager.isFinished = true;
            Skeliton.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            _bonusManager.GameOver();
        }
        
    }
}
