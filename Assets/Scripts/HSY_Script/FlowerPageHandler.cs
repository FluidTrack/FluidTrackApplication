using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerPageHandler : MonoBehaviour
{
    public string TargetDateString = "2021-03-01";
    private TimeHandler.DateTimeStamp targetDate;
    private TimeHandler.DateTimeStamp loopDate;
    private DataHandler.GardenLog[] logsGarden;

    public void OnEnable() {
        StartCoroutine(DataHandler.ReadGardenLogs(1));
        StartCoroutine(dataLoadCheck());
        targetDate = new TimeHandler.DateTimeStamp(TargetDateString); //convert timestamp(string) to timestamp(class)

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
        logsGarden = DataHandler.Garden_logs.GardenLogs;
        if (logsGarden.Length == 0) //if the user have not created logs yet
        {
            Debug.Log("There is no log of the user");
            return;
        }

        for (int i = 0; i < logsGarden.Length; i++)
        {
                loopDate = new TimeHandler.DateTimeStamp(logsGarden[i].timestamp);
                Debug.Log(">> " + loopDate.ToDateString());
                if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(targetDate, loopDate) == 0)
                {  
                    Debug.Log(logsGarden[i].timestamp);
                    Debug.Log(logsGarden[i].flower);
                    Debug.Log(logsGarden[i].item_0);
                    return;
                }
        }
        Debug.Log("There is no log of target date"); //if there is no log of target date

    }
}
