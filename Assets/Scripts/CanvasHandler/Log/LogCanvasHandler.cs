﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogCanvasHandler : MonoBehaviour
{
    public static LogCanvasHandler Instance;
    public Button TodayButton;
    public Image DateLeftButton;
    public Image DateRightButton;
    public Image TimeLeftButton;
    public Image TimeRightButton;
    public Button TimeLeftButton2;
    public Button TimeRightButton2;
    public GameObject WaterButton;
    public GameObject PeeButton;
    public GameObject PooButton;
    public GameObject DrinkButton;
    public GameObject FlowerCircle;

    public Button WaterButton2;
    public Button PeeButton2;
    public Button PooButton2;
    public Button DrinkButton2;

    public GameObject UpShield;
    public GameObject DownShield;
    public Color ActiveColor;
    public Color InactiveColor;
    public Text LogCanvasTimeText;
    public Text WaterCountText;
    public Text DrinkCountText;
    public Text PeeCountText;
    public Text PooCountText;

    public Text WaterCountText2;
    public Text DrinkCountText2;
    public Text PeeCountText2;
    public Text PooCountText2;

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

    public GameObject DrinkWindow;
    public Text DrinkWindowTitleText;
    public int DrinkWindow_Volume;
    public Scrollbar DrinkWindow_Volume_Input;
    public Text DrinkWindow_Volume_Input_Text;
    public int DrinkWindow_Type;
    public int DrinkWindow_Index;
    public bool DrinkWindow_ScrollAutoAdjust = false;
    public Image[] DrinkButtonIcons;

    public GameObject PooWindow;
    public Text PooWindowTitleText;
    public int PooWindow_Type;
    public int PooWindow_Index;
    public Image[] PooButtonIcons;

    public enum LOG_TYPE { NONE, WATER, DRINK, PEE, POOP, };
    public static string[] LogTypeList = { "None Log", "Water Log", "Drink Log", "Pee Log", "Poop Log" };

    public GameObject SelectDrinkUI;
    public GameObject DrinkModifyWindow;
    public Image[] DrinkModifyButtonIcons;
    public Scrollbar DrinkModifyWindow_Volume_Input;
    public Text DrinkModifyWindow_Volume_Input_Text;
    public Text DrinkModifyWindowTitleText;
    private int ModifyDrinkLogType = 0;
    private int ModifyDrinkLogVolume = 0;

    public GameObject SelectPooUI;
    public GameObject DeleteWaterUI;
    public GameObject DeletePeeUI;

    internal bool WaterButtonClicked = false;
    internal bool PooButtonClicked = false;
    internal bool PeeButtonClicked = false;
    internal bool DrinkButtonClicked = false;

    internal LOG_TYPE PressLogType = LOG_TYPE.NONE;
    internal int PressLogIndex = 0;

    private TimeHandler.DateTimeStamp targetDate = null;
    private List<Log> totalLog;
    private List<DataHandler.WaterLog> todayWaterLogs;
    private List<DataHandler.DrinkLog> todayDrinkLogs;
    private List<DataHandler.PeeLog>   todayPeeLogs;
    private List<DataHandler.PoopLog>  todayPoopLogs;
    private List<GameObject>           spawnObjects;
    internal DataHandler.GardenLog     TargetGardenLog;
    private int todayFirstHour;
    private int todayLastHour;
    public int currentFirstHour;
    public int currentLastHour;
    private float barSize = 110f;
    private float offset = 20f;
    private int lastLogIndex = -1;
    private LOG_TYPE lastLogType = LOG_TYPE.NONE;
    private bool makeMeal = false;

    public Sprite ButtonSpirte_Water;
    public Sprite ButtonSpirte_Drink;
    public Sprite ButtonSpirte_Poo;
    public Sprite ButtonSpirte_Pee;
    public Sprite ButtonSpirte_Water_Cancel;
    public Sprite ButtonSpirte_Drink_Cancel;
    public Sprite ButtonSpirte_Poo_Cancel;
    public Sprite ButtonSpirte_Pee_Cancel;

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
        if(totalLog != null)
            totalLog.Clear();
        DrawOff();
    }


    public void BLE_Redraw() {
        StartCoroutine(FetchAllData2());
    }

    public void DrawOff() {
        try {
            for (int i = 0; i < spawnObjects.Count; i ++) {
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
        } catch (System.Exception e) { e.ToString(); }
        try {
            WaterCountText2.text = "0";
            DrinkCountText2.text = "0";
            PeeCountText2.text = "0";
            PooCountText2.text = "0";
        } catch(System.Exception e) { e.ToString(); }
    }

    public void OnEnable() {
        StartCoroutine(UserDataCheck());
        TimeHandler.GetCurrentTime();
        WriteTimeStamp(TimeHandler.LogCanvasTime);

        if (TotalManager.instance != null &&
            TotalManager.instance.TargetDateString != null && TotalManager.instance.TargetDateString != "") {
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
        yield return 0;
        StartCoroutine(LogDataLoadChecks());
    }

    public IEnumerator LogDataLoadChecks() {
        TargetGardenLog = null;
        if (targetDate != null) {
            for (int i = 0; i < DataHandler.Garden_logs.GardenLogs.Length; i++) {
                if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                    new TimeHandler.DateTimeStamp(DataHandler.Garden_logs.GardenLogs[i].timestamp), targetDate) == 0) {
                    TargetGardenLog = DataHandler.Garden_logs.GardenLogs[i];
                    break;
                }
            }
        } else {
            TimeHandler.GetCurrentTime();
            for (int i = 0; i < DataHandler.Garden_logs.GardenLogs.Length; i++) {
                if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                    new TimeHandler.DateTimeStamp(DataHandler.Garden_logs.GardenLogs[i].timestamp), TimeHandler.LogCanvasTime) == 0) {
                    TargetGardenLog = DataHandler.Garden_logs.GardenLogs[i];
                    break;
                }
            }
            targetDate = TimeHandler.LogCanvasTime;
        }
        if (TargetGardenLog == null) {
            DataHandler.GardenLog newGarden = new DataHandler.GardenLog();
            newGarden.id = DataHandler.User_id;
            newGarden.timestamp = (new TimeHandler.DateTimeStamp(targetDate)).ToString();
            newGarden.flower = 0;
            newGarden.log_water = 0; newGarden.log_poop = 0; newGarden.log_pee = 0;
            newGarden.item_0 = 0; newGarden.item_1 = 0; newGarden.item_2 = 0; newGarden.item_3 = 0; newGarden.item_4 = 0;
            DataHandler.User_isGardenDataCreated = false;

            DataHandler.GardenLog[] array =
                            new DataHandler.GardenLog[DataHandler.Garden_logs.GardenLogs.Length + 1];
            DataHandler.CreateGardenIndex.Enqueue(array.Length - 1);
            for (int i = 0; i < DataHandler.Garden_logs.GardenLogs.Length; i++)
                array[i] = DataHandler.Garden_logs.GardenLogs[i];
            array[array.Length - 1] = newGarden;
            DataHandler.Garden_logs.GardenLogs = array;

            StartCoroutine(DataHandler.CreateGardenlogs(newGarden));
            StartCoroutine(writeGardenLogId(newGarden));
            StartCoroutine(FetchGardenLogId());
            TargetGardenLog = newGarden;
        }

        bool ThereAreTodos = false;
        if (DataHandler.Garden_logs != null)
            foreach (DataHandler.GardenLog log in DataHandler.Garden_logs.GardenLogs) {
                if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(new TimeHandler.DateTimeStamp(log.timestamp), targetDate) == 0) {
                    if (( log.log_water > 0 && log.flower < 10 ) || ( log.log_pee > 0 && log.item_0 == 0 ) || ( log.log_poop > 0 && log.item_1 == 0 )) {
                        ThereAreTodos = true;
                        break;
                    }
                }
            }
        FlowerCircle.SetActive(ThereAreTodos);

        foreach (DataHandler.WaterLog log in DataHandler.Water_logs.WaterLogs)
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

    IEnumerator FetchGardenLogId() {
        while (!DataHandler.User_isGardenDataCreated)
            yield return 0;
        DataHandler.User_isGardenDataCreated = false;
        TargetGardenLog.log_id = int.Parse(DataHandler.tempText);
        Debug.Log("Garden Log id : " + TargetGardenLog.log_id);
    }

    public void DrawLogs() {
        if(targetDate != null) {
            TimeHandler.LogCanvasTime = targetDate;
            targetDate = null;
        }

        if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.LogCanvasTime,
            new TimeHandler.DateTimeStamp(DataHandler.User_creation_date)) <= 0)
             DateLeftButton.transform.parent.GetComponent<Button>().interactable = false;
        else DateLeftButton.transform.parent.GetComponent<Button>().interactable = true;
        if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.LogCanvasTime,
                        TimeHandler.CurrentTime) >= 0)
             { DateRightButton.transform.parent.GetComponent<Button>().interactable = false; TodayButton.interactable = (false); }
        else { DateRightButton.transform.parent.GetComponent<Button>().interactable = true; TodayButton.interactable = (true); }

        currentFirstHour = 7;
        currentLastHour = currentFirstHour + 12;
        moveTimeZone(currentFirstHour);
        WriteTimeStamp(TimeHandler.LogCanvasTime);
        bool isThereLog = false;
        DrinkTempList = new List<KeyValuePair<DataHandler.DrinkLog, int>>();
        int count_water = 0, count_drink = 0, count_pee = 0, count_poo = 0;
        bool MongMongPoo_Flag = true;
        for (int i = 0; i < wrongPooCheck.Length; i++)
            wrongPooCheck[i] = false;
        foreach (Log log in totalLog) {
            if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                TimeHandler.LogCanvasTime, log.Time) == 0) {
                if (!isThereLog) {
                    isThereLog = true;
                    currentFirstHour = log.Time.Hours;
                    if (currentFirstHour >= 13) currentFirstHour = 12;
                    currentLastHour = currentFirstHour + 12;
                    if (currentFirstHour >= 12) {
                        TimeRightButton.transform.parent.GetComponent<Button>().interactable = false;
                        TimeRightButton2.interactable = false;
                    } else {
                        TimeRightButton.transform.parent.GetComponent<Button>().interactable = true;
                        TimeRightButton2.interactable = true;
                    }
                    if (currentFirstHour <= 0) {
                        TimeLeftButton.transform.parent.GetComponent<Button>().interactable = false;
                        TimeLeftButton2.interactable = false;
                    } else {
                        TimeLeftButton.transform.parent.GetComponent<Button>().interactable = true;
                        TimeLeftButton2.interactable = true;
                    }
                    moveTimeZone(currentFirstHour);
                }
                switch (log.LogType) {
                    case LOG_TYPE.WATER: CreateWaterLog(log.WaterLog, log.Time.Hours); count_water++; break;
                    case LOG_TYPE.DRINK: DrinkTempList.Add(new KeyValuePair<DataHandler.DrinkLog, int>(log.DrinkLog, log.Time.Hours)); count_drink++; break;
                    case LOG_TYPE.PEE:   CreatePeeLog(log.PeeLog, log.Time.Hours); count_pee++; break;
                    case LOG_TYPE.POOP:  CreatePoopLog(log.PoopLog, log.Time.Hours); count_poo++;
                                         if (log.PoopLog.type == 8) { MongMongPoo_Flag = false; } break;
                }
            }
        }
        for (int i = 0; i < wrongPooCheck.Length; i++) {
            if (DownSlot[i].GetComponent<SlotHandler>().PooTop == null) continue;

            DownSlot[i].GetComponent<SlotHandler>().
                PooTop.gameObject.GetComponent<Image>().sprite =
                    ( wrongPooCheck[i] ) ? wrongPoo : normalPoo;
        }

        if(!MongMongPoo_Flag) {
            isLogMongMong_Poo = true;
            MongMongHand.SetActive(true);
            Circle.SetActive(true);
        } else if (MongMongPoo_Flag) {
            isLogMongMong_Poo = false;
            MongMongHand.SetActive(false);
            Circle.SetActive(false);
        }

        CreateDrinkLog();
        bool ThereAreTodos = false;
        if (DataHandler.Garden_logs != null)
            foreach (DataHandler.GardenLog log in DataHandler.Garden_logs.GardenLogs) {
                if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(new TimeHandler.DateTimeStamp(log.timestamp), TimeHandler.LogCanvasTime) == 0) {
                    if (( log.log_water > 0 && log.flower < 10 ) || ( log.log_pee > 0 && log.item_0 == 0 ) || ( log.log_poop > 0 && log.item_1 == 0 )) {
                        ThereAreTodos = true;
                        break;
                    }
                }
            }
        FlowerCircle.SetActive(ThereAreTodos);
        StartCoroutine(ProtocolHandler.Instance.ReadGardenLogsRoutine());
        if (currentFirstHour <= 0) {
            TimeLeftButton.transform.parent.GetComponent<Button>().interactable = false;
            TimeLeftButton2.interactable = false;
        } else {
            TimeLeftButton.transform.parent.GetComponent<Button>().interactable = true;
            TimeLeftButton2.interactable = true;
        }
        TimeRightButton.transform.parent.GetComponent<Button>().interactable = true;
        TimeRightButton2.interactable = true;

        if (currentFirstHour >= 12) {
            TimeRightButton.transform.parent.GetComponent<Button>().interactable = false;
            TimeRightButton2.interactable = false;
        } else {
            TimeRightButton.transform.parent.GetComponent<Button>().interactable = true;
            TimeRightButton2.interactable = true;
        }
        TimeLeftButton.transform.parent.GetComponent<Button>().interactable = true;
        TimeLeftButton2.interactable = true;

        WaterCountText.text = count_water.ToString();
        DrinkCountText.text = count_drink.ToString();
        PeeCountText.text = count_pee.ToString();
        PooCountText.text = count_poo.ToString();

        WaterCountText2.text = count_water.ToString();
        DrinkCountText2.text = count_drink.ToString();
        PeeCountText2.text = count_pee.ToString();
        PooCountText2.text = count_poo.ToString();
    }

    private List<KeyValuePair<DataHandler.DrinkLog, int>> DrinkTempList;

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

    public void CreateDrinkLog() {
        List<int> maxInt = new List<int>();
        for (int k = 0; k < UpSlot.Length; k++)
            maxInt.Add(0);


        for(int i = 0; i < DrinkTempList.Count; i++) {
            int hour = DrinkTempList[i].Value;
            DataHandler.DrinkLog log = DrinkTempList[i].Key;
            if (hour < currentFirstHour || hour >= currentLastHour) continue;
            int index = hour - currentFirstHour;
            maxInt[index] = ( maxInt[index] >= 4 ) ? 4 : maxInt[index]+1;
        }

        for(int i = 0; i < DrinkTempList.Count; i++) {
            int hour = DrinkTempList[i].Value;
            DataHandler.DrinkLog log = DrinkTempList[i].Key;

            if (hour < currentFirstHour || hour >= currentLastHour) continue;
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
                    new Vector2(0f, -100f - 15f * ( maxInt[index] - slot.DrinkCount));
            }
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

    internal bool isLogMongMong_Poo = false;
    private bool[] wrongPooCheck = { false, false, false, false, false, false,
                                     false, false, false, false, false, false };

    public void CreatePoopLog(DataHandler.PoopLog log, int hour) {
        if (hour < currentFirstHour || hour >= currentLastHour) return;
        int index = hour - currentFirstHour;

        SlotHandler slot = DownSlot[index].GetComponent<SlotHandler>();
        if (( slot.PooCount >= 4 && !slot.isMeal ) || ( slot.PooCount >= 1 && slot.isMeal )) {
            slot.PooCount++;
            slot.PooTop.Number.text = slot.PooCount.ToString();
        } else {
            if (slot.PooTop != null) {
                slot.PooTop.Number.text = " ";
            }
            GameObject newLog = Instantiate(PoopLogPrefab, DownSlot[index]);
            if(log.type ==8)
                wrongPooCheck[index] = true;
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
            DrinkButton2.interactable = false;
            PeeButton2.interactable = false;
            PooButton2.interactable = false;
            WaterButton.GetComponent<Image>().sprite = ButtonSpirte_Water_Cancel;
            DrinkButton.GetComponent<Image>().sprite = ButtonSpirte_Drink;
            PeeButton.GetComponent<Image>().sprite = ButtonSpirte_Pee;
            PooButton.GetComponent<Image>().sprite = ButtonSpirte_Poo;
            WaterButton2.GetComponent<Image>().sprite = ButtonSpirte_Water_Cancel;
            DrinkButton2.GetComponent<Image>().sprite = ButtonSpirte_Drink;
            PeeButton2.GetComponent<Image>().sprite = ButtonSpirte_Pee;
            PooButton2.GetComponent<Image>().sprite = ButtonSpirte_Poo;
            WaterButtonClicked = true;
            UpShield.SetActive(true);
            LogBlocker.Instance.BlockOn(true);
        } else {
            ResetButtonImage();
            WaterButtonClicked = false;
        }
    }

    public void DrinkPlusButton() {
        if (!DrinkButtonClicked) {
            WaterButton.GetComponent<Button>().enabled = false;
            PeeButton.GetComponent<Button>().enabled = false;
            PooButton.GetComponent<Button>().enabled = false;
            WaterButton.GetComponent<Image>().sprite = ButtonSpirte_Water;
            DrinkButton.GetComponent<Image>().sprite = ButtonSpirte_Drink_Cancel;
            PeeButton.GetComponent<Image>().sprite = ButtonSpirte_Pee;
            PooButton.GetComponent<Image>().sprite = ButtonSpirte_Poo;
            WaterButton2.GetComponent<Image>().sprite = ButtonSpirte_Water;
            DrinkButton2.GetComponent<Image>().sprite = ButtonSpirte_Drink_Cancel;
            PeeButton2.GetComponent<Image>().sprite = ButtonSpirte_Pee;
            PooButton2.GetComponent<Image>().sprite = ButtonSpirte_Poo;
            WaterButton2.interactable = false;
            PeeButton2.interactable = false;
            PooButton2.interactable = false;
            DrinkButtonClicked = true;
            UpShield.SetActive(true);
            LogBlocker.Instance.BlockOn(true);
        } else {
            ResetButtonImage();
            DrinkButtonClicked = false;
        }
    }

    public void PeePlusButton() {
        if (!PeeButtonClicked) {
            WaterButton.GetComponent<Button>().enabled = false;
            DrinkButton.GetComponent<Button>().enabled = false;
            PooButton.GetComponent<Button>().enabled = false;
            WaterButton.GetComponent<Image>().sprite = ButtonSpirte_Water;
            DrinkButton.GetComponent<Image>().sprite = ButtonSpirte_Drink;
            PeeButton.GetComponent<Image>().sprite = ButtonSpirte_Pee_Cancel;
            PooButton.GetComponent<Image>().sprite = ButtonSpirte_Poo;
            WaterButton2.GetComponent<Image>().sprite = ButtonSpirte_Water;
            DrinkButton2.GetComponent<Image>().sprite = ButtonSpirte_Drink;
            PeeButton2.GetComponent<Image>().sprite = ButtonSpirte_Pee_Cancel;
            PooButton2.GetComponent<Image>().sprite = ButtonSpirte_Poo;
            WaterButton2.interactable = false;
            DrinkButton2.interactable = false;
            PooButton2.interactable = false;
            PeeButtonClicked = true;
            DownShield.SetActive(true);
            LogBlocker.Instance.BlockOn(false);
        } else {
            ResetButtonImage();
            PeeButtonClicked = false;
        }
    }

    public void PooPlusButton() {
        if (!PooButtonClicked) {
            WaterButton.GetComponent<Button>().enabled = false;
            DrinkButton.GetComponent<Button>().enabled = false;
            PeeButton.GetComponent<Button>().enabled = false;
            WaterButton.GetComponent<Image>().sprite = ButtonSpirte_Water;
            DrinkButton.GetComponent<Image>().sprite = ButtonSpirte_Drink;
            PeeButton.GetComponent<Image>().sprite = ButtonSpirte_Pee;
            PooButton.GetComponent<Image>().sprite = ButtonSpirte_Poo_Cancel;
            WaterButton2.GetComponent<Image>().sprite = ButtonSpirte_Water;
            DrinkButton2.GetComponent<Image>().sprite = ButtonSpirte_Drink;
            PeeButton2.GetComponent<Image>().sprite = ButtonSpirte_Pee;
            PooButton2.GetComponent<Image>().sprite = ButtonSpirte_Poo_Cancel;
            WaterButton2.interactable = false;
            DrinkButton2.interactable = false;
            PeeButton2.interactable = false;
            PooButtonClicked = true;
            DownShield.SetActive(true);
            LogBlocker.Instance.BlockOn(false);
        } else {
            ResetButtonImage();
            PooButtonClicked = false;
        }
    }

    internal bool isModifying = false;
    internal bool isModifyingWater = false;
    internal bool isModifyingDrink = false;
    internal bool isModifyingPoo = false;
    internal bool isModifyingPee = false;
    internal int modifyingTargetId = 0;


    public void CancelTimeLineModify() {
        DownShield.SetActive(false);
        UpShield.SetActive(false);
        LogBlocker.Instance.BlockOff();
        TimeLeftButton2.gameObject.SetActive(false);
        TimeRightButton2.gameObject.SetActive(false);
        
        ResetButtonImage();
    }

    public void TimeLineModifyInit(bool isUp) {
        if(isUp) { UpShield.SetActive(true); }
        else     { DownShield.SetActive(true); }

        LogBlocker.Instance.BlockOn(isUp);
        TimeLeftButton2.gameObject.SetActive(true);
        TimeRightButton2.gameObject.SetActive(true);
        isModifying = true;
    }

    public void DataLeftButtonClick() {
        if (DataHandler.User_creation_date == null) return;
        DrawOff();
        TimeHandler.LogCanvasTime = TimeHandler.LogCanvasTime - 1;
        if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.LogCanvasTime,
            new TimeHandler.DateTimeStamp(DataHandler.User_creation_date)) <= 0)
            DateLeftButton.transform.parent.GetComponent<Button>().interactable = false;
        DateRightButton.transform.parent.GetComponent<Button>().interactable = true;
        TodayButton.interactable = ( true );
        targetDate = TimeHandler.LogCanvasTime;

        TargetGardenLog = null;
        for (int i = 0; i < DataHandler.Garden_logs.GardenLogs.Length; i++) {
            if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                new TimeHandler.DateTimeStamp(DataHandler.Garden_logs.GardenLogs[i].timestamp), targetDate) == 0) {
                TargetGardenLog = DataHandler.Garden_logs.GardenLogs[i];
                break;
            }
        }
        
        if (TargetGardenLog == null) {
            DataHandler.GardenLog newGarden = new DataHandler.GardenLog();
            newGarden.id = DataHandler.User_id;
            newGarden.timestamp = ( new TimeHandler.DateTimeStamp(targetDate) ).ToString();
            newGarden.flower = 0;
            newGarden.log_water = 0; newGarden.log_poop = 0; newGarden.log_pee = 0;
            newGarden.item_0 = 0; newGarden.item_1 = 0; newGarden.item_2 = 0; newGarden.item_3 = 0; newGarden.item_4 = 0;
            DataHandler.User_isGardenDataCreated = false;

            DataHandler.GardenLog[] array =
                            new DataHandler.GardenLog[DataHandler.Garden_logs.GardenLogs.Length + 1];
            DataHandler.CreateGardenIndex.Enqueue(array.Length - 1);
            for (int i = 0; i < DataHandler.Garden_logs.GardenLogs.Length; i++)
                array[i] = DataHandler.Garden_logs.GardenLogs[i];
            array[array.Length - 1] = newGarden;
            DataHandler.Garden_logs.GardenLogs = array;

            StartCoroutine(DataHandler.CreateGardenlogs(newGarden));
            StartCoroutine(writeGardenLogId(newGarden));
            StartCoroutine(FetchGardenLogId());
            TargetGardenLog = newGarden;
        }

        bool ThereAreTodos = false;
        if (DataHandler.Garden_logs != null)
            foreach (DataHandler.GardenLog log in DataHandler.Garden_logs.GardenLogs) {
                if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(new TimeHandler.DateTimeStamp(log.timestamp), TimeHandler.LogCanvasTime) == 0) {
                    if (( log.log_water > 0 && log.flower < 10 ) || ( log.log_pee > 0 && log.item_0 == 0 ) || ( log.log_poop > 0 && log.item_1 == 0 )) {
                        ThereAreTodos = true;
                        break;
                    }
                }
            }

        FlowerCircle.SetActive(ThereAreTodos);

        DrawLogs();
    }

    public void DataRightButtonClick() {
        DrawOff();
        TimeHandler.LogCanvasTime = TimeHandler.LogCanvasTime + 1;
        if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.LogCanvasTime,
                TimeHandler.CurrentTime) >= 0) { 
            DateRightButton.transform.parent.GetComponent<Button>().interactable = false;
            TodayButton.interactable = ( false );
        }
        DateLeftButton.transform.parent.GetComponent<Button>().interactable = true;
        targetDate = TimeHandler.LogCanvasTime;

        TargetGardenLog = null;
        for (int i = 0; i < DataHandler.Garden_logs.GardenLogs.Length; i++) {
            if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                new TimeHandler.DateTimeStamp(DataHandler.Garden_logs.GardenLogs[i].timestamp), targetDate) == 0) {
                TargetGardenLog = DataHandler.Garden_logs.GardenLogs[i];
                break;
            }
        }

        if (TargetGardenLog == null) {
            DataHandler.GardenLog newGarden = new DataHandler.GardenLog();
            newGarden.id = DataHandler.User_id;
            newGarden.timestamp = ( new TimeHandler.DateTimeStamp(targetDate) ).ToString();
            newGarden.flower = 0;
            newGarden.log_water = 0; newGarden.log_poop = 0; newGarden.log_pee = 0;
            newGarden.item_0 = 0; newGarden.item_1 = 0; newGarden.item_2 = 0; newGarden.item_3 = 0; newGarden.item_4 = 0;
            DataHandler.User_isGardenDataCreated = false;

            DataHandler.GardenLog[] array =
                            new DataHandler.GardenLog[DataHandler.Garden_logs.GardenLogs.Length + 1];
            DataHandler.CreateGardenIndex.Enqueue(array.Length - 1);
            for (int i = 0; i < DataHandler.Garden_logs.GardenLogs.Length; i++)
                array[i] = DataHandler.Garden_logs.GardenLogs[i];
            array[array.Length - 1] = newGarden;
            DataHandler.Garden_logs.GardenLogs = array;

            StartCoroutine(DataHandler.CreateGardenlogs(newGarden));
            StartCoroutine(writeGardenLogId(newGarden));
            StartCoroutine(FetchGardenLogId());
            TargetGardenLog = newGarden;
        }
        bool ThereAreTodos = false;
        if (DataHandler.Garden_logs != null)
            foreach (DataHandler.GardenLog log in DataHandler.Garden_logs.GardenLogs) {
                if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(new TimeHandler.DateTimeStamp(log.timestamp), TimeHandler.LogCanvasTime) == 0) {
                    if (( log.log_water > 0 && log.flower < 10 ) || ( log.log_pee > 0 && log.item_0 == 0 ) || ( log.log_poop > 0 && log.item_1 == 0 )) {
                        ThereAreTodos = true;
                        break;
                    }
                }
            }

        FlowerCircle.SetActive(ThereAreTodos);
        DrawLogs();
    }

    public void TodayButtonClick() {
        DrawOff();
        TimeHandler.GetCurrentTime();
        bool ThereAreTodos = false;
        if (DataHandler.Garden_logs != null)
            foreach (DataHandler.GardenLog log in DataHandler.Garden_logs.GardenLogs) {
                if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(new TimeHandler.DateTimeStamp(log.timestamp), TimeHandler.LogCanvasTime) == 0) {
                    if (( log.log_water > 0 && log.flower < 10 ) || ( log.log_pee > 0 && log.item_0 == 0 ) || ( log.log_poop > 0 && log.item_1 == 0 )) {
                        ThereAreTodos = true;
                        break;
                    }
                }
            }

        FlowerCircle.SetActive(ThereAreTodos);
        DrawLogs();
    }

    public void TimeLeftButtonClick() {
        if (currentFirstHour <= 0) return;
        DrawOff();
        currentFirstHour--;
        currentLastHour--;
        if (currentFirstHour <= 0) {
            TimeLeftButton.transform.parent.GetComponent<Button>().interactable = false;
            TimeLeftButton2.interactable = false;
        } else {
            TimeLeftButton.transform.parent.GetComponent<Button>().interactable = true;
            TimeLeftButton2.interactable = true;
        }
        TimeRightButton.transform.parent.GetComponent<Button>().interactable = true;
        TimeRightButton2.interactable = true;
        moveTimeZone(currentFirstHour);
        WriteTimeStamp(TimeHandler.LogCanvasTime);
        DrinkTempList = new List<KeyValuePair<DataHandler.DrinkLog, int>>();
        int count_water = 0, count_drink = 0, count_pee = 0, count_poo = 0;
        for (int i = 0; i < wrongPooCheck.Length; i++)
            wrongPooCheck[i] = false;
        foreach (Log log in totalLog) {
            if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                TimeHandler.LogCanvasTime, log.Time) == 0) {
                switch (log.LogType) {
                    case LOG_TYPE.WATER: CreateWaterLog(log.WaterLog, log.Time.Hours); count_water++; break;
                    case LOG_TYPE.DRINK:
                    DrinkTempList.Add(new KeyValuePair<DataHandler.DrinkLog, int>(log.DrinkLog, log.Time.Hours));
                    count_drink++; break;
                    case LOG_TYPE.PEE: CreatePeeLog(log.PeeLog, log.Time.Hours); count_pee++; break;
                    case LOG_TYPE.POOP: CreatePoopLog(log.PoopLog, log.Time.Hours); count_poo++; break;
                }
            }
        }
        for (int i = 0; i < wrongPooCheck.Length; i++) {
            if (DownSlot[i].GetComponent<SlotHandler>().PooTop == null) continue;

            DownSlot[i].GetComponent<SlotHandler>().
                PooTop.gameObject.GetComponent<Image>().sprite =
                    ( wrongPooCheck[i] ) ? wrongPoo : normalPoo;
        }
        CreateDrinkLog();
        WaterCountText.text = count_water.ToString();
        DrinkCountText.text = count_drink.ToString();
        PeeCountText.text = count_pee.ToString();
        PooCountText.text = count_poo.ToString();

        WaterCountText2.text = count_water.ToString();
        DrinkCountText2.text = count_drink.ToString();
        PeeCountText2.text = count_pee.ToString();
        PooCountText2.text = count_poo.ToString();
    }

    public void TimeRightButtonClick() {
        if (currentFirstHour >= 12) return;
        DrawOff();
        currentFirstHour++;
        currentLastHour++;
        if (currentFirstHour >= 12) {
            TimeRightButton.transform.parent.GetComponent<Button>().interactable = false;
            TimeRightButton2.interactable = false;
        } else {
            TimeRightButton.transform.parent.GetComponent<Button>().interactable = true;
            TimeRightButton2.interactable = true;
        }
        TimeLeftButton.transform.parent.GetComponent<Button>().interactable = true;
        TimeLeftButton2.interactable = true;
        moveTimeZone(currentFirstHour);
        WriteTimeStamp(TimeHandler.LogCanvasTime);
        DrinkTempList = new List<KeyValuePair<DataHandler.DrinkLog, int>>();
        int count_water = 0, count_drink = 0, count_pee = 0, count_poo = 0;
        for (int i = 0; i < wrongPooCheck.Length; i++)
            wrongPooCheck[i] = false;
        foreach (Log log in totalLog) {
            if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                TimeHandler.LogCanvasTime, log.Time) == 0) {
                switch (log.LogType) {
                    case LOG_TYPE.WATER: CreateWaterLog(log.WaterLog, log.Time.Hours); count_water++; break;
                    case LOG_TYPE.DRINK: DrinkTempList.Add(new KeyValuePair<DataHandler.DrinkLog, int>(log.DrinkLog, log.Time.Hours));
                                         count_drink++; break;
                    case LOG_TYPE.PEE: CreatePeeLog(log.PeeLog, log.Time.Hours); count_pee++; break;
                    case LOG_TYPE.POOP: CreatePoopLog(log.PoopLog, log.Time.Hours); count_poo++; break;
                }
            }
        }
        for (int i = 0; i < wrongPooCheck.Length; i++) {
            if (DownSlot[i].GetComponent<SlotHandler>().PooTop == null) continue;

            DownSlot[i].GetComponent<SlotHandler>().
                PooTop.gameObject.GetComponent<Image>().sprite =
                    ( wrongPooCheck[i] ) ? wrongPoo : normalPoo;
        }
        CreateDrinkLog();
        WaterCountText.text = count_water.ToString();
        DrinkCountText.text = count_drink.ToString();
        PeeCountText.text = count_pee.ToString();
        PooCountText.text = count_poo.ToString();

        WaterCountText2.text = count_water.ToString();
        DrinkCountText2.text = count_drink.ToString();
        PeeCountText2.text = count_pee.ToString();
        PooCountText2.text = count_poo.ToString();
    }

    public void TimeBarClick_Add(int index) {
        if(!isModifying) {
            if      (WaterButtonClicked) TimeBarClick_AddWater(index);
            else if (DrinkButtonClicked) TimeBarClick_AddDrink(index);
            else if (PeeButtonClicked)   TimeBarClick_AddPee(index);
            else if (PooButtonClicked)   TimeBarClick_AddPoo(index);
        } else {
            if (isModifyingWater) TimeBarClick_ModifyWater(index);
            else if (isModifyingDrink) TimeBarClick_ModifyDrink(index);
            else if (isModifyingPee)   TimeBarClick_ModifyPee(index);
            else if (isModifyingPoo)   TimeBarClick_ModifyPoo(index);
        }
    }

    public IEnumerator ModifyingTimeLineUpdate() {
        Fetching();
        yield return 0;
    }

    public void TimeBarClick_ModifyWater(int index) {
        DataHandler.WaterLog log = null;
        foreach (DataHandler.WaterLog targetLog in DataHandler.Water_logs.WaterLogs)
            if(modifyingTargetId == targetLog.log_id) {
                log = targetLog;
                break;
            }
        if (log == null) return;

        string timestamp = log.timestamp.Split(' ')[0] + " " +
                           ( currentFirstHour + index ) + ":00:00";
        log.timestamp = timestamp;
        if (log.auto == 0) log.auto = 2;
        else if (log.auto == 1) log.auto = 3;

        DataHandler.User_isWaterDataUpdated = false;
        StartCoroutine(DataHandler.UpdateWaterLogs(log));
        StartCoroutine(ModifyingTimeLineUpdate());
        CancelTimeLineModify();
    }

    public void TimeBarClick_ModifyDrink(int index) {
        DataHandler.DrinkLog log = null;
        foreach (DataHandler.DrinkLog targetLog in DataHandler.Drink_logs.DrinkLogs)
            if (modifyingTargetId == targetLog.log_id) {
                log = targetLog;
                break;
            }
        if (log == null) return;

        string timestamp = log.timestamp.Split(' ')[0] + " " +
                           ( currentFirstHour + index ) + ":00:00";
        log.timestamp = timestamp;
        if (log.auto == 0) log.auto = 2;
        else if (log.auto == 1) log.auto = 3;
        DataHandler.User_isDrinkDataUpdated = false;
        StartCoroutine(DataHandler.UpdateDrinkLogs(log));
        StartCoroutine(ModifyingTimeLineUpdate());
        CancelTimeLineModify();
    }

    public void TimeBarClick_ModifyPee(int index) {
        DataHandler.PeeLog log = null;
        foreach (DataHandler.PeeLog targetLog in DataHandler.Pee_logs.PeeLogs)
            if (modifyingTargetId == targetLog.log_id) {
                log = targetLog;
                break;
            }
        if (log == null) return;

        string timestamp = log.timestamp.Split(' ')[0] + " " +
                           ( currentFirstHour + index ) + ":00:00";
        log.timestamp = timestamp;
        if (log.auto == 0) log.auto = 2;
        else if (log.auto == 1) log.auto = 3;
        DataHandler.User_isPeeDataUpdated = false;
        StartCoroutine(DataHandler.UpdatePeeLogs(log));
        StartCoroutine(ModifyingTimeLineUpdate());
        CancelTimeLineModify();
    }

    public void TimeBarClick_ModifyPoo(int index) {
        DataHandler.PoopLog log = null;
        foreach (DataHandler.PoopLog targetLog in DataHandler.Poop_logs.PoopLogs)
            if (modifyingTargetId == targetLog.log_id) {
                log = targetLog;
                break;
            }
        if (log == null) return;

        string timestamp = log.timestamp.Split(' ')[0] + " " +
                           ( currentFirstHour + index ) + ":00:00";
        log.timestamp = timestamp;
        if (log.auto == 0) log.auto = 2;
        else if (log.auto == 1) log.auto = 3;
        DataHandler.User_isPooDataUpdated = false;
        StartCoroutine(DataHandler.UpdatePoopLogs(log));
        StartCoroutine(ModifyingTimeLineUpdate());
        CancelTimeLineModify();
    }


    IEnumerator writeGardenLogId(DataHandler.GardenLog log) {
        while (!DataHandler.User_isGardenDataCreated)
            yield return 0;
        DataHandler.User_isGardenDataCreated = false;
        log.log_id = DataHandler.User_isGardenDataCreatedId;

    }

    public void TimeBarClick_AddWater(int index) {
        string timestamp = TargetGardenLog.timestamp.Split(' ')[0] + " " +
                           ( currentFirstHour + index ) + ":00:00";
        DataHandler.WaterLog newLog = new DataHandler.WaterLog();
        newLog.auto = 0;
        newLog.timestamp = timestamp;
        newLog.id = DataHandler.User_id;
        newLog.type = 0;

        DataHandler.WaterLog[] array = new DataHandler.WaterLog[DataHandler.Water_logs.WaterLogs.Length + 1];
        for (int i = 0; i < DataHandler.Water_logs.WaterLogs.Length; i++)
            array[i] = DataHandler.Water_logs.WaterLogs[i];
        array[array.Length - 1] = newLog;
        DataHandler.Water_logs.WaterLogs = array;
        DataHandler.CreateWaterIndex.Enqueue(array.Length - 1);
        StartCoroutine(DataHandler.CreateWaterlogs(newLog));
        //StartCoroutine(writeWaterLogId(newLog));
        TargetGardenLog.log_water++;
        StartCoroutine(DataHandler.UpdateGardenLogs(TargetGardenLog));
        StartCoroutine(Redraw_Water());
    }

    public void TimeBarClick_AddDrink(int index) {
        LogBlocker.Instance.BlockOnAll();
        DrinkWindow.SetActive(true);
        DrinkWindow_Volume = 50;
        DrinkWindow_Volume_Input.value = 0;
        DrinkWindow_Type = 0;
        DrinkWindow_Index = index;
        for (int i = 1; i < 4; i++)
            DrinkButtonIcons[i].color = new Color(1f, 1f, 1f, 1f);
        DrinkButtonIcons[0].color = new Color(0.55f, 0.55f, 0.55f, 1f);
        int showTime = currentFirstHour + index;
        string str = "새벽";
        if (showTime >= 6 && showTime <= 11) str = "아침";
        else if (showTime >= 12 && showTime <= 14) str = "점심";
        else if (showTime >= 15 && showTime <= 17) str = "낮";
        else if (showTime >= 18 && showTime <= 20) str = "저녁";
        else if (showTime >= 21) str = "밤";
        if (showTime >= 12)
            showTime -= 12;
        showTime = ( showTime == 0 ) ? 12 : showTime;
        str += " " + showTime + "시에 추가할 음료의 종류와 양을 골라주세요.";
        DrinkWindowTitleText.text = str;
    }

    public void DrinkWidowClose() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.BACK);
        DrinkPlusButton();
        DrinkWindow.SetActive(false);
    }

    public void DrinkWindowOkay() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.POPED);
        DrinkPlusButton();
        TimeBarClick_AddDrink(DrinkWindow_Index, DrinkWindow_Type, DrinkWindow_Volume);
        DrinkWindow.SetActive(false);

    }

    public void DrinkWindowSelectTypeButton(int index) {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.CLICKED3);
        DrinkWindow_Type = index;
        for(int  i = 1; i < 4; i ++) {
            if (index == i) DrinkButtonIcons[i].color = new Color(1f, 1f, 1f, 1f);
            else DrinkButtonIcons[i].color = new Color(1f, 1f, 1f, 0.3f);
        }
        if (index == 0) DrinkButtonIcons[0].color = new Color(0.55f, 0.55f, 0.55f, 1f);
        else DrinkButtonIcons[0].color = new Color(0.55f, 0.55f, 0.55f, 0.3f);
    }

    public void DrinkWindowSelectValue() {
        //DrinkWindow_Volume = int.Parse(DrinkWindow_Volume_Input.captionText.text.Split(' ')[0]);
        if (DrinkWindow_ScrollAutoAdjust) {
            float splitAmount = ( (float)1 / (float)12 );
            float scrollValue = DrinkWindow_Volume_Input.value;
            bool flag = true;
            if(scrollValue < splitAmount * 3) {
                DrinkWindow_Volume = 50; flag = false;
                if(scrollValue < splitAmount * 2)
                    DrinkWindow_Volume_Input.value = splitAmount * 2;
            }
            for(int i = 3; i < 8 && flag; i ++) {
                if(scrollValue < splitAmount * ((i * 2) - 1)) {
                    DrinkWindow_Volume = (i-1) * 50; flag = false;
                }
            }
            DrinkWindow_Volume_Input_Text.text = DrinkWindow_Volume + " ml";
        }
    }

    public void TimeBarClick_AddDrink(int index,int type, int volume) {
        string timestamp = TargetGardenLog.timestamp.Split(' ')[0] + " " +
                           ( currentFirstHour + index ) + ":00:00";
        DataHandler.DrinkLog newLog = new DataHandler.DrinkLog();
        newLog.auto = 0;
        newLog.timestamp = timestamp;
        newLog.id = DataHandler.User_id;
        newLog.type = type;
        newLog.volume = volume;
        DataHandler.DrinkLog[] array = new DataHandler.DrinkLog[DataHandler.Drink_logs.DrinkLogs.Length + 1];
        for (int i = 0; i < DataHandler.Drink_logs.DrinkLogs.Length; i++)
            array[i] = DataHandler.Drink_logs.DrinkLogs[i];
        array[array.Length - 1] = newLog;
        DataHandler.Drink_logs.DrinkLogs = array;
        DataHandler.CreateDrinkIndex.Enqueue(array.Length - 1);
        StartCoroutine(DataHandler.CreateDrinklogs(newLog));
        //StartCoroutine(writeDrinkLogId(newLog));
        StartCoroutine(Redraw_Drink());
    }

    public void TimeBarClick_AddPee(int index) {
        string timestamp = TargetGardenLog.timestamp.Split(' ')[0] + " " +
                           ( currentFirstHour + index ) + ":00:00";
        DataHandler.PeeLog newLog = new DataHandler.PeeLog();
        newLog.auto = 0;
        newLog.timestamp = timestamp;
        newLog.id = DataHandler.User_id;

        DataHandler.PeeLog[] array = new DataHandler.PeeLog[DataHandler.Pee_logs.PeeLogs.Length + 1];
        for (int i = 0; i < DataHandler.Pee_logs.PeeLogs.Length; i++)
            array[i] = DataHandler.Pee_logs.PeeLogs[i];
        array[array.Length - 1] = newLog;
        DataHandler.Pee_logs.PeeLogs = array;
        DataHandler.CreatePeeIndex.Enqueue(array.Length - 1);
        StartCoroutine(DataHandler.CreatePeelogs(newLog));
        //StartCoroutine(writePeeLogId(newLog));

        TargetGardenLog.log_pee++;
        StartCoroutine(DataHandler.UpdateGardenLogs(TargetGardenLog));
        StartCoroutine(Redraw_Pee());
    }

    public void TimeBarClick_AddPoo(int index) {
        LogBlocker.Instance.BlockOnAll();
        PooWindow.SetActive(true);
        PooWindow_Type = 0;
        PooWindow_Index = index;
        for (int i = 1; i < 8; i++)
            PooButtonIcons[i].color = new Color(1f,1f,1f,1f);
        PooButtonIcons[0].color = new Color(0.8f,0.8f,0.8f,1f);
        int showTime = currentFirstHour + index;
        string str = "새벽";
        if (showTime >= 6 && showTime <= 11) str = "아침";
        else if (showTime >= 12 && showTime <= 14) str = "점심";
        else if (showTime >= 15 && showTime <= 17) str = "낮";
        else if (showTime >= 18 && showTime <= 20) str = "저녁";
        else if (showTime >= 21) str = "밤";
        if (showTime >= 12)
            showTime -= 12;
        showTime = ( showTime == 0 ) ? 12 : showTime;
        str += " " + showTime + "시에 추가할 똥의 모양을 골라주세요.";
        PooWindowTitleText.text = str;
    }


    public void PooWidowClose() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.BACK);
        PooPlusButton();
        PooWindow.SetActive(false);
    }

    public void PooWindowOkay() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.POPED);
        PooPlusButton();
        TimeBarClick_AddPoo(PooWindow_Index, PooWindow_Type);
        PooWindow.SetActive(false);
    }

    public void PooWindowSelectTypeButton(int index) {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.CLICKED3);
        PooWindow_Type = index;
        for(int  i = 1; i < 8; i ++) {
            if (i == index) PooButtonIcons[i].color = new Color(1f, 1f, 1f, 1f);
            else PooButtonIcons[i].color = new Color(1f, 1f, 1f, 0.3f);
        }
        if (index == 0) PooButtonIcons[0].color = new Color(0.8f, 0.8f, 0.8f, 1f);
        else PooButtonIcons[0].color = new Color(0.8f, 0.8f, 0.8f, 0.3f);
    }

    public void TimeBarClick_AddPoo(int index,int type) {
        string timestamp = TargetGardenLog.timestamp.Split(' ')[0] + " " +
                           ( currentFirstHour + index ) + ":00:00";
        DataHandler.PoopLog newLog = new DataHandler.PoopLog();
        newLog.auto = 0;
        newLog.timestamp = timestamp;
        newLog.id = DataHandler.User_id;
        newLog.type = type;
        DataHandler.PoopLog[] array = new DataHandler.PoopLog[DataHandler.Poop_logs.PoopLogs.Length + 1];
        for (int i = 0; i < DataHandler.Poop_logs.PoopLogs.Length; i++)
            array[i] = DataHandler.Poop_logs.PoopLogs[i];
        array[array.Length - 1] = newLog;
        DataHandler.Poop_logs.PoopLogs = array;
        DataHandler.CreatePooIndex.Enqueue(array.Length - 1);
        StartCoroutine(DataHandler.CreatePooplogs(newLog));
        //StartCoroutine(writePooLogId(newLog));
        TargetGardenLog.log_poop++;
        StartCoroutine(DataHandler.UpdateGardenLogs(TargetGardenLog));
        StartCoroutine(Redraw_Poo());
    }

    IEnumerator Redraw_Water() {
        //while (!DataHandler.User_isWaterDataCreated) { yield return 0; }
        //DataHandler.User_isWaterDataCreated = false;
        yield return 0;
        Fetching();
    }

    IEnumerator Redraw_Drink() {
        //while (!DataHandler.User_isDrinkDataCreated) { yield return 0; }
        //DataHandler.User_isDrinkDataCreated = false;
        yield return 0;
        Fetching();
    }

    IEnumerator Redraw_Pee() {
        //while (!DataHandler.User_isPeeDataCreated) { yield return 0; }
        //DataHandler.User_isPeeDataCreated = false;
        yield return 0;
        Fetching();
    }

    IEnumerator Redraw_Poo() {
        //while (!DataHandler.User_isPooDataCreated) { yield return 0; }
        //DataHandler.User_isPooDataCreated = false;
        yield return 0;
        Fetching();
    }

    public void Fetching() {
        StartCoroutine(FetchAllData2());
    }

    IEnumerator FetchAllData() {
        yield return 0;

        TargetGardenLog = null;
        if (targetDate != null) {
            for (int i = 0; i < DataHandler.Garden_logs.GardenLogs.Length; i++) {
                if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                    new TimeHandler.DateTimeStamp(DataHandler.Garden_logs.GardenLogs[i].timestamp), targetDate) == 0) {
                    TargetGardenLog = DataHandler.Garden_logs.GardenLogs[i];
                    break;
                }
            }
        } else {
            for (int i = 0; i < DataHandler.Garden_logs.GardenLogs.Length; i++) {
                if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                    new TimeHandler.DateTimeStamp(DataHandler.Garden_logs.GardenLogs[i].timestamp), TimeHandler.LogCanvasTime) == 0) {
                    TargetGardenLog = DataHandler.Garden_logs.GardenLogs[i];
                    break;
                }
            }
        }
        if (TargetGardenLog == null) {
            DataHandler.GardenLog newGarden = new DataHandler.GardenLog();
            newGarden.id = DataHandler.User_id;
            newGarden.timestamp = ( new TimeHandler.DateTimeStamp(targetDate) ).ToString();
            newGarden.flower = 0;
            newGarden.log_water = 0; newGarden.log_poop = 0; newGarden.log_pee = 0;
            newGarden.item_0 = 0; newGarden.item_1 = 0; newGarden.item_2 = 0; newGarden.item_3 = 0; newGarden.item_4 = 0;

            DataHandler.GardenLog[] array =
                            new DataHandler.GardenLog[DataHandler.Garden_logs.GardenLogs.Length + 1];
            DataHandler.CreateGardenIndex.Enqueue(array.Length - 1);
            for (int i = 0; i < DataHandler.Garden_logs.GardenLogs.Length; i++)
                array[i] = DataHandler.Garden_logs.GardenLogs[i];
            array[array.Length - 1] = newGarden;
            DataHandler.Garden_logs.GardenLogs = array;

            StartCoroutine(DataHandler.CreateGardenlogs(newGarden));
            StartCoroutine(writeGardenLogId(newGarden));
            TargetGardenLog = newGarden;
        }

        totalLog.Clear();
        foreach (DataHandler.WaterLog log in DataHandler.Water_logs.WaterLogs)
            totalLog.Add(new Log(log));
        foreach (DataHandler.DrinkLog log in DataHandler.Drink_logs.DrinkLogs)
            totalLog.Add(new Log(log));
        foreach (DataHandler.PeeLog log in DataHandler.Pee_logs.PeeLogs)
            totalLog.Add(new Log(log));
        foreach (DataHandler.PoopLog log in DataHandler.Poop_logs.PoopLogs)
            totalLog.Add(new Log(log));
        yield return 0;

        totalLog.Sort(delegate (Log a, Log b) {
            return TimeHandler.DateTimeStamp.CmpDateTimeStampDetail(a.Time, b.Time);
        });

        DrawOff();
        yield return 0;
        DrinkTempList = new List<KeyValuePair<DataHandler.DrinkLog, int>>();
        int count_water = 0, count_drink = 0, count_pee = 0, count_poo = 0;
        bool MongMongPoo_Flag = true;

        for (int i = 0; i < wrongPooCheck.Length; i++)
            wrongPooCheck[i] = false;
        foreach (Log log in totalLog) {
            if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                TimeHandler.LogCanvasTime, log.Time) == 0) {
                switch (log.LogType) {
                    case LOG_TYPE.WATER: CreateWaterLog(log.WaterLog, log.Time.Hours); count_water++; break;
                    case LOG_TYPE.DRINK:
                    DrinkTempList.Add(new KeyValuePair<DataHandler.DrinkLog, int>(log.DrinkLog, log.Time.Hours));
                    count_drink++; break;
                    case LOG_TYPE.PEE: CreatePeeLog(log.PeeLog, log.Time.Hours); count_pee++; break;
                    case LOG_TYPE.POOP: CreatePoopLog(log.PoopLog, log.Time.Hours); count_poo++;
                                        if (log.PoopLog.type == 8) { MongMongPoo_Flag = false; } break;
                }
            }
        }
        for (int i = 0; i < wrongPooCheck.Length; i++) {
            if (DownSlot[i].GetComponent<SlotHandler>().PooTop == null) continue;

            DownSlot[i].GetComponent<SlotHandler>().
                PooTop.gameObject.GetComponent<Image>().sprite =
                    ( wrongPooCheck[i] ) ? wrongPoo : normalPoo;
        }
        if (!MongMongPoo_Flag) {
            isLogMongMong_Poo = true;
            MongMongHand.SetActive(true);
            Circle.SetActive(true);
        } else if (MongMongPoo_Flag) {
            isLogMongMong_Poo = false;
            MongMongHand.SetActive(false);
            Circle.SetActive(false);
        }
        CreateDrinkLog();
        bool ThereAreTodos = false;
        if (DataHandler.Garden_logs != null)
            foreach (DataHandler.GardenLog log in DataHandler.Garden_logs.GardenLogs) {
                if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(new TimeHandler.DateTimeStamp(log.timestamp), TimeHandler.LogCanvasTime) == 0) {
                    if (( log.log_water > 0 && log.flower < 10 ) || ( log.log_pee > 0 && log.item_0 == 0 ) || ( log.log_poop > 0 && log.item_1 == 0 )) {
                        ThereAreTodos = true;
                        break;
                    }
                }
            }
        FlowerCircle.SetActive(ThereAreTodos);
        StartCoroutine(ProtocolHandler.Instance.ReadGardenLogsRoutine());
        WaterCountText.text = count_water.ToString();
        DrinkCountText.text = count_drink.ToString();
        PeeCountText.text = count_pee.ToString();
        PooCountText.text = count_poo.ToString();

        WaterCountText2.text = count_water.ToString();
        DrinkCountText2.text = count_drink.ToString();
        PeeCountText2.text = count_pee.ToString();
        PooCountText2.text = count_poo.ToString();

        ResetButtonImage();
    }

    public void ResetButtonImage() {
        WaterButton.GetComponent<Button>().enabled = true;
        DrinkButton.GetComponent<Button>().enabled = true;
        PeeButton.GetComponent<Button>().enabled = true;
        PooButton.GetComponent<Button>().enabled = true;
        WaterButton2.interactable = true;
        DrinkButton2.interactable = true;
        PeeButton2.interactable = true;
        PooButton2.interactable = true;
        WaterButton.GetComponent<Image>().sprite = ButtonSpirte_Water;
        DrinkButton.GetComponent<Image>().sprite = ButtonSpirte_Drink;
        PeeButton.GetComponent<Image>().sprite = ButtonSpirte_Pee;
        PooButton.GetComponent<Image>().sprite = ButtonSpirte_Poo;
        WaterButton2.GetComponent<Image>().sprite = ButtonSpirte_Water;
        DrinkButton2.GetComponent<Image>().sprite = ButtonSpirte_Drink;
        PeeButton2.GetComponent<Image>().sprite = ButtonSpirte_Pee;
        PooButton2.GetComponent<Image>().sprite = ButtonSpirte_Poo;
        WaterButtonClicked = false;
        DrinkButtonClicked = false;
        PeeButtonClicked = false;
        PooButtonClicked = false;
        isModifying = false;
        isModifyingWater = false;
        isModifyingDrink = false;
        isModifyingPoo = false;
        isModifyingPee = false;
        DownShield.SetActive(false);
        UpShield.SetActive(false);
        LogBlocker.Instance.BlockOff();
    }

    IEnumerator FetchAllData2() {
        yield return 0;
        TargetGardenLog = null;
        if (targetDate != null) {
            for (int i = 0; i < DataHandler.Garden_logs.GardenLogs.Length; i++) {
                if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                    new TimeHandler.DateTimeStamp(DataHandler.Garden_logs.GardenLogs[i].timestamp), targetDate) == 0) {
                    TargetGardenLog = DataHandler.Garden_logs.GardenLogs[i];
                    break;
                }
            }
        } else {
            for (int i = 0; i < DataHandler.Garden_logs.GardenLogs.Length; i++) {
                if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                    new TimeHandler.DateTimeStamp(DataHandler.Garden_logs.GardenLogs[i].timestamp), TimeHandler.LogCanvasTime) == 0) {
                    TargetGardenLog = DataHandler.Garden_logs.GardenLogs[i];
                    break;
                }
            }
        }
        if (TargetGardenLog == null) {
            DataHandler.GardenLog newGarden = new DataHandler.GardenLog();
            newGarden.id = DataHandler.User_id;
            //if (targetDate == null) { TimeHandler.GetCurrentTime(); targetDate = TimeHandler.CurrentTime; }   
            newGarden.timestamp = ( new TimeHandler.DateTimeStamp(targetDate) ).ToString();
            newGarden.flower = 0;
            newGarden.log_water = 0; newGarden.log_poop = 0; newGarden.log_pee = 0;
            newGarden.item_0 = 0; newGarden.item_1 = 0; newGarden.item_2 = 0; newGarden.item_3 = 0; newGarden.item_4 = 0;

            DataHandler.GardenLog[] array =
                            new DataHandler.GardenLog[DataHandler.Garden_logs.GardenLogs.Length + 1];
            DataHandler.CreateGardenIndex.Enqueue(array.Length - 1);
            for (int i = 0; i < DataHandler.Garden_logs.GardenLogs.Length; i++)
                array[i] = DataHandler.Garden_logs.GardenLogs[i];
            array[array.Length - 1] = newGarden;
            DataHandler.Garden_logs.GardenLogs = array;

            StartCoroutine(DataHandler.CreateGardenlogs(newGarden));
            StartCoroutine(writeGardenLogId(newGarden));
            TargetGardenLog = newGarden;
        }

        totalLog.Clear();
        foreach (DataHandler.WaterLog log in DataHandler.Water_logs.WaterLogs)
            totalLog.Add(new Log(log));
        foreach (DataHandler.DrinkLog log in DataHandler.Drink_logs.DrinkLogs)
            totalLog.Add(new Log(log));
        foreach (DataHandler.PeeLog log in DataHandler.Pee_logs.PeeLogs)
            totalLog.Add(new Log(log));
        foreach (DataHandler.PoopLog log in DataHandler.Poop_logs.PoopLogs)
            totalLog.Add(new Log(log));
        yield return 0;

        totalLog.Sort(delegate (Log a, Log b) {
            return TimeHandler.DateTimeStamp.CmpDateTimeStampDetail(a.Time, b.Time);
        });

        DrawOff();
        yield return 0;
        DrinkTempList = new List<KeyValuePair<DataHandler.DrinkLog, int>>();
        int count_water = 0, count_drink = 0, count_pee = 0, count_poo = 0;
        bool MongMongPoo_Flag = true;
        for (int i = 0; i < wrongPooCheck.Length; i++)
            wrongPooCheck[i] = false;
        foreach (Log log in totalLog) {
            if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                TimeHandler.LogCanvasTime, log.Time) == 0) {
                switch (log.LogType) {
                    case LOG_TYPE.WATER: CreateWaterLog(log.WaterLog, log.Time.Hours); count_water++; break;
                    case LOG_TYPE.DRINK:
                    DrinkTempList.Add(new KeyValuePair<DataHandler.DrinkLog, int>(log.DrinkLog, log.Time.Hours));
                    count_drink++; break;
                    case LOG_TYPE.PEE: CreatePeeLog(log.PeeLog, log.Time.Hours); count_pee++; break;
                    case LOG_TYPE.POOP: CreatePoopLog(log.PoopLog, log.Time.Hours); count_poo++;
                                        if (log.PoopLog.type == 8) { MongMongPoo_Flag = false; } break;
                }
            }
        }
        for (int i = 0; i < wrongPooCheck.Length; i++) {
            if (DownSlot[i].GetComponent<SlotHandler>().PooTop == null) continue;

            DownSlot[i].GetComponent<SlotHandler>().
                PooTop.gameObject.GetComponent<Image>().sprite =
                    ( wrongPooCheck[i] ) ? wrongPoo : normalPoo;
        }
        if (!MongMongPoo_Flag) {
            isLogMongMong_Poo = true;
            MongMongHand.SetActive(true);
            Circle.SetActive(true);
        } else if (MongMongPoo_Flag) {
            isLogMongMong_Poo = false;
            MongMongHand.SetActive(false);
            Circle.SetActive(false);
        }
        CreateDrinkLog();
        bool ThereAreTodos = false;
        if (DataHandler.Garden_logs != null)
            foreach (DataHandler.GardenLog log in DataHandler.Garden_logs.GardenLogs) {
                if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(new TimeHandler.DateTimeStamp(log.timestamp), TimeHandler.LogCanvasTime) == 0) {
                    if (( log.log_water > 0 && log.flower < 10 ) || ( log.log_pee > 0 && log.item_0 == 0 ) || ( log.log_poop > 0 && log.item_1 == 0 )) {
                        ThereAreTodos = true;
                        break;
                    }
                }
            }
        FlowerCircle.SetActive(ThereAreTodos);
        StartCoroutine(ProtocolHandler.Instance.ReadGardenLogsRoutine());
        WaterCountText.text = count_water.ToString();
        DrinkCountText.text = count_drink.ToString();
        PeeCountText.text = count_pee.ToString();
        PooCountText.text = count_poo.ToString();

        WaterCountText2.text = count_water.ToString();
        DrinkCountText2.text = count_drink.ToString();
        PeeCountText2.text = count_pee.ToString();
        PooCountText2.text = count_poo.ToString();

        ResetButtonImage();
    }

    public List<int> autoLogId;
    public List<int> noneAutoLogId;

    public void DeleteLog() {
        if (PressLogType == LOG_TYPE.NONE) return;
        
        int hour = currentFirstHour + PressLogIndex;
        autoLogId = new List<int>();
        noneAutoLogId = new List<int>();
        if(PressLogType == LOG_TYPE.WATER) {
            foreach(DataHandler.WaterLog log in DataHandler.Water_logs.WaterLogs) {
                TimeHandler.DateTimeStamp target = new TimeHandler.DateTimeStamp(log.timestamp);
                if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.LogCanvasTime, target) != 0) continue;
                if ( target.Hours == hour)
                    if (log.auto == 1) autoLogId.Add(log.log_id);
                    else noneAutoLogId.Add(log.log_id);
            }
        } else if(PressLogType == LOG_TYPE.DRINK) {
            foreach (DataHandler.DrinkLog log in DataHandler.Drink_logs.DrinkLogs) {
                TimeHandler.DateTimeStamp target = new TimeHandler.DateTimeStamp(log.timestamp);
                if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.LogCanvasTime, target) != 0) continue;
                if (target.Hours == hour)
                    if (log.auto == 1) autoLogId.Add(log.log_id);
                    else noneAutoLogId.Add(log.log_id);
            }
        } else if (PressLogType == LOG_TYPE.PEE) {
            foreach (DataHandler.PeeLog log in DataHandler.Pee_logs.PeeLogs) {
                TimeHandler.DateTimeStamp target = new TimeHandler.DateTimeStamp(log.timestamp);
                if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.LogCanvasTime, target) != 0) continue;
                if (target.Hours == hour)
                    if (log.auto == 1) autoLogId.Add(log.log_id);
                    else noneAutoLogId.Add(log.log_id);
            }
        } else {
            foreach (DataHandler.PoopLog log in DataHandler.Poop_logs.PoopLogs) {
                TimeHandler.DateTimeStamp target = new TimeHandler.DateTimeStamp(log.timestamp);
                if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.LogCanvasTime, target) != 0) continue;
                if (target.Hours == hour)
                    if (log.auto == 1) autoLogId.Add(log.log_id);
                    else noneAutoLogId.Add(log.log_id);
            }
        }
        
        if(PressLogType == LOG_TYPE.DRINK) {
            SelectDrinkUI.GetComponent<SelectDrinkHandler>().isDelete = true;
            SelectDrinkUI.GetComponent<SelectDrinkHandler>().titleText.text
                = "제거할 기록을 골라주세요";
            SelectDrinkUI.GetComponent<SelectDrinkHandler>().auto = autoLogId;
            SelectDrinkUI.GetComponent<SelectDrinkHandler>().noneauto = noneAutoLogId;
            SelectDrinkUI.SetActive(true);
        } else if (PressLogType == LOG_TYPE.POOP) {
            SelectPooUI.GetComponent<SelectPooHandler>().isDelete = true;
            SelectPooUI.GetComponent<SelectPooHandler>().titleText.text
                = "제거할 기록을 골라주세요";
            SelectPooUI.GetComponent<SelectPooHandler>().auto = autoLogId;
            SelectPooUI.GetComponent<SelectPooHandler>().noneauto = noneAutoLogId;
            SelectPooUI.SetActive(true);
        } else if (PressLogType == LOG_TYPE.WATER) {
            DeleteWaterUI.SetActive(true);
            DeleteWaterUI.GetComponent<DeleteWaterHandler>().auto = autoLogId;
            DeleteWaterUI.GetComponent<DeleteWaterHandler>().noneauto = noneAutoLogId;
            DeleteWaterUI.GetComponent<DeleteWaterHandler>().Init();
        } else if (PressLogType == LOG_TYPE.PEE) {
            DeletePeeUI.SetActive(true);
            DeletePeeUI.GetComponent<DeletePeeHandler>().auto = autoLogId;
            DeletePeeUI.GetComponent<DeletePeeHandler>().noneauto = noneAutoLogId;
            DeletePeeUI.GetComponent<DeletePeeHandler>().Init();
        }
        LogBlocker.Instance.BlockOff();
    }

    IEnumerator DeleteUpdateCheck() {
        yield return 0;
        Fetching();
    }

    public void RealRemovePooLog() {
        if (TargetGardenLog.log_poop > 0) TargetGardenLog.log_poop--;
        else if (TargetGardenLog.item_1 > 0) TargetGardenLog.item_1--;
        StartCoroutine(DataHandler.UpdateGardenLogs(TargetGardenLog));
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.ERROR);
        LogBlocker.Instance.BlockOff();
        totalLog.Clear();
        int tempCurrent = currentFirstHour;
        Fetching();
        moveTimeZone(tempCurrent);
    }

    public void ModifyLog() {
        if (PressLogType == LOG_TYPE.NONE) return;

        isModifyingDrink = ( PressLogType == LOG_TYPE.DRINK );
        isModifyingWater = ( PressLogType == LOG_TYPE.WATER );
        isModifyingPee = ( PressLogType == LOG_TYPE.PEE );
        isModifyingPoo = ( PressLogType == LOG_TYPE.POOP );
        int hour = currentFirstHour + PressLogIndex;
        autoLogId = new List<int>();
        noneAutoLogId = new List<int>();
        if (PressLogType == LOG_TYPE.DRINK) {
            foreach (DataHandler.DrinkLog log in DataHandler.Drink_logs.DrinkLogs) {
                TimeHandler.DateTimeStamp target = new TimeHandler.DateTimeStamp(log.timestamp);
                if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.LogCanvasTime, target) != 0) continue;
                if (target.Hours == hour)
                    if (log.auto == 1) autoLogId.Add(log.log_id);
                    else noneAutoLogId.Add(log.log_id);
            }
        } else if (PressLogType == LOG_TYPE.POOP) {
            foreach (DataHandler.PoopLog log in DataHandler.Poop_logs.PoopLogs) {
                TimeHandler.DateTimeStamp target = new TimeHandler.DateTimeStamp(log.timestamp);
                if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.LogCanvasTime, target) != 0) continue;
                if (target.Hours == hour)
                    if (log.auto == 1) autoLogId.Add(log.log_id);
                    else noneAutoLogId.Add(log.log_id);
            }
        } else if (PressLogType == LOG_TYPE.WATER) {
            foreach (DataHandler.WaterLog log in DataHandler.Water_logs.WaterLogs) {
                TimeHandler.DateTimeStamp target = new TimeHandler.DateTimeStamp(log.timestamp);
                if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.LogCanvasTime, target) != 0) continue;
                if (target.Hours == hour)
                    if (log.auto == 1) autoLogId.Add(log.log_id);
                    else noneAutoLogId.Add(log.log_id);
            }
        } else if (PressLogType == LOG_TYPE.PEE) {
            foreach (DataHandler.PeeLog log in DataHandler.Pee_logs.PeeLogs) {
                TimeHandler.DateTimeStamp target = new TimeHandler.DateTimeStamp(log.timestamp);
                if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.LogCanvasTime, target) != 0) continue;
                if (target.Hours == hour)
                    if (log.auto == 1) autoLogId.Add(log.log_id);
                    else noneAutoLogId.Add(log.log_id);
            }
        }

        LogBlocker.Instance.DisableDetailShield();
        WaterButton.GetComponent<Button>().enabled = false;
        PeeButton.GetComponent<Button>().enabled = false;
        PooButton.GetComponent<Button>().enabled = false;
        DrinkButton.GetComponent<Button>().enabled = false;
        WaterButton2.interactable = false;
        PeeButton2.interactable = false;
        PooButton2.interactable = false;
        DrinkButton2.interactable = false;
        if (PressLogType == LOG_TYPE.DRINK || PressLogType == LOG_TYPE.POOP) {
            if (PressLogType == LOG_TYPE.DRINK) {
                SelectDrinkUI.GetComponent<SelectDrinkHandler>().isDelete = false;
                SelectDrinkUI.GetComponent<SelectDrinkHandler>().titleText.text
                    = "변경할 기록을 골라주세요";
                SelectDrinkUI.GetComponent<SelectDrinkHandler>().auto = autoLogId;
                SelectDrinkUI.GetComponent<SelectDrinkHandler>().noneauto = noneAutoLogId;
                SelectDrinkUI.SetActive(true);
                LogBlocker.Instance.BlockOff();
            } else {
                SelectPooUI.GetComponent<SelectPooHandler>().isDelete = false;
                SelectPooUI.GetComponent<SelectPooHandler>().titleText.text
                    = "변경할 기록을 골라주세요";
                SelectPooUI.GetComponent<SelectPooHandler>().auto = autoLogId;
                SelectPooUI.GetComponent<SelectPooHandler>().noneauto = noneAutoLogId;
                SelectPooUI.SetActive(true);
                LogBlocker.Instance.BlockOff();
            }
        } else {
            if (PressLogType == LOG_TYPE.WATER || PressLogType == LOG_TYPE.PEE) {
                if (noneAutoLogId.Count > 0)
                    modifyingTargetId = noneAutoLogId[0];
                else if (autoLogId.Count > 0)
                    modifyingTargetId = autoLogId[0];
                TimeLineModifyInit(LOG_TYPE.WATER == PressLogType);
            }
        }
    }

    public void InitDrinkModify(int type,int hour) {
        LogBlocker.Instance.BlockOff();
        DrinkModifyWindow.SetActive(true);
        ModifyDrinkLogType = ModifyDrinkLogHandler.Target.type;
        ModifyDrinkLogVolume = ModifyDrinkLogHandler.Target.volume;
        for (int i = 1; i < 4; i++) {
            if (type == i) DrinkModifyButtonIcons[i].color = new Color(1f, 1f, 1f, 1f);
            else DrinkModifyButtonIcons[i].color = new Color(1f, 1f, 1f, 0.3f);
        }
        if (type == 0) DrinkModifyButtonIcons[0].color = new Color(0.55f, 0.55f, 0.55f, 1f);
        else DrinkModifyButtonIcons[0].color = new Color(0.55f, 0.55f, 0.55f, 0.3f);
        int showTime = hour;
        string str = "변경할 새벽";
        if (showTime >= 6 && showTime <= 11) str = "변경할 아침";
        else if (showTime >= 12 && showTime <= 14) str = "변경할 점심";
        else if (showTime >= 15 && showTime <= 17) str = "변경할 낮";
        else if (showTime >= 18 && showTime <= 20) str = "변경할 저녁";
        else if (showTime >= 21) str = "변경할 밤";
        if (showTime >= 12)
            showTime -= 12;
        showTime = ( showTime == 0 ) ? 12 : showTime;
        str += " " + showTime + "시 음료의 종류와 양을 골라주세요.";
        DrinkModifyWindowTitleText.text = str;
    }

    public void DrinkModifyWidowClose() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.BACK);
        ResetButtonImage();
        DrinkModifyWindow.SetActive(false);
    }

    public void DrinkModifyWindowOkay() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.POPED);
        ModifyDrinkLogHandler.Target.volume = ModifyDrinkLogVolume;
        ModifyDrinkLogHandler.Target.type = ModifyDrinkLogType;
        StartCoroutine(DataHandler.UpdateDrinkLogs(ModifyDrinkLogHandler.Target));
        StartCoroutine(UpdateDrinkCheck());
    }

    public IEnumerator UpdateDrinkCheck() {
        yield return 0;
        DrinkModifyWindow.SetActive(false);
    }

    public void DrinkModifyWindowSelectTypeButton(int index) {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.CLICKED3);
        ModifyDrinkLogType = index;
        for (int i = 1; i < 4; i++) {
            if (index == i) DrinkModifyButtonIcons[i].color = new Color(1f, 1f, 1f, 1f);
            else DrinkModifyButtonIcons[i].color = new Color(1f, 1f, 1f, 0.3f);
        }
        if (index == 0) DrinkButtonIcons[0].color = new Color(0.55f, 0.55f, 0.55f, 1f);
        else DrinkModifyButtonIcons[0].color = new Color(0.55f, 0.55f, 0.55f, 0.3f);
    }

    public void DrinkModifyWindowSelectValue() {
        //DrinkWindow_Volume = int.Parse(DrinkWindow_Volume_Input.captionText.text.Split(' ')[0]);

            float splitAmount = ( (float)1 / (float)12 );
            float scrollValue = DrinkModifyWindow_Volume_Input.value;
            bool flag = true;
            if (scrollValue < splitAmount * 3) {
                ModifyDrinkLogVolume = 50; flag = false;
                if (scrollValue < splitAmount * 2)
                    DrinkModifyWindow_Volume_Input.value = splitAmount * 2;
            }
            for (int i = 3; i < 8 && flag; i++) {
                if (scrollValue < splitAmount * ( ( i * 2 ) - 1 )) {
                ModifyDrinkLogVolume = ( i - 1 ) * 50; flag = false;
                }
            }
            DrinkModifyWindow_Volume_Input_Text.text = ModifyDrinkLogVolume + " ml";
    }

    public GameObject MongMongHand;
    public GameObject MongMongUI;
    public GameObject Circle;
    public Sprite wrongPoo;
    public Sprite normalPoo;

    public void MongMongHandClick() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.CLICKED);
        MongMongUI.SetActive(true);
    }

    public void MongMongUIOkay() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.BACK);
        MongMongUI.SetActive(false);
    }
}
