using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogCanvasHandler : MonoBehaviour
{
    public static LogCanvasHandler Instance;
    public GameObject TodayButton;
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
    public Text WaterCountText;
    public Text DrinkCountText;
    public Text PeeCountText;
    public Text PooCountText;
    public Text DebugText;
    public Text DebugTextLabel;
    public GameObject WaterLogPrefab;
    public GameObject DrinkLogPrefab;
    public GameObject PeeLogPrefab;
    public GameObject PoopLogPrefab;
    public GameObject MealLogPrefab;
    public List<GameObject> MealObjectList;
    public RectTransform TimeZoneBar;
    public Transform[] UpSlot;
    public Transform[] DownSlot;

    public enum LOG_TYPE { NONE, WATER, DRINK, PEE, POOP, };
    public static string[] LogTypeList = { "None Log", "Water Log", "Drink Log", "Pee Log", "Poop Log" };

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
    public int currentFirstHour;
    public int currentLastHour;
    private float barSize = 110f;
    private float offset = 20f;
    private int lastLogIndex = -1;
    private LOG_TYPE lastLogType = LOG_TYPE.NONE;
    private bool makeMeal = false;
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
        MealObjectList = new List<GameObject>();
        DebugText.gameObject.SetActive(TotalManager.instance.isDebugMode);
        DebugTextLabel.gameObject.SetActive(TotalManager.instance.isDebugMode);
    }

    public void OnDisable() {
        totalLog.Clear();
        DrawOff();
    }

    public void DrawOff() {
        for(int i = 0; i < spawnObjects.Count; i ++) {
            GameObject temp = spawnObjects[i];
            spawnObjects[i] = null;
            Destroy(temp);
        }
        for(int i = 0; i < MealObjectList.Count; i++) {
            GameObject temp = MealObjectList[i];
            MealObjectList[i] = null;
            Destroy(temp);
        }
        spawnObjects.Clear();
        MealObjectList.Clear();
        for (int i = 0; i < 12; i++) {
            SlotHandler slot_up   = UpSlot[i].GetComponent<SlotHandler>();
            SlotHandler slot_down = DownSlot[i].GetComponent<SlotHandler>();
            slot_up.WaterCount = 0;
            slot_up.DrinkCount = 0;
            slot_up.WaterTop = null;
            slot_up.DrinkTop = null;
            slot_down.PeeCount = 0;
            slot_down.PooCount = 0;
            slot_down.PeeTop = null;
            slot_down.PooTop = null;
            slot_down.isPooNoType = false;
        }
        WaterCountText.text = "0";
        DrinkCountText.text = "0";
        PeeCountText.text = "0";
        PooCountText.text = "0";
    }

    public void OnEnable() {
        StartCoroutine(UserDataCheck());
        TimeHandler.GetCurrentTime();
        WriteTimeStamp(TimeHandler.LogCanvasTime);

        if (TotalManager.instance.TargetDateString != "") {
            targetDate = new TimeHandler.DateTimeStamp(TotalManager.instance.TargetDateString);
            TotalManager.instance.TargetDateString = "";
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

        DrawOff();
        yield return 0;
        DrawLogs();
    }

    public void DrawLogs() {
        if(targetDate != null) {
            TimeHandler.LogCanvasTime = targetDate;
            targetDate = null;
        }

        if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.LogCanvasTime,
            new TimeHandler.DateTimeStamp(DataHandler.User_creation_date)) <= 0)
             DateLeftButton.GetComponent<Button>().interactable = false;
        else DateLeftButton.GetComponent<Button>().interactable = true;
        if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.LogCanvasTime,
                        TimeHandler.CurrentTime) >= 0)
             { DateRightButton.GetComponent<Button>().interactable = false; TodayButton.SetActive(false); }
        else { DateRightButton.GetComponent<Button>().interactable = true; TodayButton.SetActive(true); }

        currentFirstHour = 7;
        currentLastHour = currentFirstHour + 12;
        moveTimeZone(currentFirstHour);
        WriteTimeStamp(TimeHandler.LogCanvasTime);
        bool isThereLog = false;
        int count_water = 0, count_drink = 0, count_pee = 0, count_poo = 0;
        foreach (Log log in totalLog) {
            if(TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                TimeHandler.LogCanvasTime,log.Time) == 0 ) {
                if (!isThereLog) {
                    isThereLog = true;
                    currentFirstHour = log.Time.Hours;
                    if (currentFirstHour >= 13) currentFirstHour = 12;
                    currentLastHour = currentFirstHour + 12;
                    if (currentFirstHour >= 12) TimeRightButton.GetComponent<Button>().interactable = false;
                    else TimeRightButton.GetComponent<Button>().interactable = true;
                    if (currentFirstHour <= 0) TimeLeftButton.GetComponent<Button>().interactable = false;
                    else TimeLeftButton.GetComponent<Button>().interactable = true;
                    moveTimeZone(currentFirstHour);
                }
                log.ToString();
                switch(log.LogType) {
                    case LOG_TYPE.WATER: CreateWaterLog(log.WaterLog,log.Time.Hours); count_water++; break;
                    case LOG_TYPE.DRINK: CreateDrinkLog(log.DrinkLog,log.Time.Hours); count_drink++; break;
                    case LOG_TYPE.PEE:   CreatePeeLog(log.PeeLog,log.Time.Hours);     count_pee++; break;
                    case LOG_TYPE.POOP:  CreatePoopLog(log.PoopLog, log.Time.Hours);  count_poo++; break;
                }
            }
        }
        WaterCountText.text = count_water.ToString();
        DrinkCountText.text = count_drink.ToString();
        PeeCountText.text = count_pee.ToString();
        PooCountText.text = count_poo.ToString();
    }

    public void moveTimeZone(int startHour) {
        TimeZoneBar.anchoredPosition = new Vector2(-startHour * (barSize + offset),0f);
        for (int i = 0; i < 12; i++) {
            SlotHandler slot_up = UpSlot[i].GetComponent<SlotHandler>();
            slot_up.isMeal = false;
        }
        for(int i = 0; i < MealObjectList.Count; i ++) {
            GameObject temp = MealObjectList[i];
            MealObjectList[i] = null;
            Destroy(temp);
        }
        MealObjectList.Clear();
        string strHead = TimeHandler.CurrentTime.ToDateString() + " ";
        int breakfast_index =
            new TimeHandler.DateTimeStamp(strHead + DataHandler.User_breakfast_time).Hours - startHour;
        int lunch_index =
            new TimeHandler.DateTimeStamp(strHead + DataHandler.User_lunch_time).Hours - startHour;
        int dinner_index =
            new TimeHandler.DateTimeStamp(strHead + DataHandler.User_dinner_time).Hours - startHour;
        if(makeMeal) {
            if(breakfast_index >= 0 && breakfast_index < 12) {
                GameObject breakfastObject = Instantiate(MealLogPrefab, UpSlot[breakfast_index]);
                UpSlot[breakfast_index].GetComponent<SlotHandler>().isMeal = true;
                breakfastObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -280f);
                MealObjectList.Add(breakfastObject);
            }

            if(lunch_index >= 0 && lunch_index < 12) {
                GameObject lunchObject = Instantiate(MealLogPrefab, UpSlot[lunch_index]);
                lunchObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -280f);
                UpSlot[lunch_index].GetComponent<SlotHandler>().isMeal = true;
                MealObjectList.Add(lunchObject);
            }

            if(dinner_index >= 0 && dinner_index < 12) {
                GameObject dinnerObject = Instantiate(MealLogPrefab, UpSlot[dinner_index]);
                UpSlot[dinner_index].GetComponent<SlotHandler>().isMeal = true;
                dinnerObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -280f);
                MealObjectList.Add(dinnerObject);
            }
        }
    }

    public void CreateWaterLog(DataHandler.WaterLog log, int hour) {
        if (hour < currentFirstHour || hour >= currentLastHour) return;
        int index = hour - currentFirstHour;

        SlotHandler slot = UpSlot[index].GetComponent<SlotHandler>();
        if((slot.WaterCount >= 4 && !slot.isMeal) || (slot.WaterCount >= 1 && slot.isMeal)) {
            slot.WaterCount++;
            slot.WaterTop.Number.text = slot.WaterCount.ToString();
        } else {
            if (slot.WaterTop != null) slot.WaterTop.Number.text = " ";
            GameObject newLog = Instantiate(WaterLogPrefab, UpSlot[index]);
            spawnObjects.Add(newLog);
            slot.WaterCount++;
            slot.WaterTop = newLog.GetComponent<LogSpriteHandler>();
            slot.WaterTop.log = new LogSpriteHandler.LogScript(
                log.timestamp,log.log_id,log.type,LogSpriteHandler.LOG.WATER    
            );
            slot.WaterTop.Number.text = slot.WaterCount.ToString();
            newLog.GetComponent<RectTransform>().anchoredPosition = ( !slot.isMeal ) ?
                (new Vector2(0f, -275f + 15f * ( slot.WaterCount - 1 ))) :
                (new Vector2(0f, -190f + 15f * ( slot.WaterCount - 1 )));
        }
    }

    public void CreateDrinkLog(DataHandler.DrinkLog log, int hour) {
        if (hour < currentFirstHour || hour >= currentLastHour) return;
        int index = hour - currentFirstHour;

        SlotHandler slot = UpSlot[index].GetComponent<SlotHandler>();
        if (( slot.DrinkCount >= 4 && !slot.isMeal ) || ( slot.DrinkCount >= 1 && slot.isMeal )) {
            slot.DrinkCount++;
            slot.DrinkTop.Number.text = slot.DrinkCount.ToString();
        } else {
            if (slot.DrinkTop != null) slot.DrinkTop.Number.text = " ";
            GameObject newLog = Instantiate(DrinkLogPrefab, UpSlot[index]);
            spawnObjects.Add(newLog);
            slot.DrinkCount++;
            slot.DrinkTop = newLog.GetComponent<LogSpriteHandler>();
            slot.DrinkTop.log = new LogSpriteHandler.LogScript(
                log.timestamp, log.log_id, log.type, LogSpriteHandler.LOG.DRINK
            );
            slot.DrinkTop.Number.text = slot.DrinkCount.ToString();
            newLog.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(0f, -100f - 10f * ( slot.DrinkCount - 1 ));
        }
    }

    public void CreatePeeLog(DataHandler.PeeLog log, int hour) {
        if (hour < currentFirstHour || hour >= currentLastHour) return;
        int index = hour - currentFirstHour;

        SlotHandler slot = DownSlot[index].GetComponent<SlotHandler>();
        if (( slot.PeeCount >= 4 && !slot.isMeal ) || ( slot.PeeCount >= 1 && slot.isMeal )) {
            slot.PeeCount++;
            slot.PeeTop.Number.text = slot.PeeCount.ToString();
        } else {
            if (slot.PeeTop != null) slot.PeeTop.Number.text = " ";
            GameObject newLog = Instantiate(PeeLogPrefab, DownSlot[index]);
            spawnObjects.Add(newLog);
            slot.PeeCount++;
            slot.PeeTop = newLog.GetComponent<LogSpriteHandler>();
            slot.PeeTop.log = new LogSpriteHandler.LogScript(
                log.timestamp, log.log_id, 0, LogSpriteHandler.LOG.PEE
            );
            slot.PeeTop.Number.text = slot.PeeCount.ToString();
            newLog.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(0f, 200f - 20f * ( slot.PeeCount - 1 ));
        }
    }

    public void CreatePoopLog(DataHandler.PoopLog log, int hour) {
        if (hour < currentFirstHour || hour >= currentLastHour) return;
        int index = hour - currentFirstHour;

        SlotHandler slot = DownSlot[index].GetComponent<SlotHandler>();
        if (( slot.PooCount >= 4 && !slot.isMeal ) || ( slot.PooCount >= 1 && slot.isMeal )) {
            slot.PooCount++;
            slot.PooTop.Number.text = slot.PooCount.ToString();
        } else {
            if (slot.PooTop != null) slot.PooTop.Number.text = " ";
            GameObject newLog = Instantiate(PoopLogPrefab, DownSlot[index]);
            spawnObjects.Add(newLog);
            slot.PooCount++;
            slot.PooTop = newLog.GetComponent<LogSpriteHandler>();
            slot.PooTop.log = new LogSpriteHandler.LogScript(
                log.timestamp, log.log_id, log.type, LogSpriteHandler.LOG.POO
            );
            slot.PooTop.Number.text = slot.PooCount.ToString();
            newLog.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(0f, -275f + 15f * ( slot.PooCount - 1 ));
        }
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
        if (DataHandler.User_creation_date == null) return;
        DrawOff();
        TimeHandler.LogCanvasTime = TimeHandler.LogCanvasTime - 1;
        if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.LogCanvasTime,
            new TimeHandler.DateTimeStamp(DataHandler.User_creation_date)) <= 0)
            DateLeftButton.GetComponent<Button>().interactable = false;
        DateRightButton.GetComponent<Button>().interactable = true;
        TodayButton.SetActive(true);
        DrawLogs();
    }

    public void DataRightButtonClick() {
        DrawOff();
        TimeHandler.LogCanvasTime = TimeHandler.LogCanvasTime + 1;
        if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.LogCanvasTime,
                TimeHandler.CurrentTime) >= 0) { 
            DateRightButton.GetComponent<Button>().interactable = false;
            TodayButton.SetActive(false);
        }
        DateLeftButton.GetComponent<Button>().interactable = true;
        DrawLogs();
    }

    public void TodayButtonClick() {
        DrawOff();
        TimeHandler.GetCurrentTime();
        Debug.Log("move to today");
        DrawLogs();
    }

    public void TimeLeftButtonClick() {
        if (currentFirstHour <= 0) return;
        DrawOff();
        currentFirstHour--;
        currentLastHour--;
        if (currentFirstHour <= 0) TimeLeftButton.GetComponent<Button>().interactable = false;
        else TimeLeftButton.GetComponent<Button>().interactable = true;
        TimeRightButton.GetComponent<Button>().interactable = true;
        moveTimeZone(currentFirstHour);
        WriteTimeStamp(TimeHandler.LogCanvasTime);
        bool isThereLog = false;
        int count_water = 0, count_drink = 0, count_pee = 0, count_poo = 0;
        foreach (Log log in totalLog) {
            if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                TimeHandler.LogCanvasTime, log.Time) == 0) {
                switch (log.LogType) {
                    case LOG_TYPE.WATER: CreateWaterLog(log.WaterLog, log.Time.Hours); count_water++; break;
                    case LOG_TYPE.DRINK: CreateDrinkLog(log.DrinkLog, log.Time.Hours); count_drink++; break;
                    case LOG_TYPE.PEE: CreatePeeLog(log.PeeLog, log.Time.Hours); count_pee++; break;
                    case LOG_TYPE.POOP: CreatePoopLog(log.PoopLog, log.Time.Hours); count_poo++; break;
                }
            }
        }
        WaterCountText.text = count_water.ToString();
        DrinkCountText.text = count_drink.ToString();
        PeeCountText.text = count_pee.ToString();
        PooCountText.text = count_poo.ToString();
    }

    public void TimeRightButtonClick() {
        if (currentFirstHour >= 12) return;
        DrawOff();
        currentFirstHour++;
        currentLastHour++;
        if (currentFirstHour >= 12) TimeRightButton.GetComponent<Button>().interactable = false;
        else TimeRightButton.GetComponent<Button>().interactable = true;
        TimeLeftButton.GetComponent<Button>().interactable = true;
        moveTimeZone(currentFirstHour);
        WriteTimeStamp(TimeHandler.LogCanvasTime);
        bool isThereLog = false;
        int count_water = 0, count_drink = 0, count_pee = 0, count_poo = 0;
        foreach (Log log in totalLog) {
            if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                TimeHandler.LogCanvasTime, log.Time) == 0) {
                switch (log.LogType) {
                    case LOG_TYPE.WATER: CreateWaterLog(log.WaterLog, log.Time.Hours); count_water++; break;
                    case LOG_TYPE.DRINK: CreateDrinkLog(log.DrinkLog, log.Time.Hours); count_drink++; break;
                    case LOG_TYPE.PEE: CreatePeeLog(log.PeeLog, log.Time.Hours); count_pee++; break;
                    case LOG_TYPE.POOP: CreatePoopLog(log.PoopLog, log.Time.Hours); count_poo++; break;
                }
            }
        }
        WaterCountText.text = count_water.ToString();
        DrinkCountText.text = count_drink.ToString();
        PeeCountText.text = count_pee.ToString();
        PooCountText.text = count_poo.ToString();
    }
}
