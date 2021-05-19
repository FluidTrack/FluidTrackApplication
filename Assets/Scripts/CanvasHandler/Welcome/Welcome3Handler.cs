using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Welcome3Handler : MonoBehaviour
{
    const string ACTION_SET_ALARM = "android.intent.action.SET_ALARM";
    const string EXTRA_HOUR = "android.intent.extra.alarm.HOUR";
    const string EXTRA_MINUTES = "android.intent.extra.alarm.MINUTES";
    const string EXTRA_MESSAGE = "android.intent.extra.alarm.MESSAGE";
    const string EXTRA_DAYS = "android.intent.extra.alarm.DAYS";
    const string EXTRA_SKIP_UI = "android.intent.extra.alarm.SKIP_UI";
    public Text subtext;
    public TimerHandler Morning;
    public TimerHandler School;
    public TimerHandler Home;

    public void Start() {
        if (DataHandler.User_name == null)
            StartCoroutine(SetLabel());
        else {
            string labelText = DataHandler.User_name_back;

            if (KoreanUnderChecker.UnderCheck(labelText))
                labelText += "이의 하루";
            else labelText += "의 하루";
            subtext.text = labelText;
        }
    }

    IEnumerator SetLabel() {
        while (true) {
            yield return 0;
            if (DataHandler.User_isDataLoaded) {
                string labelText = DataHandler.User_name_back;

                if (KoreanUnderChecker.UnderCheck(labelText))
                    labelText += "이의 하루";
                else labelText += "의 하루";
                subtext.text = labelText;
            }
        }
    }

    public void OkayButton() {
        DataHandler.User_morning_call_time = Morning.getTime();
        DataHandler.User_school_time = School.getTime();
        DataHandler.User_home_time = Home.getTime();
        TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.WELCOME5].SetActive(true);
        MorningCallButtonClick();
        this.gameObject.SetActive(false);
    }

    public void PrevButton() {
        TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.WELCOME2].SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void MorningCallButtonClick() {
#if UNITY_EDITOR
        Debug.Log(Morning.getTime());
#elif UNITY_ANDROID
        CreateAlarm("일어날시간!", Morning.Hour, Morning.Min);
#endif
    }


    public void CreateAlarm(string message,int hour, int minutes ) {
        var intentAJO = new AndroidJavaObject("android.content.Intent", ACTION_SET_ALARM);
        int[] daySelect = { 7, 2,3,4,5,6,7};

        intentAJO
            .Call<AndroidJavaObject>("putExtra", EXTRA_MESSAGE, message)
            .Call<AndroidJavaObject>("putExtra", EXTRA_HOUR, hour)
            .Call<AndroidJavaObject>("putExtra", EXTRA_MINUTES, minutes)
            .Call<AndroidJavaObject>("putExtra", EXTRA_SKIP_UI, true)
            .Call<AndroidJavaObject>("putExtra", EXTRA_DAYS, daySelect);
        GetUnityActivity().Call("startActivity", intentAJO);
    }

    AndroidJavaObject GetUnityActivity() {
        using( var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
            return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }
    }
}
