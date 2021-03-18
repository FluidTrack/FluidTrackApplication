using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerPageHandler : MonoBehaviour
{
    public string TargetDateString = "2021-03-01";
    private TimeHandler.DateTimeStamp targetDate;

    public void OnEnable() {
        StartCoroutine(DataHandler.ReadGardenLogs(DataHandler.User_id));
        StartCoroutine(dataLoadCheck());
        targetDate = new TimeHandler.DateTimeStamp(TargetDateString);
    }
    
    IEnumerator dataLoadCheck() {
        while(!DataHandler.User_isGardenDataLoaded) {
            yield return 0;
        }
        DataHandler.User_isGardenDataLoaded = false;
        isLoaded();
    }

    public void isLoaded() {
        Debug.Log(DataHandler.Garden_logs.GardenLogs.Length);
    }
}
