using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalendarHandler : MonoBehaviour {
    public static CalendarHandler Instance;

    public Image[] PeeGauges = new Image[28];
    public Image[] WaterGauges = new Image[28];
    public Image[] PoopGauges = new Image[28];
    public Text[] PeeGaugeTexts = new Text[28];
    public Text[] WaterGaugeTexts = new Text[28];
    public Sprite[] AfterPeeGauges = new Sprite[5];
    public Sprite[] AfterWaterGauges = new Sprite[10];
    public Sprite[] AfterPoopGauges = new Sprite[2];
    public int[] PeeBoundary = new int[4];
    public int[] WaterBoundary = new int[10];
    public Text[] LeftLabelDates = new Text[7];
    public Text[] RightLabelDates = new Text[7];
    public Text[] WeekTexts = new Text[4];
    public Sprite WaterEmptyGauges;
    public String[] WeekTextsList = { "1주", "2주", "3주", "4주", "5주", "6주", "7주", "8주" };

    public GameObject UpButton;
    public GameObject DownButton;
    public RectTransform TodayMark_Water;
    public RectTransform TodayMark_Pee;
    public Text DebugText;
    public Text DebugTextLabel;

    public SoundHandler.SFX WaterIconSFX;
    public SoundHandler.SFX PeeIconSFX;
    public SoundHandler.SFX PoopIconSFX;

    private Dictionary<string, int> waterDictionary;
    private Dictionary<string, int> peeDictionary;
    private Dictionary<string, int> pooDictionary;
    private int weekData = 0;
    private int[,] stairs = new int[10,4]{
        { 0,-1,-1,-1}, { 1, 7,-1,-1},
        { 2, 8,14,-1}, { 3, 9,15,21},
        { 4,10,16,22}, { 5,11,17,23},
        { 6,12,18,24}, {13,19,25,-1},
        {20,26,-1,-1}, {27,-1,-1,-1},
    };
    private int[,] stairs2 = new int[10, 4]{
        {-1,-1,-1, 6}, {-1,-1,13, 5},
        {-1,20,12, 4}, {27,19,11, 3},
        {26,18,10, 2}, {25,17, 9, 1},
        {24,16, 8, 0}, {23,15, 7,-1},
        {22,14,-1,-1}, {21,-1,-1,-1},
    };


    public void Awake() {
        Instance = this;
    }

    public void Start() {
        waterDictionary = new Dictionary<string, int>();
        peeDictionary = new Dictionary<string, int>();
        pooDictionary = new Dictionary<string, int>();
        DebugText.gameObject.SetActive(TotalManager.instance.isDebugMode);
        DebugTextLabel.gameObject.SetActive(TotalManager.instance.isDebugMode);
    }

    private void OnDisable() {
        waterDictionary.Clear();
        peeDictionary.Clear();
        pooDictionary.Clear();
        foreach (Image img in PeeGauges) img.sprite = AfterPeeGauges[0];
        foreach (Image img in PoopGauges) img.sprite = AfterPoopGauges[1];
        foreach (Image img in WaterGauges) img.sprite = WaterEmptyGauges;
        foreach (Text t in WaterGaugeTexts) t.text = "";
        foreach (Text t in PeeGaugeTexts) t.text = "";
        TodayMark_Pee.gameObject.SetActive(false);
        TodayMark_Water.gameObject.SetActive(false);
        isDataLoaded = false;
    }

    private void OnEnable() {
        TimeHandler.GetCurrentTime();
        StartCoroutine(CheckLoadAndVisualize_1());
    }
    private IEnumerator CheckLoadAndVisualize_1() {
        yield return 0;
        while (!DataHandler.User_isDataLoaded) {
            yield return 0;
        }
        DataHandler.User_isDataLoaded = false;
        StartCoroutine(DataHandler.ReadWaterLogs(DataHandler.User_id));
        StartCoroutine(DataHandler.ReadPeeLogs(DataHandler.User_id));
        StartCoroutine(DataHandler.ReadPoopLogs(DataHandler.User_id));
        StartCoroutine(CheckLoadAndVisualize_2());
    }

    private IEnumerator CheckLoadAndVisualize_2() {
        isDataLoaded = true;
        yield return 0;
        while(!DataHandler.User_isWaterDataLoaded ||
              !DataHandler.User_isPeeDataLoaded ||
              !DataHandler.User_isPooDataLoaded) { yield return 0; }
        DataHandler.User_isWaterDataLoaded = false;
        DataHandler.User_isPeeDataLoaded = false;
        DataHandler.User_isPooDataLoaded = false;

        foreach(DataHandler.WaterLog log in DataHandler.Water_logs.WaterLogs) {
            string timestamp = log.timestamp.Split(' ')[0];
            try {
                int num = waterDictionary[timestamp];
                waterDictionary[timestamp] = num + 1;
            } catch(Exception e) {
                e.ToString();
                waterDictionary.Add(timestamp, 1);
            }
        }

        foreach (DataHandler.PeeLog log in DataHandler.Pee_logs.PeeLogs) {
            string timestamp = log.timestamp.Split(' ')[0];
            try {
                int num = peeDictionary[timestamp];
                peeDictionary[timestamp] = num + 1;
            } catch (Exception e) {
                e.ToString();
                peeDictionary.Add(timestamp, 1);
            }
        }

        foreach (DataHandler.PoopLog log in DataHandler.Poop_logs.PoopLogs) {
            string timestamp = log.timestamp.Split(' ')[0];
            if(log.type != 3 && log.type != 4) continue;
            try {
                int num = pooDictionary[timestamp];
                pooDictionary[timestamp] = num + 1;
            } catch (Exception e) {
                e.ToString();
                pooDictionary.Add(timestamp, 1);
            }
        }

        TimeHandler.DateTimeStamp indexTime =
             new TimeHandler.DateTimeStamp(DataHandler.User_creation_date);
        indexTime = indexTime - ( indexTime.Date );
        bool flag = true;
        for (int w = 1; (w <= DataHandler.User_periode+1) && flag; w++) {
            for(int i = 0; (i < 7) && flag; i ++) {
                if(TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                    indexTime,TimeHandler.CalendarCanvasTime
                    ) == 0) {
                    if (w <= 4) weekData = 4;
                    else weekData = w;
                    flag = false;
                }
                indexTime = indexTime + 1;
            }
        }
        if(flag) {
            DebugText.text = "Can't find Week Data!";
            Debug.LogError("Can't find Week Data!\n" + TimeHandler.CalendarCanvasTime.ToDateString());
        } else { DebugText.text = "Today : " + TimeHandler.CalendarCanvasTime.ToDateString() + "\n" +
                                  "weeks : " + weekData.ToString();
        }
        WeekTexts[0].text = WeekTextsList[weekData - 4];
        WeekTexts[1].text = WeekTextsList[weekData - 3];
        WeekTexts[2].text = WeekTextsList[weekData - 2];
        WeekTexts[3].text = WeekTextsList[weekData - 1];
        if (weekData == 4) UpButton.SetActive(false);
        else UpButton.SetActive(true);
        if (( new TimeHandler.DateTimeStamp(DataHandler.User_creation_date) ).Date == 0) {
            if (weekData >= DataHandler.User_periode) DownButton.SetActive(false);
            else DownButton.SetActive(true);
        } else {
            if (weekData > DataHandler.User_periode) DownButton.SetActive(false);
            else DownButton.SetActive(true);
        }
        StartCoroutine(DrawCalendarAnimation());
    }

    private bool isDataLoaded = false;
    private bool WaterIconDrawDone = false;
    private bool PeeIconDrawDone = false;

    public void UpButtonClick() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.CLICKED2);
        if (!isDataLoaded) return;
        DownButton.SetActive(true);
        weekData = ( weekData <= 4 ) ? 4 : weekData - 1;
        WeekTexts[0].text = WeekTextsList[weekData - 4];
        WeekTexts[1].text = WeekTextsList[weekData - 3];
        WeekTexts[2].text = WeekTextsList[weekData - 2];
        WeekTexts[3].text = WeekTextsList[weekData - 1];
        DrawCalendar();
        if (weekData == 4) UpButton.SetActive(false);
    }

    public void DownButtonClick() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.CLICKED2);
        if (!isDataLoaded) return;
        UpButton.SetActive(true);
        if((new TimeHandler.DateTimeStamp(DataHandler.User_creation_date)).Date == 0) {
            if (weekData < DataHandler.User_periode) {
                weekData = ( weekData >= DataHandler.User_periode ) ? DataHandler.User_periode : weekData + 1;
                WeekTexts[0].text = WeekTextsList[weekData - 4];
                WeekTexts[1].text = WeekTextsList[weekData - 3];
                WeekTexts[2].text = WeekTextsList[weekData - 2];
                WeekTexts[3].text = WeekTextsList[weekData - 1];
                DrawCalendar();
                if(weekData == DataHandler.User_periode)
                    DownButton.SetActive(false);
            } else DownButton.SetActive(false);
        } else {
            if (weekData < DataHandler.User_periode+1 ) {
                weekData = ( weekData >= DataHandler.User_periode+1 ) ? DataHandler.User_periode+1 : weekData + 1;
                WeekTexts[0].text = WeekTextsList[weekData - 4];
                WeekTexts[1].text = WeekTextsList[weekData - 3];
                WeekTexts[2].text = WeekTextsList[weekData - 2];
                WeekTexts[3].text = WeekTextsList[weekData - 1];
                DrawCalendar();
                if (weekData == DataHandler.User_periode + 1)
                    DownButton.SetActive(false);
            } else DownButton.SetActive(false);
        }
    }


    private IEnumerator DrawCalendarAnimation() {
        TimeHandler.DateTimeStamp indexTime =
            new TimeHandler.DateTimeStamp(DataHandler.User_creation_date);

        indexTime = indexTime - ( indexTime.Date );
        indexTime = indexTime + ( ( weekData - 4 ) * 7 );

        yield return new WaitForSeconds(0.3f);
        StartCoroutine(DrawWaterLogAnimation(indexTime));
        StartCoroutine(DrawPeeLogAnimation(indexTime));
        WaterIconDrawDone = false;
        PeeIconDrawDone = false;
        while (!WaterIconDrawDone || !PeeIconDrawDone) { yield return 0; }
        yield return new WaitForSeconds(0.3f);

        int todayIndex = -1;
        for (int j = 0; j < 10; j++) {
            bool flag = false;
            for (int i = 0; i < 4; i++) {
                if (stairs2[j, i] == -1) continue;
                try {
                    string indexString = ( indexTime + stairs2[j, i] ).ToDateString();
                    if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                        ( indexTime + stairs2[j, i] ), TimeHandler.CalendarCanvasTime) == 0)
                        todayIndex = stairs2[j, i];
                    int num = pooDictionary[indexString];
                    PoopGauges[stairs2[j, i]].sprite = AfterPoopGauges[0];
                    PoopGauges[stairs2[j, i]].GetComponent<Animator>().SetTrigger("Poped");
                    flag = true;
                } catch (Exception e) {
                    e.ToString();
                    PoopGauges[stairs2[j, i]].sprite = AfterPoopGauges[1];
                }
            }
            if (flag) {
                SoundHandler.Instance.Play_SFX(PoopIconSFX);
                yield return new WaitForSeconds(0.13f);
            }
        }

        if(todayIndex != -1) {
            yield return new WaitForSeconds(0.3f);
            SoundHandler.Instance.Play_SFX(SoundHandler.SFX.POPED);
            TodayMark_Water.anchoredPosition =
                WaterGauges[todayIndex].GetComponent<RectTransform>().anchoredPosition;
            TodayMark_Water.gameObject.SetActive(true);
            TodayMark_Pee.anchoredPosition =
                new Vector2(PeeGauges[todayIndex].GetComponent<RectTransform>().anchoredPosition.x,
                             227.5f - ((int)(todayIndex / 7))*165f );
            TodayMark_Pee.gameObject.SetActive(true);
        }
    }

    public IEnumerator SetPeeText(int index,string str) {
        yield return new WaitForSeconds(0.12f);
        PeeGaugeTexts[index].text = str;
    }

    public IEnumerator DrawWaterLogAnimation(TimeHandler.DateTimeStamp indexTime) {
        TimeHandler.DateTimeStamp creationTime =
            new TimeHandler.DateTimeStamp(DataHandler.User_creation_date);

        yield return new WaitForSeconds(0.12f);
        for (int j = 0; j < 10; j++) {
            bool flag = false;
            for (int i = 0; i < 4; i++) {
                if (stairs[j, i] == -1) continue;
                try {
                    string indexString = ( indexTime + stairs[j, i] ).ToDateString();
                    int num = waterDictionary[indexString];
                    int sprite_num = ( num > 10 ) ? 10 : num;
                    WaterGauges[stairs[j, i]].sprite = AfterWaterGauges[sprite_num];
                    WaterGaugeTexts[stairs[j, i]].text = num.ToString();
                    WaterGauges[stairs[j, i]].GetComponent<Animator>().SetTrigger("Poped");
                    flag = true;
                } catch (Exception e) {
                    e.ToString();
                    int result =
                        TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                            indexTime + stairs[j, i], TimeHandler.CalendarCanvasTime );
                    if (result == 1) {
                        WaterGauges[stairs[j, i]].sprite = WaterEmptyGauges;
                        WaterGaugeTexts[stairs[j, i]].text = " ";
                    } else {
                        result = TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                            indexTime + stairs[j, i], creationTime );
                        if(result == -1) {
                            WaterGauges[stairs[j, i]].sprite = WaterEmptyGauges;
                            WaterGaugeTexts[stairs[j, i]].text = " ";
                        } else {
                            WaterGauges[stairs[j, i]].sprite = AfterWaterGauges[0];
                            WaterGaugeTexts[stairs[j, i]].text = "0";
                            WaterGauges[stairs[j, i]].GetComponent<Animator>().SetTrigger("Poped");
                            flag = true;
                        }
                    }
                }
            }
            if (flag) {
                yield return new WaitForSeconds(0.13f);
            }
        }
        WaterIconDrawDone = true;
    }

    public IEnumerator DrawPeeLogAnimation(TimeHandler.DateTimeStamp indexTime) {
        TimeHandler.DateTimeStamp creationTime =
            new TimeHandler.DateTimeStamp(DataHandler.User_creation_date);

        yield return new WaitForSeconds(0.12f);
        for (int j = 0; j < 10; j++) {
            bool flag = false;
            for (int i = 0; i < 4; i++) {
                string numberString = "";
                if (stairs[j, i] == -1) continue;
                try {
                    string indexString = ( indexTime + stairs[j, i] ).ToDateString();
                    int num = peeDictionary[indexString];
                    bool peeFlag2 = false;
                    for (int k = 0; k < PeeBoundary.Length; k++) {
                        if (num <= PeeBoundary[k]) {
                            PeeGauges[stairs[j, i]].sprite = AfterPeeGauges[k];
                            peeFlag2 = true;
                            break;
                        }
                    }
                    if (!peeFlag2) PeeGauges[stairs[j, i]].sprite = AfterPeeGauges[4];
                    PeeGauges[stairs[j, i]].GetComponent<Animator>().SetTrigger("Poped");
                    numberString = num.ToString();
                    StartCoroutine(SetPeeText(stairs[j, i], numberString));
                    flag = true;
                } catch (Exception e) {
                    e.ToString();
                    int result =
                        TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                            indexTime + stairs[j, i], TimeHandler.CalendarCanvasTime
                        );
                    PeeGauges[stairs[j, i]].sprite = AfterPeeGauges[0];
                    if (result == 1) {
                        numberString = " ";
                    } else {
                        result = TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                            indexTime + stairs[j, i], creationTime );
                        if( result == -1) {
                            numberString = " ";
                        } else {
                            PeeGauges[stairs[j, i]].GetComponent<Animator>().SetTrigger("Poped");
                            numberString = "0";
                            flag = true;
                            StartCoroutine(SetPeeText(stairs[j, i], numberString));
                        }

                    }
                }
            }
            if (flag) {
                SoundHandler.Instance.Play_SFX(WaterIconSFX);
                yield return new WaitForSeconds(0.13f);
            }
        }

        PeeIconDrawDone = true;
    }


    //=======================================================================================================
    // DRAW CALENDAR
    //=======================================================================================================
    private void DrawCalendar() {
        TimeHandler.DateTimeStamp indexTime =
            new TimeHandler.DateTimeStamp(DataHandler.User_creation_date);
        TimeHandler.DateTimeStamp creationTime = indexTime;
        indexTime = indexTime - ( indexTime.Date );
        indexTime = indexTime + ( ( weekData - 4 ) * 7 );
        int todayIndex = -1;
        for (int i = 0; i < 28; i++) {
            string indexString = ( indexTime + i ).ToDateString();
            if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                        ( indexTime + i ), TimeHandler.CalendarCanvasTime) == 0)
                todayIndex = i;

            //===============================================================================================
            // PEE DRAW
            //===============================================================================================
            try {
                int num = peeDictionary[indexString];
                bool peeFlag = false;
                for (int k = 0; k < PeeBoundary.Length; k++) {
                    if (num <= PeeBoundary[k]) {
                        PeeGauges[i].sprite = AfterPeeGauges[k];
                        peeFlag = true; break;
                    }
                }
                if (!peeFlag) PeeGauges[i].sprite = AfterPeeGauges[4];
                PeeGaugeTexts[i].text = num.ToString();
            } catch (Exception e) {
                e.ToString();
                int result =
                    TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                        indexTime + i, TimeHandler.CalendarCanvasTime
                    );
                PeeGauges[i].sprite = AfterPeeGauges[0];
                if (result == 1) PeeGaugeTexts[i].text = " ";
                else { 
                    result = TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                        indexTime + i, creationTime );
                    if(result >= 0) PeeGaugeTexts[i].text = "0"; 
                    else PeeGaugeTexts[i].text = " ";
                }
            }

            //===============================================================================================
            // WATER DRAW
            //===============================================================================================
            try {
                int num = waterDictionary[indexString];
                bool waterFlag = false;
                for (int k = 0; k < WaterBoundary.Length; k++) {
                    if (num <= WaterBoundary[k]) {
                        WaterGauges[i].sprite = AfterWaterGauges[k];
                        waterFlag = true; break;
                    }
                }
                if (!waterFlag) WaterGauges[i].sprite = AfterWaterGauges[10];
                WaterGaugeTexts[i].text = num.ToString();
            } catch (Exception e) {
                e.ToString();
                int result =
                    TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                        indexTime + i, TimeHandler.CalendarCanvasTime
                    );
                if (result == 1) {
                    WaterGauges[i].sprite = WaterEmptyGauges;
                    WaterGaugeTexts[i].text = " ";
                } else {
                    result = TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                        indexTime + i, creationTime);
                    if (result >= 0) {
                        WaterGauges[i].sprite = AfterWaterGauges[0];
                        WaterGaugeTexts[i].text = "0";
                    }
                    else {
                        WaterGauges[i].sprite = WaterEmptyGauges;
                        WaterGaugeTexts[i].text = " ";
                    }
                }
            }

            //===============================================================================================
            // POOP DRAW
            //===============================================================================================
            try {
                int num = pooDictionary[indexString];
                PoopGauges[i].sprite = AfterPoopGauges[0];
            } catch (Exception e) {
                e.ToString();
                PoopGauges[i].sprite = AfterPoopGauges[1];
            }
        }

        if (todayIndex != -1) {
            TodayMark_Water.anchoredPosition =
                WaterGauges[todayIndex].GetComponent<RectTransform>().anchoredPosition;
            TodayMark_Water.gameObject.SetActive(true);
            TodayMark_Pee.anchoredPosition =
                new Vector2(PeeGauges[todayIndex].GetComponent<RectTransform>().anchoredPosition.x,
                             227.5f - ( (int)( todayIndex / 7 ) ) * 165f);
            TodayMark_Pee.gameObject.SetActive(true);
        } else {
            TodayMark_Water.gameObject.SetActive(false);
            TodayMark_Pee.gameObject.SetActive(false);
        }
    }
}
