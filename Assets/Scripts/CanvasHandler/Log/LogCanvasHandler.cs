using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogCanvasHandler : MonoBehaviour
{
    public static LogCanvasHandler Instance;
    public Image DateLeftButton;
    public Image DateRightButton;
    public Image TimeLeftButton;
    public Image TimeRightButton;
    public GameObject WaterButton;
    public GameObject PeeButton;
    public GameObject PooButton;
    public GameObject DrinkButton;
    public GameObject UpShield;
    public GameObject DownShield;
    public Color ActiveColor;
    public Color InactiveColor;
    public Text LogCanvasTimeText;
    public Text DebugText;
    public Text DebugTextLabel;
    public GameObject WaterLogPrefab;
    public GameObject DrinkLogPrefab;
    public GameObject PeeLogPrefab;
    public GameObject PoopLogPrefab;
    public RectTransform TimeZoneBar;
    public Transform[] UpSlot;
    public Transform[] DownSlot;

    public enum LOG_TYPE { NONE, WATER, DRINK, PEE, POOP, };
    public static string[] LogTypeList = { "None Log", "Water Log", "Drink Log", "Pee Log", "Poop Log" };

    internal string TargetDateString = "";
    internal bool WaterButtonClicked = false;
    internal bool PooButtonClicked = false;
    internal bool PeeButtonClicked = false;
    internal bool DrinkButtonClicked = false;

    private TimeHandler.DateTimeStamp targetDate = null;
    private List<Log> totalLog;
    private List<DataHandler.WaterLog> todayWaterLogs;
    private List<DataHandler.DrinkLog> todayDrinkLogs;
    private List<DataHandler.PeeLog>   todayPeeLogs;
    private List<DataHandler.PoopLog>  todayPoopLogs;
    private List<GameObject>           spawnObjects;
    private int todayFirstHour;
    private int todayLastHour;
    private int currentFirstHour;
    private int currentLastHour;
    private float barSize = 110f;
    private float offset = 20f;

    public class Log {
        public TimeHandler.DateTimeStamp Time;
        public int LogId;
        public DataHandler.WaterLog WaterLog;
        public DataHandler.DrinkLog DrinkLog;
        public DataHandler.PeeLog   PeeLog;
        public DataHandler.PoopLog  PoopLog;
        public LOG_TYPE LogType;

        public Log (DataHandler.WaterLog log) { 
            WaterLog = log; PeeLog = null; PoopLog = null; DrinkLog = null;
            this.LogId = log.log_id;
            Time = new TimeHandler.DateTimeStamp(log.timestamp);
            LogType = LOG_TYPE.WATER;
        }
        public Log (DataHandler.DrinkLog log) { 
            WaterLog = null; PeeLog = null; PoopLog = null; DrinkLog = log;
            this.LogId = log.log_id;
            Time = new TimeHandler.DateTimeStamp(log.timestamp);
            LogType = LOG_TYPE.DRINK;
        }
        public Log (DataHandler.PeeLog log)   { 
            WaterLog = null; PeeLog = log; PoopLog = null; DrinkLog = null;
            this.LogId = log.log_id;
            Time = new TimeHandler.DateTimeStamp(log.timestamp);
            LogType = LOG_TYPE.PEE;
        }
        public Log (DataHandler.PoopLog log)  { 
            WaterLog = null; PeeLog = null; PoopLog = log; DrinkLog = null;
            this.LogId = log.log_id;
            Time = new TimeHandler.DateTimeStamp(log.timestamp);
            LogType = LOG_TYPE.POOP;
        }

        public override string ToString() {
            try {
                string result = "[" + Time.ToString() + "] " + LogTypeList[(int)LogType];
                if(LogType == LOG_TYPE.DRINK) { result += " / Volume : " + DrinkLog.volume; }
                else if(LogType == LOG_TYPE.POOP) { result += " / Type : " + PoopLog.type; }
                return result;
            } catch(System.Exception e) { e.ToString(); return "To String Error...!"; }
        }
    }

    public void Awake() {
        Instance = this;
    }

    public void Start() {
        totalLog = new List<Log>();
        spawnObjects   = new List<GameObject>();
        DebugText.gameObject.SetActive(TotalManager.instance.isDebugMode);
        DebugTextLabel.gameObject.SetActive(TotalManager.instance.isDebugMode);
    }

    public void OnDisable() {
        DrawOff();
    }

    public void DrawOff() {
        for(int i = 0; i < spawnObjects.Count; i ++) {
            GameObject temp = spawnObjects[i];
            spawnObjects[i] = null;
            Destroy(temp);
        }
        foreach(Transform tr in UpSlot) {

        }
        spawnObjects.Clear();
        totalLog.Clear();
    }

    public void OnEnable() {
        StartCoroutine(UserDataCheck());
        TimeHandler.GetCurrentTime();
        WriteTimeStamp(TimeHandler.LogCanvasTime);

        if (TargetDateString != "") {
            targetDate = new TimeHandler.DateTimeStamp(TargetDateString);
            TargetDateString = "";
        }
    }

    public void WriteTimeStamp(TimeHandler.DateTimeStamp writeTime) {
        string str = writeTime.Months + "월 ";
        str += writeTime.Days + "일 ";
        str += TimeHandler.DateTimeStamp.DateList[writeTime.Date];
        LogCanvasTimeText.text = str;
    }

    public IEnumerator UserDataCheck() {
        while(!DataHandler.User_isDataLoaded) {
            yield return 0;
        }

        //===================================================
        //TO-DO :
        //===================================================
        //
        //  식사 아이콘 배치
        //
        //===================================================

        StartCoroutine(DataHandler.ReadWaterLogs(DataHandler.User_id));
        StartCoroutine(DataHandler.ReadDrinkLogs(DataHandler.User_id));
        StartCoroutine(DataHandler.ReadPeeLogs(DataHandler.User_id));
        StartCoroutine(DataHandler.ReadPoopLogs(DataHandler.User_id));
        StartCoroutine(LogDataLoadChecks());
    }

    public IEnumerator LogDataLoadChecks() {
        while(!DataHandler.User_isWaterDataLoaded) { yield return 0; }
        DataHandler.User_isWaterDataLoaded = false;

        while (!DataHandler.User_isDrinkDataLoaded) { yield return 0; }
        DataHandler.User_isDrinkDataLoaded = false;

        while (!DataHandler.User_isPeeDataLoaded) { yield return 0; }
        DataHandler.User_isPeeDataLoaded = false;

        while (!DataHandler.User_isPooDataLoaded) { yield return 0; }
        DataHandler.User_isPooDataLoaded = false;

        foreach(DataHandler.WaterLog log in DataHandler.Water_logs.WaterLogs)
            totalLog.Add(new Log(log));
        foreach (DataHandler.DrinkLog log in DataHandler.Drink_logs.DrinkLogs)
            totalLog.Add(new Log(log));
        foreach (DataHandler.PeeLog log in DataHandler.Pee_logs.PeeLogs)
            totalLog.Add(new Log(log));
        foreach (DataHandler.PoopLog log in DataHandler.Poop_logs.PoopLogs)
            totalLog.Add(new Log(log));
        yield return 0;

        totalLog.Sort(delegate (Log a, Log b) {
            return TimeHandler.DateTimeStamp.CmpDateTimeStampDetail(a.Time,b.Time);
        });

        yield return 0;
        DrawLogs();
    }

    public void DrawLogs() {
        if(targetDate != null) {
            TimeHandler.LogCanvasTime = targetDate;
            targetDate = null;
        }
        currentFirstHour = 7;
        currentLastHour = currentFirstHour + 12;
        moveTimeZone(currentFirstHour);
        WriteTimeStamp(TimeHandler.LogCanvasTime);
        bool isThereLog = false;
        foreach (Log log in totalLog) {
            if(TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                TimeHandler.LogCanvasTime,log.Time) == 0 ) {
                if (isThereLog) {
                    isThereLog = true;
                    currentFirstHour = log.Time.Hours;
                    if (currentFirstHour >= 13) currentFirstHour = 12;
                    currentLastHour = currentFirstHour + 12;
                    moveTimeZone(currentFirstHour);
                }
                log.ToString();
                switch(log.LogType) {
                    case LOG_TYPE.WATER: CreateWaterLog(log.WaterLog,log.Time.Hours); break;
                    case LOG_TYPE.DRINK: CreateDrinkLog(log.DrinkLog,log.Time.Hours); break;
                    case LOG_TYPE.PEE:   CreatePeeLog(log.PeeLog,log.Time.Hours);     break;
                    case LOG_TYPE.POOP:  CreatePoopLog(log.PoopLog, log.Time.Hours);  break;
                }
            }
        }
    }

    public void moveTimeZone(int startHour) {
        TimeZoneBar.anchoredPosition = new Vector2(-startHour * (barSize + offset),0f);
    }

    public void CreateWaterLog(DataHandler.WaterLog log, int hour) {
        if (hour < currentFirstHour || hour >= currentLastHour) return;
        int index = hour - currentFirstHour;

        GameObject newLog = Instantiate(WaterLogPrefab, UpSlot[index]);
        SlotHandler slot = UpSlot[index].GetComponent<SlotHandler>();
        slot.WaterCount++;
        slot.WaterTop = newLog.GetComponent<LogSpriteHandler.LogScript>();
    }

    public void CreateDrinkLog(DataHandler.DrinkLog log, int hour) {
        if (hour < currentFirstHour || hour >= currentLastHour) return;
        int index = hour - currentFirstHour;

        GameObject newLog = Instantiate(DrinkLogPrefab, UpSlot[index]);
        SlotHandler slot = UpSlot[index].GetComponent<SlotHandler>();
        slot.DrinkCount++;
        slot.DrinkTop = newLog.GetComponent<LogSpriteHandler.LogScript>();
    }

    public void CreatePeeLog(DataHandler.PeeLog log, int hour) {
        if (hour < currentFirstHour || hour >= currentLastHour) return;
        int index = hour - currentFirstHour;

        GameObject newLog = Instantiate(PeeLogPrefab, DownSlot[index]);
        SlotHandler slot = DownSlot[index].GetComponent<SlotHandler>();
        slot.PeeCount++;
        slot.PeeTop = newLog.GetComponent<LogSpriteHandler.LogScript>();
    }

    public void CreatePoopLog(DataHandler.PoopLog log, int hour) {
        if (hour < currentFirstHour || hour >= currentLastHour) return;
        int index = hour - currentFirstHour;

        GameObject newLog = Instantiate(PoopLogPrefab, DownSlot[index]);
        SlotHandler slot = DownSlot[index].GetComponent<SlotHandler>();
        slot.PooCount++;
        slot.PooTop = newLog.GetComponent<LogSpriteHandler.LogScript>();
    }

    public void WaterPlusButton() {
        if(!WaterButtonClicked) {
            DrinkButton.GetComponent<Button>().enabled = false;
            PeeButton.GetComponent<Button>().enabled = false;
            PooButton.GetComponent<Button>().enabled = false;
            DrinkButton.GetComponent<Image>().color = InactiveColor;
            PeeButton.GetComponent<Image>().color = InactiveColor;
            PooButton.GetComponent<Image>().color = InactiveColor;
            UpShield.SetActive(true);
            WaterButtonClicked = true;
        } else {
            DrinkButton.GetComponent<Button>().enabled = true;
            PeeButton.GetComponent<Button>().enabled = true;
            PooButton.GetComponent<Button>().enabled = true;
            DrinkButton.GetComponent<Image>().color = ActiveColor;
            PeeButton.GetComponent<Image>().color = ActiveColor;
            PooButton.GetComponent<Image>().color = ActiveColor;
            UpShield.SetActive(false);
            WaterButtonClicked = false;
        }
    }

    public void DrinkPlusButton() {
        if (!DrinkButtonClicked) {
            WaterButton.GetComponent<Button>().enabled = false;
            PeeButton.GetComponent<Button>().enabled = false;
            PooButton.GetComponent<Button>().enabled = false;
            WaterButton.GetComponent<Image>().color = InactiveColor;
            PeeButton.GetComponent<Image>().color = InactiveColor;
            PooButton.GetComponent<Image>().color = InactiveColor;
            UpShield.SetActive(true);
            DrinkButtonClicked = true;
        } else {
            WaterButton.GetComponent<Button>().enabled = true;
            PeeButton.GetComponent<Button>().enabled = true;
            PooButton.GetComponent<Button>().enabled = true;
            WaterButton.GetComponent<Image>().color = ActiveColor;
            PeeButton.GetComponent<Image>().color = ActiveColor;
            PooButton.GetComponent<Image>().color = ActiveColor;
            UpShield.SetActive(false);
            DrinkButtonClicked = false;
        }
    }

    public void PeePlusButton() {
        if (!PeeButtonClicked) {
            WaterButton.GetComponent<Button>().enabled = false;
            DrinkButton.GetComponent<Button>().enabled = false;
            PooButton.GetComponent<Button>().enabled = false;
            WaterButton.GetComponent<Image>().color = InactiveColor;
            DrinkButton.GetComponent<Image>().color = InactiveColor;
            PooButton.GetComponent<Image>().color = InactiveColor;
            DownShield.SetActive(true);
            PeeButtonClicked = true;
        } else {
            WaterButton.GetComponent<Button>().enabled = true;
            DrinkButton.GetComponent<Button>().enabled = true;
            PooButton.GetComponent<Button>().enabled = true;
            WaterButton.GetComponent<Image>().color = ActiveColor;
            DrinkButton.GetComponent<Image>().color = ActiveColor;
            PooButton.GetComponent<Image>().color = ActiveColor;
            DownShield.SetActive(false);
            PeeButtonClicked = false;
        }
    }

    public void PooPlusButton() {
        if (!PooButtonClicked) {
            WaterButton.GetComponent<Button>().enabled = false;
            DrinkButton.GetComponent<Button>().enabled = false;
            PeeButton.GetComponent<Button>().enabled = false;
            WaterButton.GetComponent<Image>().color = InactiveColor;
            DrinkButton.GetComponent<Image>().color = InactiveColor;
            PeeButton.GetComponent<Image>().color = InactiveColor;
            DownShield.SetActive(true);
            PooButtonClicked = true;
        } else {
            WaterButton.GetComponent<Button>().enabled = true;
            DrinkButton.GetComponent<Button>().enabled = true;
            PeeButton.GetComponent<Button>().enabled = true;
            WaterButton.GetComponent<Image>().color = ActiveColor;
            DrinkButton.GetComponent<Image>().color = ActiveColor;
            PeeButton.GetComponent<Image>().color = ActiveColor;
            DownShield.SetActive(false);
            PooButtonClicked = false;
        }
    }

    public void DataLeftButtonClick() {

    }

    public void DataRightButtonClick() {

    }

    public void TimeLeftButtonClick() {

    }

    public void TimeRightButtonClick() {

    }
}
