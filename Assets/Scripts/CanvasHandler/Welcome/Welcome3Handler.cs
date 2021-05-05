using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Welcome3Handler : MonoBehaviour
{
    public Text subtext;
    public TimerHandler Morning;
    public TimerHandler School;
    public TimerHandler Home;

    public void Start() {
        if (DataHandler.User_name == null)
            StartCoroutine(SetLabel());
        else {
            string labelText = DataHandler.User_name.Remove(0, 1);
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
                string labelText = DataHandler.User_name.Remove(0, 1);
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
        this.gameObject.SetActive(false);
    }

    public void PrevButton() {
        TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.WELCOME2].SetActive(true);
        this.gameObject.SetActive(false);
    }
}
