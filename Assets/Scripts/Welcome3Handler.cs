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

    private void OnEnable() {
        subtext.text = DataHandler.User_name + "의 하루!";
    }

    public void OkayButton() {
        DataHandler.User_morning_call_time = Morning.getTime();
        DataHandler.User_school_time = School.getTime();
        DataHandler.User_home_time = Home.getTime();
        TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.WELCOME4].SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void PrevButton() {
        TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.WELCOME2].SetActive(true);
        this.gameObject.SetActive(false);
    }
}
