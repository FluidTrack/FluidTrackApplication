using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModifyDrinkLogHandler : MonoBehaviour
{
    public static DataHandler.DrinkLog Target;
    public Button OkayButton;
    public Scrollbar Scroll;
    public bool isClicked;
    public bool isChanged;
    public Text Label;

    public void ChangeVolumeScroll() {
        isChanged = true;
    }

    public void ClickTypeButton() {
        isClicked = true;
    }

    public void OnEnable() {
        OkayButton.interactable = false;
        if(Target != null) {
            Scroll.value = ((float)1f/(float)7f) * ( ( Target.volume ) / 50 );
            Label.text = Target.volume + " ml";
            TimeHandler.DateTimeStamp targetTime = new TimeHandler.DateTimeStamp(Target.timestamp);
            LogCanvasHandler.Instance.InitDrinkModify(Target.type,targetTime.Hours);
        }
    }

    public void OnDisable() {
        LogCanvasHandler.Instance.DrinkWindow_ScrollAutoAdjust = false;
        isClicked = false;
        isChanged = false;
        Scroll.value = 0;
        Label.text = "";
    }

    public void Update() {
        OkayButton.interactable = isClicked || isChanged;
    }
}
