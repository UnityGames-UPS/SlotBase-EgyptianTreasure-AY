using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Space]
    [SerializeField] SocketIOManager socketManager;

    [Header("Menu UI")]
    [SerializeField]
    private Button Info_Button;
    [SerializeField]
    private Button SoundButton;
    [SerializeField]
    private Button exit_button;

    [Header("Popus UI")]
    [SerializeField]
    private GameObject MainPopup_Object;

    [Header("Sun Animation")]
    [SerializeField]
    private ImageAnimation Sun_Anim;

    [Header("Settings Popup")]
    [SerializeField]
    private GameObject SettingsPopup_Object;
    [SerializeField]
    private Button SettingsExit_Button;
    [SerializeField]
    private Button Sound_Button;
    [SerializeField]
    private Button Music_Button;
    [SerializeField]
    private Image SoundImage;
    [SerializeField]
    private Image MusicImage;
    [SerializeField]
    private Sprite Enabled_Sprite;
    [SerializeField]
    private Sprite Disabled_Sprite;

    [Header("Win Popup")]
    [SerializeField]
    private Sprite BigWin_Sprite;
    [SerializeField]
    private Sprite HugeWin_Sprite;
    [SerializeField]
    private Sprite MegaWin_Sprite;
    [SerializeField]
    private Sprite Jackpot_Sprite;
    [SerializeField]
    private Image Win_Image;
    [SerializeField]
    private GameObject WinPopup_Object;
    [SerializeField]
    private TMP_Text Win_Text;
    [SerializeField]
    private Button MegaWinHideBtn;
    private Tween WinTween;
    private Tween TextTween;

    [Header("Disconnection Popup")]
    [SerializeField]
    private Button CloseDisconnect_Button;
    [SerializeField]
    private GameObject DisconnectPopup_Object;

    [Header("Reconnection Popup")]
    [SerializeField]
    private GameObject ReconnectPopup_Object;


    [Header("LowBalance Popup")]
    [SerializeField]
    private Button LBExit_Button;
    [SerializeField]
    private GameObject LBPopup_Object;

    [Header("Quit Popup")]
    [SerializeField]
    private GameObject QuitPopup_Object;
    [SerializeField]
    private Button YesQuit_Button;
    [SerializeField]
    private Button NoQuit_Button;
    [SerializeField]
    private Button CrossQuit_Button;

    [Header("Paytable Popup")]
    [SerializeField]
    private GameObject PaytablePopup_Object;
    [SerializeField]
    private Button PaytableExit_Button;
    [SerializeField]
    private Button Next_Button;
    [SerializeField]
    private Button Previous_Button;
    [SerializeField]
    private TMP_Text Pagination_Text;
    [SerializeField]
    private Image Info_Image;
    [SerializeField]
    private Sprite[] Info_Sprites;
    [SerializeField]
    private TMP_Text BonusDesc_text;

    private int paginationCounter = 1;
    [SerializeField] private GameObject[] pageList;
    [SerializeField] private TMP_Text[] SymbolsText;
    [SerializeField] private TMP_Text[] SpecialSymbolsText;

    private bool isMusic = true;
    private bool isSound = true;
    private bool isExit = false;

    [SerializeField] private AudioController audioController;
    [SerializeField] private SlotBehaviour slotBehaviour;

    [SerializeField] internal GameObject Raycastblocker;


    private void Start()
    {
        if (PaytableExit_Button) PaytableExit_Button.onClick.RemoveAllListeners();
        if (PaytableExit_Button) PaytableExit_Button.onClick.AddListener(delegate { ClosePopup(PaytablePopup_Object); });

        if (Info_Button) Info_Button.onClick.RemoveAllListeners();
        if (Info_Button) Info_Button.onClick.AddListener(delegate { OpenPopup(PaytablePopup_Object); });

        paginationCounter = 1;
        if (Info_Image) Info_Image.sprite = Info_Sprites[paginationCounter - 1];

        if (Next_Button) Next_Button.onClick.RemoveAllListeners();
        if (Next_Button) Next_Button.onClick.AddListener(delegate { TurnPage(true); });

        if (Previous_Button) Previous_Button.onClick.RemoveAllListeners();
        if (Previous_Button) Previous_Button.onClick.AddListener(delegate { TurnPage(false); });

        if (Previous_Button) Previous_Button.interactable = false;

        if (Pagination_Text) Pagination_Text.text = paginationCounter + "  4";

        if (SoundButton) SoundButton.onClick.RemoveAllListeners();
        if (SoundButton) SoundButton.onClick.AddListener(delegate { OpenPopup(SettingsPopup_Object); });

        if (SettingsExit_Button) SettingsExit_Button.onClick.RemoveAllListeners();
        if (SettingsExit_Button) SettingsExit_Button.onClick.AddListener(delegate { ClosePopup(SettingsPopup_Object); });

        if (exit_button) exit_button.onClick.RemoveAllListeners();
        if (exit_button) exit_button.onClick.AddListener(delegate { OpenPopup(QuitPopup_Object); });

        if (NoQuit_Button) NoQuit_Button.onClick.RemoveAllListeners();
        if (NoQuit_Button) NoQuit_Button.onClick.AddListener(delegate { if (!isExit) { ClosePopup(QuitPopup_Object); } });

        if (CrossQuit_Button) CrossQuit_Button.onClick.RemoveAllListeners();
        if (CrossQuit_Button) CrossQuit_Button.onClick.AddListener(delegate { if (!isExit) { ClosePopup(QuitPopup_Object); } });

        if (LBExit_Button) LBExit_Button.onClick.RemoveAllListeners();
        if (LBExit_Button) LBExit_Button.onClick.AddListener(delegate { ClosePopup(LBPopup_Object); });

        if (YesQuit_Button) YesQuit_Button.onClick.RemoveAllListeners();
        if (YesQuit_Button) YesQuit_Button.onClick.AddListener(CallOnExitFunction);

        if (CloseDisconnect_Button) CloseDisconnect_Button.onClick.RemoveAllListeners();
        if (CloseDisconnect_Button) CloseDisconnect_Button.onClick.AddListener(delegate { CallOnExitFunction(); socketManager.ReactNativeCallOnFailedToConnect(); }); //BackendChanges

        if (audioController) audioController.ToggleMute(false);

        if (Sound_Button) Sound_Button.onClick.RemoveAllListeners();
        if (Sound_Button) Sound_Button.onClick.AddListener(ToggleSound);

        if (Music_Button) Music_Button.onClick.RemoveAllListeners();
        if (Music_Button) Music_Button.onClick.AddListener(ToggleMusic);

        if (SoundImage) SoundImage.sprite = Enabled_Sprite;
        if (MusicImage) MusicImage.sprite = Enabled_Sprite;

        if (MegaWinHideBtn) MegaWinHideBtn.onClick.RemoveAllListeners();
        if (MegaWinHideBtn) MegaWinHideBtn.onClick.AddListener(OnClickMegaWinHide);
    }

    internal void LowBalPopup()
    {
        OpenPopup(LBPopup_Object);
    }

    internal void DisconnectionPopup()
    {
        if (!isExit)
        {
            OpenPopup(DisconnectPopup_Object);
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
            case 4:
                if (Win_Image) Win_Image.sprite = Jackpot_Sprite;
                break;
        }

        StartPopupAnim(amount);
    }

    private void StartPopupAnim(double amount)
    {
        CloseAllpopups();
        if (audioController) audioController.PlayWLAudio("megawin");
        double initAmount = 0;
        if (WinPopup_Object) WinPopup_Object.SetActive(true);
        if (MainPopup_Object) MainPopup_Object.SetActive(true);

        TextTween = DOTween.To(() => initAmount, (val) => initAmount = val, amount, 5f).OnUpdate(() =>
        {
            if (Win_Text) Win_Text.text = initAmount.ToString("f3");
        });

        WinTween = DOVirtual.DelayedCall(6f, () =>
         {
             ClosePopup(WinPopup_Object);
             slotBehaviour.CheckPopups = false;
         });
    }

    private void OnClickMegaWinHide()
    {
        TextTween?.Kill();
        WinTween?.Kill();

        ClosePopup(WinPopup_Object);
        slotBehaviour.CheckPopups = false;
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
        if (!DisconnectPopup_Object.activeSelf)
        {
            if (MainPopup_Object) MainPopup_Object.SetActive(false);
        }
    }
    private void CloseAllpopups()
    {
        if (SettingsPopup_Object.activeSelf) ClosePopup(SettingsPopup_Object);
        if (PaytablePopup_Object.activeSelf) ClosePopup(PaytablePopup_Object);
    }
    private void ToggleMusic()
    {
        isMusic = !isMusic;
        if (isMusic)
        {
            if (MusicImage) MusicImage.sprite = Enabled_Sprite;
            audioController.ToggleMute(false, "bg");
        }
        else
        {
            if (MusicImage) MusicImage.sprite = Disabled_Sprite;
            audioController.ToggleMute(true, "bg");
        }
    }
    internal void ReconnectionPopup()
    {
        OpenPopup(ReconnectPopup_Object);
    }

    internal void CheckAndClosePopups()
    {
        if (ReconnectPopup_Object.activeInHierarchy)
        {
            ClosePopup(ReconnectPopup_Object);
        }
        if (DisconnectPopup_Object.activeInHierarchy)
        {
            ClosePopup(DisconnectPopup_Object);
        }
    }
    private void ToggleSound()
    {
        isSound = !isSound;
        if (isSound)
        {
            if (SoundImage) SoundImage.sprite = Enabled_Sprite;
            if (audioController) audioController.ToggleMute(false, "button");
            if (audioController) audioController.ToggleMute(false, "wl");
        }
        else
        {
            if (SoundImage) SoundImage.sprite = Disabled_Sprite;
            if (audioController) audioController.ToggleMute(true, "button");
            if (audioController) audioController.ToggleMute(true, "wl");
        }
    }

    private void TurnPage(bool type)
    {
        if (audioController) audioController.PlayButtonAudio();
        for (int i = 0; i < pageList.Length; i++)
        {
            pageList[i].SetActive(false);
        }
        if (type)
        {
            if (Previous_Button) Previous_Button.interactable = true;
            paginationCounter++;
            //if (Info_Image) Info_Image.sprite = Info_Sprites[paginationCounter - 1];
            pageList[paginationCounter - 1].SetActive(true);
            if (paginationCounter == 4)
            {
                if (Next_Button) Next_Button.interactable = false;
            }
            if (Pagination_Text) Pagination_Text.text = paginationCounter + "  4";

        }
        else
        {
            if (Next_Button) Next_Button.interactable = true;
            paginationCounter--;
            pageList[paginationCounter - 1].SetActive(true);

            //if (Info_Image) Info_Image.sprite = Info_Sprites[paginationCounter - 1];
            if (paginationCounter == 1)
            {
                if (Previous_Button) Previous_Button.interactable = false;
            }
            if (Pagination_Text) Pagination_Text.text = paginationCounter + "  4";
        }
    }
    internal void InitialiseUIData(Paylines symbolsText)
    {
        PopulateSymbolsPayout(symbolsText);
    }

    private void PopulateSymbolsPayout(Paylines paylines)
    {
        for (int i = 0; i < SymbolsText.Length; i++)
        {
            switch (paylines.symbols[i].name)
            {
                case "0":
                    string text = null;
                    if (paylines.symbols[i].multiplier[0] != 0)
                    {
                        text += "5x - " + paylines.symbols[i].multiplier[0] + "x";
                    }
                    if (paylines.symbols[i].multiplier[1] != 0)
                    {
                        text += "\n4x - " + paylines.symbols[i].multiplier[1] + "x";
                    }
                    if (paylines.symbols[i].multiplier[2] != 0)
                    {
                        text += "\n3x - " + paylines.symbols[i].multiplier[2] + "x";
                    }
                    if (SymbolsText[0]) SymbolsText[0].text = text;
                    if (SymbolsText[1]) SymbolsText[1].text = text;
                    break;
                case "4":
                    text = null;
                    if (paylines.symbols[i].multiplier[0] != 0)
                    {
                        text += "5x - " + paylines.symbols[i].multiplier[0] + "x";
                    }
                    if (paylines.symbols[i].multiplier[1] != 0)
                    {
                        text += "\n4x - " + paylines.symbols[i].multiplier[1] + "x";
                    }
                    if (paylines.symbols[i].multiplier[2] != 0)
                    {
                        text += "\n3x - " + paylines.symbols[i].multiplier[2] + "x";
                    }
                    if (SymbolsText[2]) SymbolsText[2].text = text;
                    if (SymbolsText[3]) SymbolsText[3].text = text;
                    if (SymbolsText[4]) SymbolsText[4].text = text;
                    if (SymbolsText[5]) SymbolsText[5].text = text;
                    break;
            }
        }

        for (int i = 0; i < paylines.symbols.Count; i++)
        {
            if (paylines.symbols[i].name.ToUpper() == "BONUS")
            {
                if (BonusDesc_text) BonusDesc_text.text = paylines.symbols[i].description.ToString();
            }
            if (paylines.symbols[i].name.ToUpper() == "JACKPOT")
            {
                if (SymbolsText[6]) SymbolsText[6].text = paylines.symbols[i].description.ToString();
                // if (SymbolsText[6]) SymbolsText[6].text = "Mega win triggered by 5 Jackpot symbols appearing anywhere on the result matrix. Payout: " + paylines.symbols[i].defaultPayout.ToString() + "x";
            }
        }
    }

    internal void StartSunAnim()
    {
        if (Sun_Anim) Sun_Anim.StartAnimation();
    }

    internal void StopSunAnim()
    {
        if (Sun_Anim) Sun_Anim.StopAnimation();
    }

    private void CallOnExitFunction()
    {
        slotBehaviour.CallCloseSocket();
        // Application.ExternalCall("window.parent.postMessage", "onExit", "*");
    }
}
