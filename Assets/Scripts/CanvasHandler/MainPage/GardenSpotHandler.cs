using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GardenSpotHandler : MonoBehaviour
{
    public GameObject TodayUI;
    public GameObject NotYetObject;
    public GameObject Flower3_Base;
    public GameObject[] Flowers;
    public GameObject ButterFlies;
    public Text DateText;
    
    public TimeHandler.DateTimeStamp.DATE Date = TimeHandler.DateTimeStamp.DATE.MON;
    public bool isToday = false;
    public bool isNotUse = false;
    public bool isFuture = false;
    internal string DateString = "2021-1-1";
    internal int flowerCount = 0;
    public int Step = 0;

    public void Start() {
        FlowerShapeHandler[] shapes = this.GetComponentsInChildren<FlowerShapeHandler>();
        if(shapes != null) {
            foreach(FlowerShapeHandler shape in shapes) {
                shape.Change(( FlowerShapeHandler.STAGE_TYPE )Step);
            }
        }
    }

    public void InitSpot(DataHandler.GardenLog logData, string DateString, string DateType) {
        foreach (GameObject go in Flowers)
            go.SetActive(false);
        Flower3_Base.SetActive(false);
        TodayUI.SetActive(false);
        NotYetObject.SetActive(false);
        isToday = false;
        ButterFlies.SetActive(false);

        flowerCount = logData.flower;
        this.DateString = DateString;
        DateText.text = DateType;
        int cmpResult = TimeHandler.DateTimeStamp.CmpDateTimeStamp(
            TimeHandler.HomeCanvasTime,
            new TimeHandler.DateTimeStamp(DateString));
        if(cmpResult == 0) {
            isToday = true;
            TodayUI.SetActive(true);
        }

        if (logData == null) {
            if (cmpResult == -1) { NotYetObject.SetActive(true); isFuture = true; }
            return;
        }

        if(logData.item_0 > 0) {
            isNotUse = true;
            if (flowerCount == 0)
                NotYetObject.SetActive(true);
        } else {
            if(flowerCount == 1)
                Flowers[0].SetActive(true);
            else if (flowerCount == 2)
                Flowers[1].SetActive(true);
            else if (flowerCount == 3)
                Flowers[2].SetActive(true);
            else if (flowerCount >= 4) {
                Flower3_Base.SetActive(true);
                for (int i = 3; i < flowerCount; i++)
                    Flowers[i].SetActive(true);
            }
            if (flowerCount >= 10)
                ButterFlies.SetActive(true);
        }
    }

    public void GardenClick() {
        if (isFuture) return;
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.CLICKED);
        if (isNotUse) Debug.Log("하루보기 화면으로 이동");
        else Debug.Log("꽃키우기 화면으로 이동");
    }
}
