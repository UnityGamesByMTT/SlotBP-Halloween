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
    [SerializeField]
    private GameObject Opening;
    [SerializeField] private ImageAnimation imageAnimation;
    [SerializeField] private BonusController _bonusManager;
    [SerializeField] private SocketIOManager SocketManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameObject Skeliton;
    [SerializeField]
    internal bool isOpen;

    void Start()
    {
        if (Coffin) Coffin.onClick.RemoveAllListeners();
        if (Coffin) Coffin.onClick.AddListener(OpenCase);
    }

    internal void ResetCase()
    {
        isOpen = false;
        text.gameObject.SetActive(false);
    }

    void OpenCase()
    {
        if (isOpen)
            return;
        PopulateCase();
        imageAnimation.StartAnimation();
        Opening.SetActive(true);
        Coffin.gameObject.SetActive(true);
        StartCoroutine(setCase());
    }

    void PopulateCase()
    {
        int value = _bonusManager.GetValue();
        if (value == -1)
        {
            text.text = "game over";
        }

        else
        {
            text.text = value.ToString();
        }
    }

    IEnumerator setCase()
    {
        yield return new WaitUntil(() => !imageAnimation.isplaying);
        yield return new WaitForSeconds(0.3f);
        text.gameObject.SetActive(true);
        text.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, text_color);
        isOpen = true;
        if (text.text == "game over")
        {
            Skeliton.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            _bonusManager.GameOver();
        }
        
    }
}
