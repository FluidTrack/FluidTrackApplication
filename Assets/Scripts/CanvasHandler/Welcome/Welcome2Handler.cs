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

    public void Start() {
        if(DataHandler.User_name == null)
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

    public void NextButton() {
        DataHandler.User_breakfast_time = Breakfast.getTime();
        DataHandler.User_lunch_time = Lunch.getTime();
        DataHandler.User_dinner_time = Dinner.getTime();

        TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.WELCOME3].SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void PrevButton() {
        TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.WELCOME].SetActive(true);
        this.gameObject.SetActive(false);
    }
}
