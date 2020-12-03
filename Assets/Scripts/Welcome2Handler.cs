using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Welcome2Handler : MonoBehaviour
{
    public Text subtext;
    public TimerHandler Breakfast;
    public TimerHandler Lunch;
    public TimerHandler Dinner;

    private void OnEnable() {
        subtext.text = DataHandler.User_name + "의 하루!";
    }

    public void NextButton() {
        DataHandler.User_breakfast_time = Breakfast.getTime();
        DataHandler.User_lunch_time = Lunch.getTime();
        DataHandler.User_dinner_time = Dinner.getTime();

        TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.WELCOME3].SetActive(true);
        this.gameObject.SetActive(false);
    }
}
