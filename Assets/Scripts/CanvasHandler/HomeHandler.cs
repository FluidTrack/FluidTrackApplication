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
                        Debug.Log("flower : " + logs[j].flower);
                        next = j + 1;
                        flag = true;
                    }
                }
                if(!flag) {
                    // Draw i-th garden is empty
                    GrassSpots[i].sprite = empty;
                    GardenSpots[i].sprite = Flowers[0];
                    Debug.Log("empty " + target);
                }
            } else {
                if(next >= logs.Length) {
                    // Draww i-th garden is empty
                    GrassSpots[i].sprite = empty;
                    GardenSpots[i].sprite = Flowers[0];
                    Debug.Log("empty " + target);
                }else if (target == logs[next].timestamp.Split(' ')[0]) {
                    //Draw i-th garden spot
                    GrassSpots[i].sprite = Grass;
                    GardenSpots[i].sprite = Flowers[logs[next].flower];
                    Debug.Log("flower : " + logs[next].flower);
                    next += 1;
                } else {
                    // Draww i-th garden is empty
                    GrassSpots[i].sprite = empty;
                    GardenSpots[i].sprite = Flowers[0];
                    Debug.Log("empty " + target);
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
        if (DataHandler.User_isDataLoaded) {
            StartCoroutine(GetGardenLogList());
        }
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

}
