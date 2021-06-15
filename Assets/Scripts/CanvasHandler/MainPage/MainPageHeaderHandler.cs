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

    public void Awake() {
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

    public void OnEnable() {
        StartCoroutine(CheckUserDataLoaded());
    }

    public IEnumerator CheckUserDataLoaded() {
            yield return new WaitForSeconds(0.1f);
        StartCoroutine(DataHandler.ReadWaterLogs(DataHandler.User_id));
        StartCoroutine(DataHandler.ReadDrinkLogs(DataHandler.User_id));
        StartCoroutine(DataHandler.ReadPeeLogs(DataHandler.User_id));
        StartCoroutine(DataHandler.ReadPoopLogs(DataHandler.User_id));
        StartCoroutine(CheckLogDataLoaded());
    }

    public void DataReload() {
        StartCoroutine(DataHandler.ReadWaterLogs(DataHandler.User_id));
        StartCoroutine(DataHandler.ReadDrinkLogs(DataHandler.User_id));
        StartCoroutine(DataHandler.ReadPeeLogs(DataHandler.User_id));
        StartCoroutine(DataHandler.ReadPoopLogs(DataHandler.User_id));
        StartCoroutine(CheckLogDataLoaded());
    }

    public IEnumerator CheckLogDataLoaded() {
        while( !DataHandler.User_isWaterDataLoaded ||
               !DataHandler.User_isDrinkDataLoaded ||
               !DataHandler.User_isPeeDataLoaded   ||
               !DataHandler.User_isPooDataLoaded   )
            yield return 0;
        DataHandler.User_isWaterDataLoaded = false;
        DataHandler.User_isDrinkDataLoaded = false;
        DataHandler.User_isPeeDataLoaded = false;
        DataHandler.User_isPooDataLoaded = false;

        int water_num = 0, drink_num = 0, pee_num = 0, poo_num = 0;

        TimeHandler.DateTimeStamp currentDate = TimeHandler.HomeCanvasTime;
        foreach(DataHandler.WaterLog log in DataHandler.Water_logs.WaterLogs) {
            TimeHandler.DateTimeStamp logTime = new TimeHandler.DateTimeStamp(log.timestamp);
            if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(currentDate, logTime) == 0)
                water_num++;
        }
        foreach (DataHandler.DrinkLog log in DataHandler.Drink_logs.DrinkLogs) {
            TimeHandler.DateTimeStamp logTime = new TimeHandler.DateTimeStamp(log.timestamp);
            if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(currentDate, logTime) == 0)
                drink_num++;
        }
        foreach (DataHandler.PeeLog log in DataHandler.Pee_logs.PeeLogs) {
            TimeHandler.DateTimeStamp logTime = new TimeHandler.DateTimeStamp(log.timestamp);
            if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(currentDate, logTime) == 0)
                pee_num++;
        }
        foreach (DataHandler.PoopLog log in DataHandler.Poop_logs.PoopLogs) {
            TimeHandler.DateTimeStamp logTime = new TimeHandler.DateTimeStamp(log.timestamp);
            if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(currentDate, logTime) == 0)
                poo_num++;
        }
        WriteWaterCounter(water_num);
        WriteDrinkCounter(drink_num);
        WritePeeCounter(pee_num);
        WritePooCounter(poo_num);
    }
}
