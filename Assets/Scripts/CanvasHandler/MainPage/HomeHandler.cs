using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeHandler : MonoBehaviour
{
    public Text GardenName;
    public TimeHandler.DateTimeStamp[] TimeList;
    public Text[] DateList;
    public Image[] GardenSpots;
    public Image[] GrassSpots;
    public Sprite[] Flowers;
    public Sprite Grass;
    public Sprite empty;
    public Sprite Arrow;
    public Sprite BrightArrow;
    public Image LeftButton;
    public Image RightButton;
    public Text TimeLineText;
    public Text WaterLogText;
    public Text DrinkLogText;
    public Text PeeLogText;
    public Text PooLogText;
    private DataHandler.GardenLog[] logs;

    public void Start() {
        TimeList = new TimeHandler.DateTimeStamp[7];
        try {
            logs = DataHandler.Garden_logs.GardenLogs;
        } catch (System.Exception e) {
            e.ToString();
            logs = null;
        }
        StartCoroutine(SetGardenName());
        SetGardenTimeList();
    }

    public void DrawGarden() {
        if (logs == null) return;
        bool flag = false;
        int next = -1;

        for (int i = 0; i < 7; i++) {
            string target = TimeList[i].ToDateString();
            if(!flag) {
                for(int j = 0; j < logs.Length; j ++) {
                    if(target == logs[j].timestamp.Split(' ')[0]) {
                        //Draw i-th garden spot
                        GrassSpots[i].sprite = Grass;
                        GardenSpots[i].sprite = Flowers[logs[j].flower];
                        next = j + 1;
                        flag = true;
                    }
                }
                if(!flag) {
                    // Draw i-th garden is empty
                    GrassSpots[i].sprite = empty;
                    GardenSpots[i].sprite = Flowers[0];
                }
            } else {
                if(next >= logs.Length) {
                    // Draww i-th garden is empty
                    GrassSpots[i].sprite = empty;
                    GardenSpots[i].sprite = Flowers[0];
                }else if (target == logs[next].timestamp.Split(' ')[0]) {
                    //Draw i-th garden spot
                    GrassSpots[i].sprite = Grass;
                    GardenSpots[i].sprite = Flowers[logs[next].flower];
                    next += 1;
                } else {
                    // Draww i-th garden is empty
                    GrassSpots[i].sprite = empty;
                    GardenSpots[i].sprite = Flowers[0];
                }
            }
        }
    }

    public void SetGardenTimeList() {
        for(int i = 0; i< 7; i++) {
            if(i - 4 < 0)     
                TimeList[i] = TimeHandler.HomeCanvasTime - ( 4 - i );
            else if (i - 4 > 0)
                TimeList[i] = TimeHandler.HomeCanvasTime + (i - 4 );
            else TimeList[i] = TimeHandler.HomeCanvasTime;
        }
        for (int i = 0; i < 7; i++)
            if(TimeList[i].ToDateString() == TimeHandler.CurrentTime.ToDateString())
                DateList[i].text = "*" + TimeHandler.DateTimeStamp.DateList[TimeList[i].Date] + "*";
            else 
                DateList[i].text = TimeHandler.DateTimeStamp.DateList[TimeList[i].Date];
    }

    public void PrevGardenPage() {
        if (logs == null) return;
        TimeHandler.HomeCanvasTime -= 5;
        if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.CreationTime, TimeHandler.HomeCanvasTime) == 1) {
            TimeHandler.HomeCanvasTime += 5;
            return;
        }
        TimeHandler.HomeCanvasTime -= 2;
        SetGardenTimeList();
        DrawGarden();
        DrawArrow();
    }

    public void NextGardenPage() {
        if (logs == null) return;
        TimeHandler.HomeCanvasTime += 7;
        if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.CurrentTime, TimeHandler.HomeCanvasTime) == -1) {
            TimeHandler.HomeCanvasTime -= 7;
            return;
        }
        SetGardenTimeList();
        DrawGarden();
        DrawArrow();
    }

    public void DrawArrow() {
        if (logs == null) return;
        TimeHandler.HomeCanvasTime -= 5;
        if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.CreationTime, TimeHandler.HomeCanvasTime) == 1)
            LeftButton.sprite = BrightArrow;
        else LeftButton.sprite = Arrow;
        TimeHandler.HomeCanvasTime += 5;

        TimeHandler.HomeCanvasTime += 7;
        if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.CurrentTime, TimeHandler.HomeCanvasTime) == -1) 
            RightButton.sprite = BrightArrow;
        else RightButton.sprite = Arrow;
        TimeHandler.HomeCanvasTime -= 7;
    }

    IEnumerator SetGardenName() {
        while(true) {
            yield return 0;
            if(DataHandler.User_isDataLoaded) {
                string middleName = DataHandler.User_name.Remove(0, 1);
                GardenName.text =
                    ( KoreanUnderChecker.UnderCheck(DataHandler.User_name) ) ?
                    middleName + "이네 정원" : middleName + "네 정원";
                break;
            }
        }
        StartCoroutine(GetGardenLogList());
    }

    public void OnEnable() {
        TimeHandler.GetCurrentTime();
        TimeLineText.text = TimeHandler.CurrentTime.Years + "년\n" +
                            TimeHandler.CurrentTime.Months + "월 " +
                            TimeHandler.CurrentTime.Days + "일";
        if (DataHandler.User_isDataLoaded) {
            StartCoroutine(GetGardenLogList());
        }
        StartCoroutine(FetchData());
    }

    IEnumerator GetGardenLogList() {
        StartCoroutine(DataHandler.ReadGardenLogs(DataHandler.User_id));
        while (true) {
            yield return 0;
            if (DataHandler.User_isGardenDataLoaded) {
                if (DataHandler.Garden_logs.GardenLogs == null)
                    logs = null;
                else logs = DataHandler.Garden_logs.GardenLogs;
                break;
            }
        }
        DrawGarden();
        SetGardenTimeList();
        DrawArrow();
        yield return 0;
    }

    public IEnumerator FetchData() {
        while (true) {
            yield return 0;
            if (DataHandler.User_isDataLoaded) {
                DataHandler.User_isPeeDataLoaded = false;
                DataHandler.User_isWaterDataLoaded = false;
                DataHandler.User_isPooDataLoaded = false;
                StartCoroutine(DataHandler.ReadWaterLogs(DataHandler.User_id));
                StartCoroutine(DataHandler.ReadPeeLogs(DataHandler.User_id));
                StartCoroutine(DataHandler.ReadPoopLogs(DataHandler.User_id));
                StartCoroutine(DataLoadCompleted());
                break;
            }
        }
    }

    IEnumerator DataLoadCompleted() {
        while (true) {
            yield return 0;
            if (DataHandler.User_isWaterDataLoaded &&
               DataHandler.User_isPeeDataLoaded &&
               DataHandler.User_isPooDataLoaded) {
                DataHandler.User_isPeeDataLoaded = false;
                DataHandler.User_isWaterDataLoaded = false;
                DataHandler.User_isPooDataLoaded = false;

                CreateIcon(TimeHandler.LogCanvasTime);
                break;
            }
        }
    }


    public void CreateIcon(TimeHandler.DateTimeStamp stamp) {
        DataHandler.WaterLog[] waterList = DataHandler.Water_logs.WaterLogs;
        DataHandler.PeeLog[] peeList = DataHandler.Pee_logs.PeeLogs;
        DataHandler.PoopLog[] pooList = DataHandler.Poop_logs.PoopLogs;

        int waterCountInt = 0, drinkCountInt = 0;
        for (int i = 0; i < waterList.Length; i++) {
            if (TimeHandler.DateTimeStamp.
                CmpDateTimeStamp(waterList[i].timestamp,
                stamp.ToString()) == 0)
                if (waterList[i].type == 0)
                    waterCountInt++;
                else drinkCountInt++;
        }
        WaterLogText.text = waterCountInt.ToString();
        DrinkLogText.text = drinkCountInt.ToString();

        int peeCountInt = 0;
        for (int i = 0; i < peeList.Length; i++) {
            if (TimeHandler.DateTimeStamp.
                CmpDateTimeStamp(peeList[i].timestamp,
                stamp.ToString()) == 0) 
                peeCountInt++;
        }
        PeeLogText.text = peeCountInt.ToString();

        int pooCountInt = 0;
        for (int i = 0; i < pooList.Length; i++) {
            if (TimeHandler.DateTimeStamp.
                CmpDateTimeStamp(pooList[i].timestamp,
                stamp.ToString()) == 0)
                pooCountInt++;
        }
        PooLogText.text = pooCountInt.ToString();
        Debug.Log("Done");
    }
}
