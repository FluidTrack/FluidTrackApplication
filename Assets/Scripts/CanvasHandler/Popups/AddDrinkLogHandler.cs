using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddDrinkLogHandler : MonoBehaviour
{
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
        LogCanvasHandler.Instance.DrinkWindow_ScrollAutoAdjust = true;
        OkayButton.interactable = false;
    }

    public void OnDisable() {
        LogCanvasHandler.Instance.DrinkWindow_ScrollAutoAdjust = false;
        isClicked = false;
        isChanged = false;
        Scroll.value = 0;
        Label.text = "";
    }

    public void Update() {
        OkayButton.interactable = isClicked && isChanged;
    }
}
