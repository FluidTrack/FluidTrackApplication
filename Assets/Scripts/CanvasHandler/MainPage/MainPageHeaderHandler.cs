using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPageHeaderHandler : MonoBehaviour
{
    public static MainPageHeaderHandler Instance;
    public Text TodayText;
    public Text WaterCountText;
    public Text DrinkCountText;
    public Text PooCountText;
    public Text PeeCountText;

    public void Start() {
        Instance = this;
    }

    public void WriteWaterCounter(int count) {
        WaterCountText.text = count.ToString();
    }

    public void WriteDrinkCounter(int count) {
        DrinkCountText.text = count.ToString();
    }

    public void WritePooCounter(int count) {
        PooCountText.text = count.ToString();
    }

    public void WritePeeCounter(int count) {
        PeeCountText.text = count.ToString();
    }

    public void WriteTodayDate(TimeHandler.DateTimeStamp today) {
        TodayText.text = today.Months + "월 " + today.Days + "일 " +
                         TimeHandler.DateTimeStamp.DateList[today.Date];
    }
}
