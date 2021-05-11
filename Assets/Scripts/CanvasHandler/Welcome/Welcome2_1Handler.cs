using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Welcome2_1Handler : MonoBehaviour {
    public Text subtext;
    public Image MaleButton;
    public Image FemaleButton;
    public Text MaleButtonText;
    public Text FemaleButtonText;

    public Text YearsText;
    public Text MonthsText;
    public Text DaysText;

    private string genderValue;

    public Button NextButton;

    public Button YearsMinusButton;
    public Button YearsPlusButton;
    public Button MonthsMinusButton;
    public Button MonthsPlusButton;
    public Button DaysMinusButton;
    public Button DaysPlusButton;

    private TimeHandler.DateTimeStamp currentTime;
    private TimeHandler.DateTimeStamp todayTime;

    public void Start() {
        if (DataHandler.User_name == null)
            StartCoroutine(SetLabel());
        else {
            string labelText = DataHandler.User_name.Remove(0, 1);
            if (KoreanUnderChecker.UnderCheck(labelText))
                labelText += "이는...";
            else labelText += "는...";
            subtext.text = labelText;
        }

        TimeHandler.GetCurrentTime();
        currentTime = TimeHandler.CurrentTime;
        todayTime = TimeHandler.CurrentTime;
        SetBirthDay(currentTime);
    }

    public void SetBirthDay(TimeHandler.DateTimeStamp stamp) {
        YearsText.text = stamp.Years.ToString();
        MonthsText.text = stamp.Months.ToString();
        DaysText.text = stamp.Days.ToString();

        if (stamp.Years >= todayTime.Years) YearsPlusButton.interactable = false;
        else YearsPlusButton.interactable = true;

        if (stamp.Years <= 0) YearsMinusButton.interactable = false;
        else YearsMinusButton.interactable = true;

        if (stamp.Months >= todayTime.Months) MonthsPlusButton.interactable = false;
        else MonthsPlusButton.interactable = true;

        if (stamp.Months <= 0) MonthsMinusButton.interactable = false;
        else MonthsMinusButton.interactable = true;

        if (stamp.Days >= todayTime.Days) DaysPlusButton.interactable = false;
        else DaysPlusButton.interactable = true;

        if (stamp.Days <= 0) DaysMinusButton.interactable = false;
        else DaysMinusButton.interactable = true;
    }

    IEnumerator SetLabel() {
        while (true) {
            yield return 0;
            if (DataHandler.User_isDataLoaded) {
                string labelText = DataHandler.User_name.Remove(0, 1);
                if (KoreanUnderChecker.UnderCheck(labelText))
                    labelText += "이는...";
                else labelText += "는...";
                subtext.text = labelText;
            }
        }
    }

    public void MaleButtonClick() {
        MaleButton.color = new Color(1, 1, 1, 1);
        FemaleButton.color = new Color(0.4f, 0.4f, 0.4f, 1f);
        genderValue = "male";
        MaleButtonText.color = new Color(1, 1, 1, 1);
        FemaleButtonText.color = new Color(0.4f, 0.4f, 0.4f, 1f);
        NextButton.interactable = true;
    }

    public void FemaleButtonClick() {
        FemaleButton.color = new Color(1, 1, 1, 1);
        MaleButton.color = new Color(0.4f, 0.4f, 0.4f, 1f);
        genderValue = "female";
        FemaleButtonText.color = new Color(1, 1, 1, 1);
        MaleButtonText.color = new Color(0.4f, 0.4f, 0.4f, 1f);
        NextButton.interactable = true;
    }

    public void YearsMinusButtonClick() {
        currentTime.Years = ( currentTime.Years - 1 < 0 ) ?
             0 : currentTime.Years - 1;
        validationCheck();
        SetBirthDay(currentTime);
    }
    public void YearsPlusButtonClick() {
        currentTime.Years = ( currentTime.Years + 1 > todayTime.Years ) ?
             currentTime.Years : currentTime.Years + 1;
        validationCheck();
        SetBirthDay(currentTime);
    }
    public void MonthsMinusButtonClick() {
        currentTime.Months = ( currentTime.Months - 1 < 1 ) ?
             1 : currentTime.Months - 1;
        validationCheck();
        SetBirthDay(currentTime);
    }
    public void MonthsPlusButtonClick() {
        currentTime.Months = ( currentTime.Months + 1 > 12 ) ?
             12 : currentTime.Months + 1;
        validationCheck();
        SetBirthDay(currentTime);
    }


    public void DaysMinusButtonClick() {
        currentTime.Days = ( currentTime.Days - 1 < 1 ) ?
             1 : currentTime.Days - 1;

        SetBirthDay(currentTime);
    }
    public void DaysPlusButtonClick() {
        if(TimeHandler.DateTimeStamp.isLeafYear(currentTime.Years)) {
            currentTime.Days = ( currentTime.Days + 1 >
                TimeHandler.DateTimeStamp.LeafYearDaysList[currentTime.Months-1]) ?
                TimeHandler.DateTimeStamp.LeafYearDaysList[currentTime.Months - 1] : currentTime.Days + 1;
        } else {
            currentTime.Days = ( currentTime.Days + 1 >
                TimeHandler.DateTimeStamp.NormalYearDaysList[currentTime.Months - 1] ) ?
                TimeHandler.DateTimeStamp.NormalYearDaysList[currentTime.Months - 1] : currentTime.Days + 1;
        }

        SetBirthDay(currentTime);
    }

    public void validationCheck() {
        if (TimeHandler.DateTimeStamp.isLeafYear(currentTime.Years)) {
            currentTime.Days = ( currentTime.Days >
                TimeHandler.DateTimeStamp.LeafYearDaysList[currentTime.Months - 1] ) ?
                TimeHandler.DateTimeStamp.LeafYearDaysList[currentTime.Months - 1] : currentTime.Days;
        } else {
            currentTime.Days = ( currentTime.Days >
                TimeHandler.DateTimeStamp.NormalYearDaysList[currentTime.Months - 1] ) ?
                TimeHandler.DateTimeStamp.NormalYearDaysList[currentTime.Months - 1] : currentTime.Days;
        }
    }

    public void OkayButton() {
        DataHandler.User_gender = genderValue;
        DataHandler.User_birthday = currentTime.ToDateString();
        TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.WELCOME2].SetActive(true);
        this.gameObject.SetActive(false);
    }

}
