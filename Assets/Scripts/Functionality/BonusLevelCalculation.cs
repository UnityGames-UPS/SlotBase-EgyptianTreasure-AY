using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;
using System.Linq;

public class BonusLevelCalculation : MonoBehaviour
{

    [SerializeField] private Button[] btn;
    [SerializeField] private TMP_Text[] textList;
    [SerializeField] private TMP_Text TotalText;

    [SerializeField] private GameObject RayCast_Panel;

    //[SerializeField] private List<double> result = new List<double>();
    // [SerializeField] private List<Button> tempButtonList = new List<Button>();
    int counter = 0;
    [SerializeField] private GameObject bonusGame;
    [SerializeField] private SlotBehaviour slotBehaviour;
    [SerializeField] private AudioController audioManager;
    [SerializeField] private SocketIOManager SocketManager;
    List<int> randomIndex = new List<int>();
    internal bool WaitForBonusResult = true;


    private double totalWin = 0;


    void Start()
    {
        for (int i = 0; i < btn.Length; i++)
        {
            int index = i;
            if (btn[index]) btn[index].onClick.RemoveAllListeners();
            if (btn[index]) btn[index].onClick.AddListener(delegate { OnSelectGrave(btn[index],  textList[index], index); });
        }
    }

    internal void StartBonusGame()
    {
        if (audioManager) audioManager.SwitchBGSound(true);
        if (RayCast_Panel) RayCast_Panel.SetActive(false);
        totalWin = 0;
        TotalText.text = "";
        Initialize();
        bonusGame.SetActive(true);
        //result.Clear();
        //result = bonusResult;
        //Debug.Log("bonus result in bonus game: ," + JsonConvert.SerializeObject(result));
    }

    IEnumerator resetgame(GameObject obj)
    {
        yield return new WaitForSeconds(2f);
        totalWin = 0;
        if (audioManager) audioManager.SwitchBGSound(false);
        slotBehaviour.updateBalance();
        bonusGame.SetActive(false);
        slotBehaviour.CheckPopups = false;
        obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y +0.5f, obj.transform.position.z);
    }

    private void Initialize()
    {
        randomIndex.Clear();
        counter = 0;
        totalWin = 0;
        foreach (var item in btn)
        {
            item.interactable = true;
            item.gameObject.SetActive(true);
        }

        foreach (var item in textList)
        {
            item.text = "";
            item.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            item.gameObject.SetActive(false);
        }

    }

    void OnSelectGrave(Button btn, TMP_Text text, int graveNo)
    {
        if (RayCast_Panel) RayCast_Panel.SetActive(true);
      
        StartCoroutine(DisplayBonusResult(btn, text, graveNo));
    }

    IEnumerator DisplayBonusResult(Button btn,  TMP_Text text, int graveNo)
    {
        
        SocketManager.OnBonusCollect(graveNo);
        StartCoroutine(PlayShakeAnimation(btn.gameObject));
        yield return new WaitUntil(() => SocketManager.isResultdone);

        if (SocketManager.bonusData.payload.payout == 0)
        {
            SocketManager.ResultData.payload.winAmount = SocketManager.bonusData.payload.winAmount;
            if (audioManager) audioManager.PlayBonusAudio("lose");
            
            text.gameObject.SetActive(true);
            text.text = "GAME OVER";
            text.gameObject.transform.position = new Vector3(text.gameObject.transform.position.x, text.gameObject.transform.position.y - 0.5f, text.gameObject.transform.position.z);
            text.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            btn.gameObject.SetActive(false);


            StartCoroutine(resetgame(text.gameObject));
            yield break;
        }
        if (audioManager) audioManager.PlayBonusAudio("win");
      

        double value = SocketManager.bonusData.payload.winAmount;
        text.text = "+" + value.ToString("0.000");
        
        totalWin = totalWin+ value;
        TotalText.text = totalWin.ToString("0.000");

        
        text.gameObject.SetActive(true);
        btn.gameObject.SetActive(false);
        

      
        if (RayCast_Panel) RayCast_Panel.SetActive(false);
    }


    IEnumerator PlayShakeAnimation(GameObject obj)
    {
        Vector3 originalPos = obj.transform.localPosition;
        float shakeAmount = 5f;
        float shakeSpeed = 50f;

        while (!SocketManager.isResultdone)
        {
            float offsetX = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
            float offsetY = Mathf.Cos(Time.time * shakeSpeed) * shakeAmount;

            obj.transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);

            yield return null;
        }

       
        obj.transform.localPosition = originalPos;
    }

}