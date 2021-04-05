using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeHandler : MonoBehaviour
{
    public static HomeHandler Instance;
    public MainPageHeaderHandler HeaderHandler;
    public Transform GardenSpotParents;
    public GameObject ReturnButton;

    public GardenSpotHandler[] Spots;

    public void Awake() {
        Instance = this;
    }

    public void Start() {
        Spots = GardenSpotParents.GetComponentsInChildren<GardenSpotHandler>();
    }

    public void OnEnable() {
        TimeHandler.GetCurrentTime();
        StartCoroutine(CheckUserDataLoad());
        HeaderHandler.WriteTodayDate(TimeHandler.HomeCanvasTime);
    }

    public IEnumerator CheckUserDataLoad() {
        while (!DataHandler.User_isDataLoaded) {
            yield return 0;
        }
        StartCoroutine(DataHandler.ReadGardenLogs(DataHandler.User_id));
        StartCoroutine(CheckGardenDataLoad());

    }

    public IEnumerator CheckGardenDataLoad() {
        yield return 0;
        while (!DataHandler.User_isGardenDataLoaded) {
            yield return 0;
        }
        DataHandler.User_isGardenDataLoaded = false;
        InitGardenSpot();
    }

    public void InitGardenSpot() {
        if (!DataHandler.User_isDataLoaded) {
            Debug.LogWarning("Please User data Load first!");
            return;
        }
        TimeHandler.DateTimeStamp inputDate =
            new TimeHandler.DateTimeStamp(DataHandler.User_creation_date);
        DataHandler.GardenLog[] logs = DataHandler.Garden_logs.GardenLogs;
        int index = 0;
        foreach(GardenSpotHandler spot in Spots) {
            DataHandler.GardenLog inputData = null;
            while(true) {
                if (index < logs.Length) {
                    TimeHandler.DateTimeStamp targetDate =
                        new TimeHandler.DateTimeStamp(logs[index].timestamp);
                    int cmpResult = TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                        inputDate.ToDateString(), targetDate.ToDateString());

                    if (cmpResult == 0) {
                        inputData = logs[index];
                        index++;
                        break;
                    } else if ( cmpResult == -1 ) { break; }
                      else if ( cmpResult ==  1 ) { index++; }
                }else { break; }
            }
            spot.InitSpot(inputData,inputDate);
            inputDate = inputDate+1;
        }
    }

    public void ReturnButtonClick() {
        Debug.Log(TimeHandler.HomeCanvasTime.ToDateString());
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.CLICKED);
        ReturnButton.SetActive(false);
        return;
    }
}
